using SkillBuilder.Data;
using SkillBuilder.Models.Analytics;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace SkillBuilder.Services
{
    public interface ICommunityAnalyticsService
    {
        Task<CommunityAnalyticsDto> GetAnalyticsAsync(string range);
    }

    public class CommunityAnalyticsService : ICommunityAnalyticsService
    {
        private readonly AppDbContext _db;

        public CommunityAnalyticsService(AppDbContext db)
        {
            _db = db;
        }

        public static class DateRangeHelper
        {
            public static (DateTime start, DateTime end) GetRange(string range)
            {
                var now = DateTime.UtcNow.Date;

                return range switch
                {
                    "today" => (now, now.AddDays(1)),
                    "yesterday" => (now.AddDays(-1), now),
                    "lastweek" => (now.AddDays(-7), now),
                    "lastmonth" => (now.AddMonths(-1), now),
                    "lastyear" => (now.AddYears(-1), now),
                    _ => (now.AddMonths(-1), now) // default
                };
            }
        }

        public async Task<CommunityAnalyticsDto> GetAnalyticsAsync(string range)
        {
            var (start, end) = DateRangeHelper.GetRange(range);

            var dto = new CommunityAnalyticsDto();

            dto.TotalCommunities = await _db.Communities.CountAsync(c => !c.IsArchived);
            dto.TotalPosts = await _db.CommunityPosts.CountAsync(p => p.IsPublished);
            dto.TotalMembers = await _db.CommunityMemberships.CountAsync();

            dto.MembersWithPosts = await _db.CommunityPosts
                .Select(p => p.AuthorId)
                .Distinct()
                .CountAsync();

            dto.MembersWithoutPosts = dto.TotalMembers - dto.MembersWithPosts;

            dto.FlaggedPostsCount = await _db.CommunityPostReports.CountAsync();

            dto.TopCommunities = await _db.Communities
                .Where(c => !c.IsArchived)
                .Select(c => new TopCommunityDto
                {
                    CommunityId = c.Id,
                    Name = c.Name,
                    MembersCount = c.Memberships.Count,
                    TotalPosts = c.Posts.Count
                })
                .OrderByDescending(c => c.MembersCount)
                .Take(5)
                .ToListAsync();

            dto.FlaggedPosts = await _db.CommunityPostReports
                .Include(r => r.Post)
                .Include(r => r.Post.Author)
                .Select(r => new FlaggedPostDto
                {
                    PostId = r.PostId,
                    TitleOrSnippet = !string.IsNullOrEmpty(r.Post.Title)
                        ? r.Post.Title
                        : r.Post.Content.Length > 40 ? r.Post.Content.Substring(0, 40) + "..." : r.Post.Content,
                    ReporterName = _db.Users
                        .Where(u => u.Id == r.ReporterId)
                        .Select(u => u.FirstName + " " + u.LastName)
                        .FirstOrDefault() ?? "Unknown",
                    Reason = r.Reason,
                    ReportedAt = r.ReportedAt
                })
                .OrderByDescending(r => r.ReportedAt)
                .ToListAsync();

            return dto;
        }
    }
}
