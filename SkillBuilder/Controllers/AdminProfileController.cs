using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillBuilder.Data;
using SkillBuilder.Models;
using SkillBuilder.Models.ViewModels;
using SkillBuilder.Services;
using System.Linq;

namespace SkillBuilder.Controllers
{
    [Authorize(AuthenticationSchemes = "TahiAuth", Roles = "Admin")]
    [Route("AdminProfile")]
    public class AdminProfileController : Controller
    {
        private readonly AppDbContext _context;
        private readonly INotificationService _notificationService;
        public AdminProfileController(AppDbContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        [HttpGet("{id}")]
        public IActionResult AdminProfile(string id)
        {
            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (id != currentUserId)
            {
                return Forbid();
            }

            if (string.IsNullOrEmpty(id))
                return NotFound();

            var adminUser = _context.Users.FirstOrDefault(u => u.Id == id && u.Role == "Admin");
            if (adminUser == null)
                return NotFound();

            var admin = _context.Admins
                .Include(a => a.User)
                .FirstOrDefault(a => a.UserId == adminUser.Id);
            if (admin == null)
                return NotFound();

            var allUsers = _context.Users
                .IgnoreQueryFilters()
                .Where(u => u.Role != "Admin")
                .ToList();

            var pendingApplications = _context.ArtisanApplications
                .Include(a => a.User)
                .Where(a => a.Status == "Pending")
                .ToList();

            var allCourses = _context.Courses
                .IgnoreQueryFilters()
                .Include(c => c.Artisan)
                .ThenInclude(a => a.User)
                .ToList();

            var submittedCourses = allCourses
                .Where(c => !c.IsPublished)
                .ToList();

            var allCommunities = _context.Communities
                .IgnoreQueryFilters()
                .Include(c => c.Creator)
                .ToList();

            var model = new AdminProfileViewModel
            {
                Admin = admin,
                AllUsers = allUsers,
                Users = allUsers,
                PendingApplications = pendingApplications,
                AllApprovedApplications = _context.ArtisanApplications
                                  .Where(a => a.Status == "Approved")
                                  .ToList(),
                AllRejectedApplications = _context.ArtisanApplications
                                  .Where(a => a.Status == "Rejected")
                                  .ToList(),
                AllPendingApplications = pendingApplications,
                AllSubmittedCourses = allCourses,
                SubmittedCourses = submittedCourses,
                AllCommunities = allCommunities,
                Communities = allCommunities
            };

            return View("~/Views/Profile/AdminProfile.cshtml", model);
        }

        [HttpGet("User/{id}")]
        public IActionResult UserProfileByAdmin(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            // Get the user
            var user = _context.Users.FirstOrDefault(u => u.Id == id && u.Role != "Admin");
            if (user == null)
                return NotFound();

            // Build the same kind of model your UserProfile uses
            var enrolledCourses = _context.Enrollments
                .Include(e => e.Course)
                .Where(e => e.UserId == id)
                .Select(e => e.Course)
                .ToList();

            var submittedProjects = _context.CourseProjectSubmissions
                .Where(p => p.UserId == id)
                .ToList();

            var model = new UserProfileViewModel
            {
                User = user,
                EnrolledCourses = enrolledCourses,
                SubmittedProjects = submittedProjects
                // Add other data you normally show in UserProfile
            };

            // Render the new admin-specific view
            return View("~/Views/Profile/UserViewByAdmin.cshtml", model);
        }

        [HttpPost("DeleteUser/{id}")]
        public async Task<IActionResult> DeleteUserAsync(string id, string adminId)
        {
            var user = await _context.Users
                .Include(u => u.Artisan)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return NotFound();

            // Archive Artisan profile if exists
            if (user.Artisan != null)
                user.Artisan.IsArchived = true;

            // Archive user
            user.IsArchived = true;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            // Send notification to admin
            await AddNotificationAsync(
                adminId,
                $"You Deleted the user {user.FirstName} {user.LastName}."
            );

            return RedirectToRoute(new { controller = "AdminProfile", action = "AdminProfile", id = adminId });
        }

        [HttpGet("Application/{id}")]
        public IActionResult ArtisanApplicationViewByAdmin(int id)
        {
            var application = _context.ArtisanApplications
                .Include(a => a.User)
                .FirstOrDefault(a => a.Id == id);

            if (application == null)
                return NotFound();

            return View("~/Views/Profile/ArtisanApplicationViewByAdmin.cshtml", application);
        }

        [HttpGet("CommunityDetails/{id}")]
        public IActionResult CommunityDetails(int id)
        {
            var community = _context.Communities
                .Include(c => c.Creator)
                .FirstOrDefault(c => c.Id == id);

            if (community == null)
                return NotFound();

            // Assuming you want a view showing details and courses of this community
            return View("~/Views/Shared/Sections/_CommunityDetailsSection.cshtml", community);
        }

        [HttpPost("ToggleUserStatus/{id}")]
        public async Task<IActionResult> ToggleUserStatus(string id, string adminId, string? reason)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
                return NotFound();

            // Toggle status
            user.IsDeactivated = !user.IsDeactivated;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            var action = user.IsDeactivated ? "deactivated" : "reactivated";

            // 🔔 Notify user
            var userMessage = user.IsDeactivated
                ? $"Your account has been deactivated by an administrator. {(!string.IsNullOrWhiteSpace(reason) ? "Reason: " + reason : "")}"
                : "Your account has been reactivated by an administrator.";

            await _notificationService.AddNotificationAsync(user.Id, userMessage);

            // 🔔 Notify admin
            await _notificationService.AddNotificationAsync(
                adminId,
                $"You have {action} the account of {user.FirstName} {user.LastName}."
            );

            return RedirectToRoute(new { controller = "AdminProfile", action = "AdminProfile", id = adminId });
        }

