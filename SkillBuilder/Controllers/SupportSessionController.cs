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

        private string? GetUserId() => User.FindFirst("UserId")?.Value;

        // POST: Learner creates a request
        [HttpPost("CreateRequest")]
        public async Task<IActionResult> CreateRequest([FromBody] SupportSessionRequest data)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

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

            // Include User and Course for JS rendering
            var newRequest = await _context.SupportSessionRequests
                .Include(r => r.User)
                .Include(r => r.Course)
                .FirstOrDefaultAsync(r => r.Id == request.Id);

            return Ok(newRequest);
        }

        // GET: Artisan’s view of pending support requests
        [HttpGet("PendingRequests")]
        public async Task<IActionResult> PendingRequests()
        {
            var artisanId = GetUserId();
            if (string.IsNullOrEmpty(artisanId)) return Unauthorized();

            var requests = await _context.SupportSessionRequests
                .Include(r => r.User)
                .Include(r => r.Course)
                .Where(r => r.Course.CreatedBy == artisanId && r.Status == "Pending")
                .OrderByDescending(r => r.CreatedAt)
                .AsNoTracking()
                .ToListAsync();

            return View("~/Views/Sections/ArtisanNotebooks/_ArtisanNotebookSupportSessions.cshtml", requests);
        }

        [HttpGet("PendingRequestsPartial")]
        public async Task<IActionResult> PendingRequestsPartial()
        {
            try
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
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        public class ConfirmSessionRequest
        {
            public int Id { get; set; }
            public string Platform { get; set; } = "";
            public string Link { get; set; } = "";
        }

        [HttpPost("Confirm")]
        public async Task<IActionResult> ConfirmSession([FromBody] ConfirmSessionRequest data)
        {
            var artisanId = GetUserId();
            if (string.IsNullOrEmpty(artisanId)) return Unauthorized();

            var request = await _context.SupportSessionRequests
                .Include(r => r.User)
                .Include(r => r.Course)
                .FirstOrDefaultAsync(r => r.Id == data.Id && r.Course.CreatedBy == artisanId);

            if (request == null) return NotFound();

            request.Status = "Confirmed";
            request.MeetingPlatform = data.Platform;
            request.MeetingLink = data.Link;
            request.ConfirmedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Return the full updated object
            return Json(request);
        }

        public class DeclineSessionRequest
        {
            public int Id { get; set; }
        }

        [HttpPost("Decline")]
        public async Task<IActionResult> DeclineSession([FromBody] DeclineSessionRequest data)
        {
            var artisanId = GetUserId();
            if (string.IsNullOrEmpty(artisanId)) return Unauthorized();

            var request = await _context.SupportSessionRequests
                .Include(r => r.Course)
                .Include(r => r.User) // include User for JS
                .FirstOrDefaultAsync(r => r.Id == data.Id && r.Course.CreatedBy == artisanId);

            if (request == null) return NotFound();

            request.Status = "Declined";
            await _context.SaveChangesAsync();

            // return the updated request
            return Ok(new
            {
                Id = request.Id,
                Title = request.Title,
                Message = request.Message,
                User = new
                {
                    UserAvatar = request.User?.UserAvatar,
                    FirstName = request.User?.FirstName,
                    LastName = request.User?.LastName
                },
                Course = new
                {
                    Title = request.Course?.Title
                }
            });
        }

        public class CompleteSessionRequest
        {
            public int Id { get; set; }
        }

        [HttpPost("Complete")]
        public async Task<IActionResult> CompleteSession([FromBody] CompleteSessionRequest data)
        {
            var artisanId = GetUserId();
            if (string.IsNullOrEmpty(artisanId)) return Unauthorized();

            var request = await _context.SupportSessionRequests
                .Include(r => r.Course)
                .FirstOrDefaultAsync(r => r.Id == data.Id && r.Course.CreatedBy == artisanId);

            if (request == null) return NotFound();

            request.Status = "Completed";
            request.CompletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(new { success = true });
        }

        [HttpGet("CompletedRequestsPartial")]
        public async Task<IActionResult> CompletedRequestsPartial()
        {
            var artisanId = GetUserId();
            if (string.IsNullOrEmpty(artisanId)) return Unauthorized();

            var requests = await _context.SupportSessionRequests
                .Include(r => r.User)
                .Include(r => r.Course)
                .Where(r => r.Course != null && r.Course.CreatedBy == artisanId && r.Status == "Completed")
                .OrderByDescending(r => r.CompletedAt)
                .AsNoTracking()
                .ToListAsync();

            return PartialView("~/Views/Shared/Sections/ArtisanNotebooks/SupportSessionsNotebooks/_SupportSessionsNotebookCompleted.cshtml", requests);
        }


        [HttpPost("Complete/{id}")]
        public async Task<IActionResult> CompleteSession(int id)
        {
            var artisanId = GetUserId();
            if (string.IsNullOrEmpty(artisanId)) return Unauthorized();

            var request = await _context.SupportSessionRequests
                .Include(r => r.Course)
                .FirstOrDefaultAsync(r => r.Id == id && r.Course.CreatedBy == artisanId);

            if (request == null) return NotFound();

            request.Status = "Completed";
            request.CompletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(new { success = true });
        }

        [HttpPost("Revert/{id}")]
        public async Task<IActionResult> RevertSession(int id)
        {
            var artisanId = GetUserId();
            if (string.IsNullOrEmpty(artisanId)) return Unauthorized();

            var request = await _context.SupportSessionRequests
                .Include(r => r.Course)
                .FirstOrDefaultAsync(r => r.Id == id && r.Course.CreatedBy == artisanId);

            if (request == null) return NotFound();

            request.Status = "Confirmed";
            request.CompletedAt = null;

            await _context.SaveChangesAsync();
            return Ok(new { success = true });
        }

        [HttpGet("DeclinedPartial")]
        public IActionResult DeclinedPartial()
        {
            var artisanId = GetUserId();
            if (string.IsNullOrEmpty(artisanId)) return Unauthorized();

            var declinedSessions = _context.SupportSessionRequests
                .Include(s => s.User)
                .Include(s => s.Course)
                .Where(s => s.Course != null &&
                            s.Course.CreatedBy == artisanId &&
                            s.Status == "Declined")
                .OrderByDescending(s => s.SessionDate)
                .ToList();

            return PartialView("~/Views/Shared/Sections/ArtisanNotebooks/SupportSessionsNotebooks/_SupportSessionsNotebookDeclined.cshtml", declinedSessions);
        }

        [HttpGet("UpcomingPartial")]
        public IActionResult UpcomingPartial()
        {
            var artisanId = GetUserId();
            if (string.IsNullOrEmpty(artisanId)) return Unauthorized();

            var upcomingSessions = _context.SupportSessionRequests
                .Include(s => s.User)
                .Include(s => s.Course)
                .Where(s => s.Course != null &&
                            s.Course.CreatedBy == artisanId &&
                            s.Status == "Confirmed" &&
                            s.SessionDate >= DateTime.Today)
                .OrderBy(s => s.SessionDate)
                .ToList();

            return PartialView("~/Views/Shared/Sections/ArtisanNotebooks/SupportSessionsNotebooks/_SupportSessionsNotebookUpcoming.cshtml", upcomingSessions);
        }
    }
}