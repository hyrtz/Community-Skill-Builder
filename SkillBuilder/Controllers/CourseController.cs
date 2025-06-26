using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillBuilder.Data;
using SkillBuilder.Models;
using SkillBuilder.Models.ViewModels;

namespace SkillBuilder.Controllers
{
    [Route("Courses")]
    public class CoursesController : Controller
    {
        private readonly AppDbContext _context;

        public CoursesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        public IActionResult CourseCatalog(string? selectedCourse = null)
        {
            ViewData["UseCourseNavbar"] = true;

            var courses = _context.Courses
                .Include(c => c.Artisan) 
                .Include(c => c.Enrollments) 
                .Include(c => c.Reviews).ThenInclude(r => r.User)
                .ToList();

            if (!string.IsNullOrEmpty(selectedCourse))
            {
                var course = courses.FirstOrDefault(c => c.Link == selectedCourse);
                if (course == null)
                    return NotFound();

                ViewData["ShowCourseDetails"] = true;
                return View("CourseCatalog", new CourseCatalogViewModel
                {
                    Courses = courses,
                    SelectedCourse = course
                });
            }

            ViewData["ShowCourseDetails"] = false;

            return View("CourseCatalog", new CourseCatalogViewModel
            {
                Courses = courses,
                SelectedCourse = null
            });
        }

        [HttpPost("Enroll")]
        public IActionResult Enroll([FromBody] EnrollRequest request)
        {
            var userId = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var alreadyEnrolled = _context.Enrollments.Any(e => e.UserId == userId && e.CourseId == request.CourseId);
            if (alreadyEnrolled)
                return Json(new { success = false, message = "Already enrolled." });

            var user = _context.Users
                .Include(u => u.Enrollments)
                .FirstOrDefault(u => u.Id == userId);

            if (user == null)
                return NotFound();

            // Track achievement conditions before enrollment
            int previousCount = user.Enrollments?.Count() ?? 0;

            // Add the new enrollment
            _context.Enrollments.Add(new Enrollment
            {
                UserId = userId,
                CourseId = request.CourseId,
                EnrolledAt = DateTime.UtcNow
            });

            _context.SaveChanges();

            // Re-check current count
            int currentCount = _context.Enrollments.Count(e => e.UserId == userId);

            // Determine which achievements were just unlocked
            List<string> achievements = new();

            if (previousCount == 0 && currentCount >= 1)
                achievements.Add("Welcome to Tahi!");

            if (previousCount < 3 && currentCount >= 3)
                achievements.Add("Lifelong Learner");

            return Json(new
            {
                success = true,
                showAchievement = achievements.Any(),
                achievements
            });
        }

        public class EnrollRequest
        {
            public int CourseId { get; set; }
        }

        [HttpGet("CourseModule/{id}")]
        public IActionResult CourseModule(int id)
        {
            var userId = User.FindFirst("UserId")?.Value;
            if (userId == null) return Unauthorized();

            var course = _context.Courses
                .Include(c => c.Artisan)
                .Include(c => c.Enrollments)
                .Include(c => c.Reviews)
                .FirstOrDefault(c => c.Id == id);

            if (course == null) return NotFound();

            // Get total modules (for now hardcoded as 5)
            int totalModules = 5;

            int completedModules = _context.ModuleProgress
                .Include(p => p.CourseModule)
                .Count(p => p.UserId == userId && p.CourseModule.CourseId == course.Id && p.IsCompleted);

            double progress = (double)completedModules / totalModules * 100;

            ViewData["CourseProgress"] = Math.Round(progress, 0);

            return View("CourseModules/CourseModule", course);
        }

        [HttpPost]
        [Route("Courses/UpdateProgress")]
        public IActionResult UpdateProgress([FromBody] ProgressUpdateModel model)
        {
            var userId = User.FindFirst("UserId")?.Value;
            if (userId == null)
                return Unauthorized();

            foreach (var moduleIndex in model.CompletedModules)
            {
                var courseModule = _context.CourseModules
                    .FirstOrDefault(m => m.CourseId == model.CourseId && m.Order == moduleIndex);

                if (courseModule == null) continue;

                var existing = _context.ModuleProgress
                    .FirstOrDefault(p => p.UserId == userId && p.CourseModuleId == courseModule.Id);

                if (existing == null)
                {
                    _context.ModuleProgress.Add(new ModuleProgress
                    {
                        UserId = userId,
                        CourseModuleId = courseModule.Id,
                        IsCompleted = true,
                        CompletedAt = DateTime.UtcNow,
                        LastUpdated = DateTime.UtcNow
                    });
                }
                else
                {
                    existing.IsCompleted = true;
                    existing.LastUpdated = DateTime.UtcNow;
                }
            }

            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = ex.Message, inner = ex.InnerException?.Message });
            }

            return Ok();
        }
    }
}