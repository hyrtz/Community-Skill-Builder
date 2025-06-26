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

            var model = new AdminProfileViewModel
            {
                Admin = admin,
                AllUsers = allUsers,
                Users = allUsers,
                PendingApplications = pendingApplications,
                AllPendingApplications = pendingApplications,
                AllSubmittedCourses = allCourses,
                SubmittedCourses = submittedCourses
            };

            return View("~/Views/Profile/AdminProfile.cshtml", model);
        }
    }
}