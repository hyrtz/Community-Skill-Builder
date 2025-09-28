using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillBuilder.Data;
using SkillBuilder.Models;

namespace SkillBuilder.Controllers
{
    [Route("api/[controller]")]
    public class NotificationController : Controller
    {
        private readonly AppDbContext _context;

        public NotificationController(AppDbContext context)
        {
            _context = context;
        }

        // Get last 5 notifications for a user
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetNotifications(string userId, [FromQuery] bool all = false)
        {
            var query = _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt);

            if (!all)
            {
                var top5 = await query.Take(5)
                    .Select(n => new
                    {
                        id = n.Id,
                        message = n.Message,
                        actionUrl = n.ActionUrl,
                        actionText = n.ActionText,
                        createdAt = n.CreatedAt,
                        isRead = n.IsRead,
                        isActioned = n.IsActioned
                    })
                    .ToListAsync();

                return Ok(top5);
            }

            var allNotifs = await query
                .Select(n => new
                {
                    id = n.Id,
                    message = n.Message,
                    actionUrl = n.ActionUrl,
                    actionText = n.ActionText,
                    createdAt = n.CreatedAt,
                    isRead = n.IsRead,
                    isActioned = n.IsActioned
                })
                .ToListAsync();

            return Ok(allNotifs);
        }

        // Mark a single notification as read
        [HttpPost("mark-read/{id:int}")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var notif = await _context.Notifications.FirstOrDefaultAsync(n => n.Id == id);
            if (notif == null) return NotFound();

            notif.IsRead = true;
            await _context.SaveChangesAsync();
            return Ok();
        }

        // Mark ALL notifications as read for a user
        [HttpPost("mark-all-read/{userId}")]
        public async Task<IActionResult> MarkAllAsRead(string userId)
        {
            var notifs = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            if (notifs.Count == 0) return Ok();

            foreach (var notif in notifs)
                notif.IsRead = true;

            await _context.SaveChangesAsync();
            return Ok();
        }

        // Mark a notification as actioned (button clicked)
        [HttpPost("mark-actioned/{id:int}")]
        public async Task<IActionResult> MarkAsActioned(int id)
        {
            var notif = await _context.Notifications.FirstOrDefaultAsync(n => n.Id == id);
            if (notif == null) return NotFound();

            notif.IsActioned = true;
            await _context.SaveChangesAsync();
            return Ok(new { success = true });
        }

        // Get unread count
        [HttpGet("unread-count/{userId}")]
        public async Task<IActionResult> GetUnreadCount(string userId)
        {
            var count = await _context.Notifications
                .CountAsync(n => n.UserId == userId && !n.IsRead);
            return Ok(count);
        }

        // Render the notification modal partial
        [HttpGet("/Notification/Modal")]
        public IActionResult Modal()
        {
            return PartialView("~/Views/Shared/Sections/_NotificationModal.cshtml");
        }
    }
}