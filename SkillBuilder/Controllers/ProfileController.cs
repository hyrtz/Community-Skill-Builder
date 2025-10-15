using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillBuilder.Data;
using SkillBuilder.Models;
using SkillBuilder.Models.ViewModels;
using SkillBuilder.Services;
using System.Security.Claims;

namespace SkillBuilder.Controllers
{
    public class ProfileController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly INotificationService _notificationService;

        public ProfileController(AppDbContext dbContext, INotificationService notificationService)
        {
            _dbContext = dbContext;
            _notificationService = notificationService;
        }

        [HttpGet]
        public IActionResult UserViewByAdmin(string id)
        {
            // No model passed; just for admin view
            ViewData["UserId"] = id;
            return View("~/Views/Profile/UserViewByAdmin.cshtml");
        }

        [HttpGet]
        public IActionResult UserViewAsLearner(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return BadRequest();

            // Fetch the user with their enrollments and courses
            var user = _dbContext.Users
                                 .Include(u => u.Enrollments)
                                 .ThenInclude(e => e.Course)
                                 .FirstOrDefault(u => u.Id == userId);

            if (user == null)
                return NotFound();

            var enrolledCourses = user.Enrollments?.Select(e => e.Course).ToList() ?? new List<Course>();

            var viewModel = new UserProfileViewModel
            {
                User = user,
                EnrolledCourses = enrolledCourses,
                // You can still access points and deactivated status through user
            };

            return View("~/Views/Profile/UserViewAsLearner.cshtml", viewModel);
        }
        [HttpPost("Profile/ReportUser/{userId}")]
        public async Task<IActionResult> ReportUserAvatar(string userId)
        {
            var reporterId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(reporterId))
                return Unauthorized();

            var reportedUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (reportedUser == null)
                return NotFound(new { success = false, message = "User not found." });

            // 📝 Save report record
            var report = new UserReport
            {
                ReporterId = reporterId,
                ReportedUserId = userId,
                ReportType = "Avatar",
                ReportedAt = DateTime.UtcNow
            };
            _dbContext.UserReports.Add(report);
            await _dbContext.SaveChangesAsync();

            // 🔔 Notify reporting user (confirmation)
            await _notificationService.AddNotificationAsync(
                reporterId,
                $"⚠️ You successfully reported {reportedUser.FirstName} {reportedUser.LastName}'s avatar for review."
            );

            // 🔔 Notify all admins
            var adminIds = await _dbContext.Users
                .Where(u => u.Role == "Admin")
                .Select(u => u.Id)
                .ToListAsync();

            foreach (var adminId in adminIds)
            {
                await _notificationService.AddNotificationAsync(
                    adminId,
                    $"⚠️ User \"{reportedUser.FirstName} {reportedUser.LastName}\"'s avatar has been reported for review."
                );
            }

            return Ok(new { success = true, message = "User avatar reported successfully." });
        }
    }
}