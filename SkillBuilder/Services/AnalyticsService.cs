using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SkillBuilder.Data; // Adjust if your DbContext namespace differs
using SkillBuilder.Models.Analytics;

namespace SkillBuilder.Services
{
    public interface IAnalyticsService
    {
        Task<AnalyticsOverviewDto> GetOverviewAsync(int monthsBack = 6);
    }

    public class AnalyticsService : IAnalyticsService
    {
        private readonly AppDbContext _db;
        public AnalyticsService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<AnalyticsOverviewDto> GetOverviewAsync(int monthsBack = 6)
        {
            var result = new AnalyticsOverviewDto();

            // A: basic counts
            result.TotalLearners = await _db.Users.CountAsync(u => u.Role == "Learner" && !u.IsArchived);
            // Use Artisan table for artisan count if exists; fallback to Users role
            result.TotalArtisans = await _db.Artisans.CountAsync(a => !a.IsArchived);
            result.TotalCourses = await _db.Courses.CountAsync(c => !c.IsArchived);

            // B: monthly enrollments for last `monthsBack` months (inclusive)
            var now = DateTime.UtcNow;
            var start = new DateTime(now.Year, now.Month, 1).AddMonths(-(monthsBack - 1));
            var end = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59, 999, DateTimeKind.Utc);

            var enrollments = await _db.Enrollments
                .Where(e => e.EnrolledAt >= start)
                .GroupBy(e => new { e.EnrolledAt.Year, e.EnrolledAt.Month })
                .Select(g => new MonthlyEnrollmentDto
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Count = g.Count()
                })
                .ToListAsync();

            // Filter by date range if needed
            var filteredEnrollments = await _db.Enrollments
                .Where(e => e.EnrolledAt >= start && e.EnrolledAt <= end)
                .ToListAsync();

            var totalEnrollments = filteredEnrollments.Count;
            var completedEnrollments = filteredEnrollments.Count(e => e.IsCompleted);

            result.CompletionRate = totalEnrollments == 0 ? 0 : (int)Math.Round((double)completedEnrollments / totalEnrollments * 100);

