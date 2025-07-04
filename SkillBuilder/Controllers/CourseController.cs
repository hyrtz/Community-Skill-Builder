﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillBuilder.Data;
using SkillBuilder.Models;
using SkillBuilder.Models.ViewModels;
using System.Security.Claims;

namespace SkillBuilder.Controllers
{
    [Route("Courses")]
    public class CoursesController : Controller
    {
        private readonly AppDbContext _context;

        public CoursesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        public IActionResult CourseCatalog(string? selectedCourse = null, string? search = null)
        {
            ViewData["UseCourseNavbar"] = true;

            var courses = _context.Courses
                .Include(c => c.Artisan)
                .Include(c => c.Enrollments)
                .Include(c => c.Reviews).ThenInclude(r => r.User)
                .Include(c => c.CourseModules).ThenInclude(m => m.Contents)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                courses = courses.Where(c => c.Title.ToLower().Contains(search));
            }

            var courseList = courses.ToList();

            if (!string.IsNullOrEmpty(selectedCourse))
            {
                var course = courseList.FirstOrDefault(c => c.Link == selectedCourse);
                if (course == null)
                    return NotFound();

                ViewData["ShowCourseDetails"] = true;
                return View("CourseCatalog", new CourseCatalogViewModel
                {
                    Courses = courseList,
                    SelectedCourse = course
                });
            }

            ViewData["ShowCourseDetails"] = false;

            return View("CourseCatalog", new CourseCatalogViewModel
            {
                Courses = courseList,
                SelectedCourse = null
            });
        }

        [HttpPost("Enroll")]
        public IActionResult Enroll([FromBody] EnrollRequest request)
        {
            var userId = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var alreadyEnrolled = _context.Enrollments.Any(e => e.UserId == userId && e.CourseId == request.CourseId);
            if (alreadyEnrolled)
                return Json(new { success = false, message = "Already enrolled." });

            var user = _context.Users
                .Include(u => u.Enrollments)
                .FirstOrDefault(u => u.Id == userId);

            if (user == null)
                return NotFound();

            // Track achievement conditions before enrollment
            int previousCount = user.Enrollments?.Count() ?? 0;

            // Add the new enrollment
            _context.Enrollments.Add(new Enrollment
            {
                UserId = userId,
                CourseId = request.CourseId,
                EnrolledAt = DateTime.UtcNow
            });

            _context.SaveChanges();

            // Re-check current count
            int currentCount = _context.Enrollments.Count(e => e.UserId == userId);

            // Determine which achievements were just unlocked
            List<string> achievements = new();

            if (previousCount == 0 && currentCount >= 1)
                achievements.Add("Welcome to Tahi!");

            if (previousCount < 3 && currentCount >= 3)
                achievements.Add("Lifelong Learner");

            return Json(new
            {
                success = true,
                showAchievement = achievements.Any(),
                achievements
            });
        }

        public class EnrollRequest
        {
            public int CourseId { get; set; }
        }

        [HttpGet("CourseModule/{id}")]
        public IActionResult CourseModule(int id)
        {
            var userId = User.FindFirst("UserId")?.Value;
            if (userId == null) return Unauthorized();

            var course = _context.Courses
                .Include(c => c.Artisan)
                .Include(c => c.Enrollments)
                .Include(c => c.Reviews)
                .Include(c => c.Materials)
                .Include(c => c.CourseModules)
                    .ThenInclude(m => m.Contents)
                        .ThenInclude(content => content.QuizQuestions) // ✅ ADD THIS LINE
                .FirstOrDefault(c => c.Id == id);

            if (course == null) return NotFound();

            int totalModules = course.CourseModules
                .SelectMany(m => m.Contents)
                .Count();

            var completedSet = _context.ModuleProgress
                .Where(mp => mp.UserId == userId && mp.CourseModule.CourseId == course.Id && mp.IsCompleted)
                .Select(mp => mp.CourseModuleId)
                .ToHashSet();

            var orderedModules = course.CourseModules
                .OrderBy(cm => cm.Order)
                .ToList();

            int validCompletions = 0;
            for (int i = 0; i < orderedModules.Count; i++)
            {
                if (i == 0 || completedSet.Contains(orderedModules[i - 1].Id))
                {
                    if (completedSet.Contains(orderedModules[i].Id))
                    {
                        validCompletions++;
                    }
                    else break;
                }
                else break;
            }

            double progress = totalModules == 0 ? 0 : (double)validCompletions / totalModules * 100;
            ViewData["CourseProgress"] = Math.Round(progress, 0);

            return View("CourseModules/CourseModule", course);
        }

        [HttpPost("UpdateProgress")]
        public IActionResult UpdateProgress([FromBody] ProgressUpdateModel model)
        {
            var userId = User.FindFirst("UserId")?.Value;
            if (userId == null)
                return Unauthorized();

            foreach (var moduleIndex in model.CompletedModules)
            {
                var courseModule = _context.CourseModules
                    .FirstOrDefault(m => m.CourseId == model.CourseId && m.Order == moduleIndex);

                if (courseModule == null) continue;

                var existing = _context.ModuleProgress
                    .FirstOrDefault(p => p.UserId == userId && p.CourseModuleId == courseModule.Id);

                if (existing == null)
                {
                    _context.ModuleProgress.Add(new ModuleProgress
                    {
                        UserId = userId,
                        CourseModuleId = courseModule.Id,
                        IsCompleted = true,
                        CompletedAt = DateTime.UtcNow,
                        LastUpdated = DateTime.UtcNow
                    });
                }
                else
                {
                    existing.IsCompleted = true;
                    existing.LastUpdated = DateTime.UtcNow;
                }
            }

            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = ex.Message, inner = ex.InnerException?.Message });
            }

            return Ok();
        }

        [HttpPost("ResetProgress")]
        public IActionResult ResetProgress([FromBody] ProgressUpdateModel model)
        {
            var userId = User.FindFirstValue("UserId");

            var toDelete = _context.ModuleProgress
                .Where(p => p.UserId == userId && p.CourseModule.CourseId == model.CourseId)
                .ToList();

            _context.ModuleProgress.RemoveRange(toDelete);
            _context.SaveChanges();

            return Ok(new { message = "Reset successful" });
        }
    }
}