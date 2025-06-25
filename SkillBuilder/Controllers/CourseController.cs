using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillBuilder.Data;
using SkillBuilder.Models;

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

            // ✅ Fetch courses from the actual database
            var courses = _context.Courses
                .Include(c => c.Artisan) 
                .Include(c => c.Enrollments) 
                .Include(c => c.Reviews) 
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
    }
}