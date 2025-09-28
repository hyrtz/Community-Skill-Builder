using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillBuilder.Data;
using SkillBuilder.Models;
using SkillBuilder.Services;
using System.Security.Claims;

namespace SkillBuilder.Controllers
{
    [Route("CourseProject")]
    public class CourseProjectController : Controller
    {
        private readonly AppDbContext _context;
        private readonly INotificationService _notificationService;

        public CourseProjectController(AppDbContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        public class ApproveRequest
        {
            public int Id { get; set; }
        }

        [HttpPost("Approve")]
        public async Task<IActionResult> Approve([FromBody] ApproveRequest data)
        {
            var project = await _context.CourseProjectSubmissions
                .Include(p => p.User)
                .Include(p => p.Course)
                .FirstOrDefaultAsync(p => p.Id == data.Id);

            if (project == null)
                return NotFound();

            // Update project status
            project.Status = "Approved";
            await _context.SaveChangesAsync();

            // Notification for learner (project owner)
            await AddNotificationAsync(
                project.UserId,
                $"✅ Your project **{project.Title}** for course **{project.Course?.Title}** has been approved!"
            );

            // Notification for artisan who approved
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await AddNotificationAsync(
                currentUserId,
                $"📢 You approved project **{project.Title}** for course **{project.Course?.Title}**."
            );

            return Ok(new
            {
                id = project.Id,
                project.Status,
                project.Title,
                project.Description,
                courseTitle = project.Course?.Title,
                project.MediaUrl
            });
        }

        public class RejectRequest
        {
            public int Id { get; set; }
            public string Reason { get; set; } = "";
        }

        [HttpPost("Reject")]
        public async Task<IActionResult> Reject([FromBody] RejectRequest data)
        {
            var project = await _context.CourseProjectSubmissions
                .Include(p => p.User)
                .Include(p => p.Course)
                .FirstOrDefaultAsync(p => p.Id == data.Id);

            if (project == null)
                return NotFound();

            // Update project status
            project.Status = "Rejected";
            await _context.SaveChangesAsync();

            // Notification for learner (project owner) with Resubmit button
            var message = $"❌ Your project **{project.Title}** for course **{project.Course?.Title}** has been rejected.";
            if (!string.IsNullOrWhiteSpace(data.Reason))
                message += $" Reason: {data.Reason}";

            await AddNotificationAsync(
                project.UserId,
                message,
                "Resubmit Project",                         // actionText
                $"/UserProfile/ResubmitFinalProject/{project.Id}" // actionUrl
            );

            // Notification for artisan who rejected
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await AddNotificationAsync(
                currentUserId,
                $"📢 You rejected project **{project.Title}** for course **{project.Course?.Title}**. Reason: {data.Reason}"
            );

            return Ok(new
            {
                id = project.Id,
                project.Status,
                project.Title,
                project.Description,
                courseTitle = project.Course?.Title,
                project.MediaUrl
            });
        }

        // Helper to add notification (like in AdminProfileController)
        private async Task AddNotificationAsync(string userId, string message, string? actionText = null, string? actionUrl = null)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(message))
                return;

            var notification = new Notification
            {
                UserId = userId,
                Message = message,
                ActionText = actionText,
                ActionUrl = actionUrl,
                CreatedAt = DateTime.UtcNow,
                IsRead = false // ensures it counts as unread
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
        }
    }
}