            // Enrollment Growth: compare last month vs previous month
            var monthlyGroups = filteredEnrollments
                .GroupBy(e => new { e.EnrolledAt.Year, e.EnrolledAt.Month })
                .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count() })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToList();

            if (monthlyGroups.Count >= 2)
            {
                var last = monthlyGroups[monthlyGroups.Count - 1].Count;
                var prev = monthlyGroups[monthlyGroups.Count - 2].Count;
                result.EnrollmentGrowth = prev == 0 ? (last > 0 ? 100 : 0) : (int)Math.Round((double)(last - prev) / prev * 100);
            }
            else
            {
                result.EnrollmentGrowth = 0;
            }

            // ensure months with zero are present (ordered)
            var monthList = new List<MonthlyEnrollmentDto>();
            for (int i = 0; i < monthsBack; i++)
            {
                var dt = start.AddMonths(i);
                var found = enrollments.FirstOrDefault(m => m.Year == dt.Year && m.Month == dt.Month);
                monthList.Add(found ?? new MonthlyEnrollmentDto { Year = dt.Year, Month = dt.Month, Count = 0 });
            }
            result.MonthlyEnrollments = monthList;

            // Define TOP categories
            var TOP_CATEGORIES = new[] { "Pottery", "Weaving", "Shoemaking", "Woodcarving", "Embroidery" };

            // Monthly category enrollments
            var categoryMonthlyEnrollments = await _db.Courses
                .Where(c => !c.IsArchived)
                .SelectMany(c => c.Enrollments, (c, e) => new { c.Category, e.EnrolledAt })
                .Where(x => x.EnrolledAt >= start && x.EnrolledAt <= end)
                .ToListAsync(); // materialize first to apply "Others" mapping in memory

            var categoryMonthlyDto = _db.Enrollments
                .Where(e => e.EnrolledAt >= start && e.EnrolledAt <= end)
                .Join(_db.Courses, e => e.CourseId, c => c.Id, (e, c) => new { c.Category, e.EnrolledAt })
                .AsEnumerable() // switch to memory
                .GroupBy(x =>
                {
                    var cat = string.IsNullOrWhiteSpace(x.Category) ? "Others" : x.Category;
                    return TOP_CATEGORIES.Contains(cat) ? cat : "Others";
                })
                .SelectMany(g => g.GroupBy(e => new { e.EnrolledAt.Year, e.EnrolledAt.Month, Category = g.Key })
                                  .Select(m => new CategoryMonthlyEnrollmentDto
                                  {
                                      Year = m.Key.Year,
                                      Month = m.Key.Month,
                                      Category = m.Key.Category,
                                      Count = m.Count()
                                  }))
                .ToList();

            result.CategoryMonthlyEnrollments = categoryMonthlyDto;

            var months = Enumerable.Range(0, monthsBack)
                .Select(i => start.AddMonths(i))
                .ToList();

            foreach (var cat in TOP_CATEGORIES.Append("Others"))
            {
                foreach (var m in months)
                {
                    if (!categoryMonthlyDto.Any(x => x.Category == cat && x.Year == m.Year && x.Month == m.Month))
                    {
                        categoryMonthlyDto.Add(new CategoryMonthlyEnrollmentDto
                        {
                            Category = cat,
                            Year = m.Year,
                            Month = m.Month,
                            Count = 0
                        });
                    }
                }
            }

            // Category totals (for the chart legend)
            var categoryTotals = await _db.Courses
                .Where(c => !c.IsArchived)
                .SelectMany(c => c.Enrollments.DefaultIfEmpty(), (c, e) => new { c.Category, Enrollment = e })
                .ToListAsync();

            var categoryDto = categoryTotals
                .GroupBy(x =>
                {
                    var cat = string.IsNullOrWhiteSpace(x.Category) ? "Others" : x.Category;
                    return TOP_CATEGORIES.Contains(cat) ? cat : "Others";
                })
                .Select(g => new CategoryEnrollmentDto
                {
                    Category = g.Key,
                    TotalStudents = g.Count(x => x.Enrollment != null)
                })
                .OrderByDescending(c => c.TotalStudents)
                .ToList();

            result.CategoryEnrollments = categoryDto;

            // C: enrollment by category
            var categoryQuery = await _db.Courses
                .Where(c => !c.IsArchived)
                .SelectMany(c => c.Enrollments.DefaultIfEmpty(), (c, e) => new { c.Category, Enrollment = e })
                .GroupBy(x => x.Category)
                .Select(g => new CategoryEnrollmentDto
                {
                    Category = g.Key ?? "Uncategorized",
                    TotalStudents = g.Count(x => x.Enrollment != null) // count actual enrollments
                })
                .OrderByDescending(c => c.TotalStudents)
                .ToListAsync();

            result.CategoryEnrollments = categoryQuery;

            // D: Top courses by total enroll + average rating
            var topCourses = await _db.Courses
                .Where(c => !c.IsArchived)
                .Select(c => new TopCourseDto
                {
                    CourseId = c.Id,
                    Title = c.Title,
                    AverageRating = c.Reviews.Any() ? c.Reviews.Average(r => r.Rating) : 0,
                    TotalEnroll = c.Enrollments.Count(),
                    Category = c.Category,
                    IsPublished = c.IsPublished
                })
                .OrderByDescending(c => c.TotalEnroll)
                .ThenByDescending(c => c.AverageRating)
                .Take(6)
                .ToListAsync();

            result.TopCourses = topCourses;

            // E: Top artisans - aggregate enrollments across their courses
            var topArtisans = await _db.Artisans
                .Where(a => !a.IsArchived)
                .Select(a => new
                {
                    a.ArtisanId,
                    Name = (a.FirstName + " " + a.LastName).Trim(),
                    Skill = a.Profession,
                    Location = a.Hometown,
                    TotalStudents = a.Courses.SelectMany(c => c.Enrollments).Count()
                })
                .OrderByDescending(a => a.TotalStudents)
                .Take(6)
                .ToListAsync();

            result.TopArtisans = topArtisans.Select(a => new TopArtisanDto
            {
                ArtisanId = a.ArtisanId,
                Name = a.Name,
                Skill = a.Skill,
                Location = a.Location,
                TotalStudents = a.TotalStudents
            }).ToList();

            // F: Top artisans by total enrollments across all their courses (for a different view)
            var topArtisansByEnrollments = await _db.Artisans
                .Where(a => !a.IsArchived)
                .Select(a => new
                {
                    a.ArtisanId,
                    Name = (a.FirstName + " " + a.LastName).Trim(),
                    Skill = a.Profession,
                    Location = a.Hometown,
                    TotalStudents = a.Courses.Sum(c => c.Enrollments.Count())
                })
                .OrderByDescending(a => a.TotalStudents)
                .Take(5)
                .ToListAsync();

            // Map to DTO
            var topArtisansByEnrollmentsDto = topArtisansByEnrollments.Select(a => new TopArtisanDto
            {
                ArtisanId = a.ArtisanId,
                Name = a.Name,
                Skill = a.Skill,
                Location = a.Location,
                TotalStudents = a.TotalStudents
            }).ToList();

            // You can either return this separately or add to AnalyticsOverviewDto as a new property
            result.TopArtisansByEnrollments = topArtisansByEnrollmentsDto;

            return result;
        }
    }
}