//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using SkillBuilder.Data;
//using SkillBuilder.Models;
//using SkillBuilder.Services;

//namespace SkillBuilder.Controllers
//{
//    [Route("ArtisanSupport")]
//    public class ArtisanSupportController : Controller
//    {
//        private readonly AppDbContext _context;
//        private readonly INotificationService _notificationService;
//        public ArtisanSupportController(AppDbContext context, INotificationService notificationService)
//        {
//            _context = context;
//            _notificationService = notificationService;
//        }

//        // Get logged-in artisan's UserId from claims (as string)
//        private string? GetUserId()
//        {
//            return User.FindFirst("UserId")?.Value;
//        }

//        // === Pending Requests Partial ===
//        [HttpGet("PendingRequestsPartial")]
//        public async Task<IActionResult> PendingRequestsPartial()
//        {
//            var artisanId = GetUserId();
//            if (string.IsNullOrEmpty(artisanId)) return Unauthorized();

//            var requests = await _context.SupportSessionRequests
//                .Include(r => r.User)
//                .Include(r => r.Course)
//                .Where(r => r.Course != null &&
//                            r.Course.CreatedBy == artisanId &&
//                            r.Status == "Pending")
//                .OrderByDescending(r => r.CreatedAt)
//                .AsNoTracking()
//                .ToListAsync();

//            return PartialView("~/Views/Shared/Sections/ArtisanNotebooks/SupportSessionsNotebooks/_SupportSessionsNotebookPending.cshtml", requests);
//        }

//        // === Confirm Session ===
//        public class ConfirmSessionRequest
//        {
//            public int SessionId { get; set; }
//            public string Platform { get; set; } = "";
//            public string Link { get; set; } = "";
//        }

//        [HttpPost("Confirm")]
//        public async Task<IActionResult> Confirm([FromBody] ConfirmSessionRequest data)
//        {
//            var artisanIdStr = GetUserId();
//            if (artisanIdStr == null) return Unauthorized();

//            var request = await _context.SupportSessionRequests
//                .Include(r => r.User)
//                .Include(r => r.Course)
//                .FirstOrDefaultAsync(r => r.Id == data.SessionId &&
//                                          r.Course != null &&
//                                          r.Course.CreatedBy == artisanIdStr);

//            if (request == null) return NotFound();

//            request.Status = "Confirmed";
//            request.MeetingPlatform = data.Platform;
//            request.MeetingLink = data.Link;
//            request.ConfirmedAt = DateTime.UtcNow;

//            await _context.SaveChangesAsync();

//            // --- Notification for the requesting user ---
//            if (request.User != null)
//            {
//                await _notificationService.AddNotificationAsync(
//                    request.User.Id,
//                    $"✅ Your support session for '{request.Course?.Title}' has been confirmed by the artisan."
//                );
//            }

//            // --- Notification for the artisan ---
//            await _notificationService.AddNotificationAsync(
//                artisanIdStr,
//                $"✏️ You have confirmed the support session for '{request.Course?.Title}'."
//            );

//            return Json(request);
//        }

//        // === Decline Session ===
//        public class DeclineSessionRequest { 
//            public int SessionId { get; set; }
//            public string? Reason { get; set; }
//        }

//        [HttpPost("Decline")]
//        public async Task<IActionResult> Decline([FromBody] DeclineSessionRequest data)
//        {
//            var artisanIdStr = GetUserId();
//            if (artisanIdStr == null) return Unauthorized();

//            var request = await _context.SupportSessionRequests
//                .Include(r => r.User)
//                .Include(r => r.Course)
//                .FirstOrDefaultAsync(r => r.Id == data.SessionId &&
//                                          r.Course != null &&
//                                          r.Course.CreatedBy == artisanIdStr);

//            if (request == null) return NotFound();

//            // Update session status
//            request.Status = "Declined";
//            await _context.SaveChangesAsync();

//            // --- Notify the requesting user with reschedule button ---
//            if (request.User != null)
//            {
//                var message = $"❌ Your support session for '{request.Course?.Title}' has been declined by the artisan.";
//                if (!string.IsNullOrWhiteSpace(data.Reason))
//                    message += $" Reason: {data.Reason}";

//                await AddNotificationAsync(
//                    request.User.Id,
//                    message,
//                    "Reschedule Session",
//                    $"/Support/RequestSession/{request.Course?.Id}"
//                );
//            }

//            // --- Notify the artisan ---
//            await _notificationService.AddNotificationAsync(
//                artisanIdStr,
//                $"✏️ You have declined the support session for '{request.Course?.Title}'."
//            );

//            return Ok(new { request.Id });
//        }

//        private async Task AddNotificationAsync(string userId, string message, string? actionText = null, string? actionUrl = null)
//        {
//            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(message))
//                return;

//            var notification = new Notification
//            {
//                UserId = userId,
//                Message = message,
//                ActionText = actionText,
//                ActionUrl = actionUrl,
//                CreatedAt = DateTime.UtcNow,
//                IsRead = false
//            };

//            _context.Notifications.Add(notification);
//            await _context.SaveChangesAsync();
//        }

//    }
//}