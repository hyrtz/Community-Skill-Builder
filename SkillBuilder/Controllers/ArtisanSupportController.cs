using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillBuilder.Data;
using SkillBuilder.Models;

namespace SkillBuilder.Controllers
{
    [Route("ArtisanSupport")]
    public class ArtisanSupportController : Controller
    {
        private readonly AppDbContext _context;
        public ArtisanSupportController(AppDbContext context) => _context = context;

        // Get logged-in artisan's UserId from claims (as string)
        private string? GetUserId()
        {
            return User.FindFirst("UserId")?.Value;
        }

        // === Pending Requests Partial ===
        [HttpGet("PendingRequestsPartial")]
        public async Task<IActionResult> PendingRequestsPartial()
        {
            var artisanId = GetUserId();
            if (string.IsNullOrEmpty(artisanId)) return Unauthorized();

            var requests = await _context.SupportSessionRequests
                .Include(r => r.User)
                .Include(r => r.Course)
                .Where(r => r.Course != null &&
                            r.Course.CreatedBy == artisanId &&
                            r.Status == "Pending")
                .OrderByDescending(r => r.CreatedAt)
                .AsNoTracking()
                .ToListAsync();

            return PartialView("~/Views/Shared/Sections/ArtisanNotebooks/SupportSessionsNotebooks/_SupportSessionsNotebookPending.cshtml", requests);
        }

        // === Confirm Session ===
        public class ConfirmSessionRequest
        {
            public int SessionId { get; set; }
            public string Platform { get; set; } = "";
            public string Link { get; set; } = "";
        }

        [HttpPost("Confirm")]
        public async Task<IActionResult> Confirm([FromBody] ConfirmSessionRequest data)
        {
            var artisanIdStr = GetUserId();
            if (artisanIdStr == null) return Unauthorized();

            var request = await _context.SupportSessionRequests
                .Include(r => r.User)
                .Include(r => r.Course)
                .FirstOrDefaultAsync(r => r.Id == data.SessionId &&
                                          r.Course != null &&
                                          r.Course.CreatedBy == artisanIdStr);

            if (request == null) return NotFound();

            request.Status = "Confirmed";
            request.MeetingPlatform = data.Platform;
            request.MeetingLink = data.Link;
            request.ConfirmedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Json(request);
        }

        // === Decline Session ===
        public class DeclineSessionRequest { public int SessionId { get; set; } }

        [HttpPost("Decline")]
        public async Task<IActionResult> Decline([FromBody] DeclineSessionRequest data)
        {
            var artisanIdStr = GetUserId();
            if (artisanIdStr == null) return Unauthorized();

            var request = await _context.SupportSessionRequests
                .Include(r => r.User)
                .Include(r => r.Course)
                .FirstOrDefaultAsync(r => r.Id == data.SessionId &&
                                          r.Course != null &&
                                          r.Course.CreatedBy == artisanIdStr);

            if (request == null) return NotFound();

            request.Status = "Declined";
            await _context.SaveChangesAsync();
            return Ok(new { request.Id });
        }
    }
}