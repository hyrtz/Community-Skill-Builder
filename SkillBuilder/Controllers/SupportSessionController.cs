using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillBuilder.Data;
using SkillBuilder.Models;
using System.Security.Claims;

namespace SkillBuilder.Controllers
{
    [Route("SupportSession")]
    public class SupportSessionController : Controller
    {
        private readonly AppDbContext _context;

        public SupportSessionController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("CreateRequest")]
        public async Task<IActionResult> CreateRequest([FromBody] SupportSessionRequest data)
        {
            var userId = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            if (!TimeSpan.TryParse(data.SessionTime.ToString(), out var parsedTime))
            {
                ModelState.AddModelError("SessionTime", "Invalid time format.");
                return BadRequest(ModelState);
            }

            var request = new SupportSessionRequest
            {
                UserId = userId,
                CourseId = data.CourseId,
                Title = data.Title,
                Message = data.Message,
                SessionDate = data.SessionDate,
                SessionTime = data.SessionTime,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            _context.SupportSessionRequests.Add(request);

            await _context.SaveChangesAsync();

            return Ok(new { success = true });
        }

        // GET: Artisan’s view of pending support requests
        [HttpGet("PendingRequests")]
        public async Task<IActionResult> PendingRequests()
        {
            var artisanId = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(artisanId))
                return Unauthorized();

            var requests = await _context.SupportSessionRequests
                .Include(r => r.User)
                .Include(r => r.Course)
                .Where(r => r.Course != null &&
                            r.Course.CreatedBy != null &&
                            r.Course.CreatedBy == artisanId &&
                            r.Status == "Pending")
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return View("~/Views/Sections/ArtisanNotebooks/_ArtisanNotebookSupportSessions.cshtml", requests);
        }

        // POST: Confirm a session
        public class ConfirmSessionRequest
        {
            public int Id { get; set; }
            public string Platform { get; set; } = "";
            public string Link { get; set; } = "";
        }

        [HttpPost("Confirm")]
        public async Task<IActionResult> ConfirmSession([FromBody] ConfirmSessionRequest data)
        {
            var request = await _context.SupportSessionRequests.FindAsync(data.Id);
            if (request == null) return NotFound();

            request.Status = "Confirmed";
            request.MeetingPlatform = data.Platform;
            request.MeetingLink = data.Link;
            request.ConfirmedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(new { success = true });
        }

        // POST: Decline a session
        [HttpPost("Decline")]
        public async Task<IActionResult> DeclineSession(int id)
        {
            var request = await _context.SupportSessionRequests.FindAsync(id);
            if (request == null) return NotFound();

            request.Status = "Declined";
            await _context.SaveChangesAsync();
            return Ok(new { success = true });
        }
    }
}