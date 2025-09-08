using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillBuilder.Data;
using SkillBuilder.Models;
using SkillBuilder.Models.ViewModels;
using System.Linq;

namespace SkillBuilder.Controllers
{
    [Route("AdminProfile")]
    public class AdminProfileController : Controller
    {
        private readonly AppDbContext _context;

        public AdminProfileController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public IActionResult AdminProfile(string id)
        {
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
                .Where(u => u.Role != "Admin")
                .ToList();

            var pendingApplications = _context.ArtisanApplications
                .Include(a => a.User)
                .Where(a => a.Status == "Pending")
                .ToList();

            var allCourses = _context.Courses
                .Include(c => c.Artisan)
                .ThenInclude(a => a.User)
                .ToList();

            var submittedCourses = allCourses
                .Where(c => !c.IsPublished)
                .ToList();

            var allCommunities = _context.Communities
                .Include(c => c.Creator)
                .ToList();

            var model = new AdminProfileViewModel
            {
                Admin = admin,
                AllUsers = allUsers,
                Users = allUsers,
                PendingApplications = pendingApplications,
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
                .Include(u => u.Enrollments)
                .Include(u => u.Reviews)
                .Include(u => u.ProjectSubmissions)
                .Include(u => u.Memberships)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return NotFound();

            if (user.Artisan != null)
                _context.Artisans.Remove(user.Artisan);

            _context.Enrollments.RemoveRange(user.Enrollments);
            _context.CourseReviews.RemoveRange(user.Reviews);
            _context.CourseProjectSubmissions.RemoveRange(user.ProjectSubmissions);
            _context.CommunityMemberships.RemoveRange(user.Memberships);

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            // ✅ Correct redirection
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
        public async Task<IActionResult> ToggleUserStatus(string id, string adminId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
                return NotFound();

            // Flip deactivated status
            user.IsDeactivated = !user.IsDeactivated;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            // Redirect back to admin profile
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

            // ✅ Mark application as approved
            application.Status = "Approved";

            // ✅ Find artisan and mark as approved
            var artisan = await _context.Artisans.FirstOrDefaultAsync(a => a.UserId == application.UserId);
            if (artisan != null)
            {
                artisan.IsApproved = true;
            }

            await _context.SaveChangesAsync();

            return RedirectToRoute(new { controller = "AdminProfile", action = "AdminProfile", id = adminId });
        }

        [HttpPost("RejectApplication/{id}")]
        public async Task<IActionResult> RejectApplication(int id, string adminId)
        {
            var application = await _context.ArtisanApplications
                .FirstOrDefaultAsync(a => a.Id == id);

            if (application == null)
                return NotFound();

            // ❌ Just mark application as rejected
            application.Status = "Rejected";

            await _context.SaveChangesAsync();

            return RedirectToRoute(new { controller = "AdminProfile", action = "AdminProfile", id = adminId });
        }

        [HttpPost("DeleteCourse/{id}")]
        public async Task<IActionResult> DeleteCourse(int id, string adminId)
        {
            var course = await _context.Courses
                .Include(c => c.Enrollments)
                .Include(c => c.Reviews)
                .Include(c => c.CourseModules)
                .Include(c => c.Materials)
                .Include(c => c.ProjectSubmissions)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
                return NotFound();

            // ✅ Remove related entities first
            _context.Enrollments.RemoveRange(course.Enrollments);
            _context.CourseReviews.RemoveRange(course.Reviews);
            _context.CourseModules.RemoveRange(course.CourseModules);
            _context.CourseMaterials.RemoveRange(course.Materials);
            _context.CourseProjectSubmissions.RemoveRange(course.ProjectSubmissions);

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            return RedirectToRoute(new { controller = "AdminProfile", action = "AdminProfile", id = adminId });
        }
    }
}