        [HttpPost("ApproveApplication/{id}")]
        public async Task<IActionResult> ApproveApplication(int id, string adminId)
        {
            var application = await _context.ArtisanApplications
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (application == null)
                return NotFound();

            if (application.Status != "Pending")
                return BadRequest("This application has already been reviewed.");

            // ✅ Mark application as approved
            application.Status = "Approved";

            // ✅ Find artisan and mark as approved
            var artisan = await _context.Artisans.FirstOrDefaultAsync(a => a.UserId == application.UserId);
            if (artisan != null)
            {
                artisan.IsApproved = true;
                _context.Artisans.Update(artisan);
            }

            await _context.SaveChangesAsync();

            // 🔔 Notify applicant
            await AddNotificationAsync(
                application.UserId,
                "🎉 Congratulations! Your artisan application has been approved. You can now publish your courses."
            );

            // 🔔 Notify admin
            await AddNotificationAsync(
                adminId,
                $"You approved the artisan application of {application.User?.FirstName} {application.User?.LastName}."
            );

            return RedirectToRoute(new { controller = "AdminProfile", action = "AdminProfile", id = adminId });
        }
        public async Task AddNotificationAsync(string userId, string message, string? actionText = null, string? actionUrl = null)
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
                IsRead = false
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
        }

        [HttpPost("RejectApplication/{id}")]
        public async Task<IActionResult> RejectApplication(int id, string adminId, string? reason)
        {
            var application = await _context.ArtisanApplications
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (application == null)
                return NotFound();

            if (application.Status != "Pending")
                return BadRequest("This application has already been reviewed.");

            // ❌ Mark application as rejected
            application.Status = "Rejected";
            await _context.SaveChangesAsync();

            // 🔔 Notify applicant with "resubmit" button
            var message = $"❌ Unfortunately, your artisan application has been rejected.";
            if (!string.IsNullOrWhiteSpace(reason))
                message += $" Reason: {reason}";

            await AddNotificationAsync(
                application.UserId,
                message,
                "Resubmit Application",
                $"/Artisan/Resubmit/{application.Id}"
            );

            // 🔔 Notify admin
            await _notificationService.AddNotificationAsync(
                adminId,
                $"You rejected the artisan application of {application.User?.FirstName} {application.User?.LastName}."
            );

            return RedirectToRoute(new { controller = "AdminProfile", action = "AdminProfile", id = adminId });
        }

        [HttpPost("DeleteCourse/{id}")]
        public async Task<IActionResult> DeleteCourse(int id, string adminId, string reason)
        {
            var course = await _context.Courses
                .Include(c => c.Artisan)
                    .ThenInclude(a => a.User)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
                return NotFound();

            course.IsArchived = true;
            _context.Courses.Update(course);
            await _context.SaveChangesAsync();

            // Notify creator with reason
            if (course.Artisan?.User != null)
            {
                await AddNotificationAsync(
                    course.Artisan.User.Id,
                    $"⚠️ Your course '{course.Title}' has been deleted by the admin. Reason: {reason}"
                );
            }

            // Notify admin
            await AddNotificationAsync(
                adminId,
                $"You have deleted the course '{course.Title}'."
            );

            return RedirectToRoute(new { controller = "AdminProfile", action = "AdminProfile", id = adminId });
        }

        [HttpPost("DeleteCommunity/{id}")]
        public async Task<IActionResult> DeleteCommunity(int id, string adminId, string reason)
        {
            var community = await _context.Communities
                    .Include(c => c.Creator)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (community == null)
                return NotFound();

            community.IsArchived = true;
            _context.Communities.Update(community);
            await _context.SaveChangesAsync();

            if (community.Creator != null)
            {
                await AddNotificationAsync(
                    community.Creator.Id,
                    $"⚠️ Your community '{community.Name}' has been deleted by the admin. Reason: {reason}"
                );
            }

            await AddNotificationAsync(
                adminId,
                $"You have deleted the community '{community.Name}'."
            );

            return RedirectToRoute(new { controller = "AdminProfile", action = "AdminProfile", id = adminId });
        }
    }
}