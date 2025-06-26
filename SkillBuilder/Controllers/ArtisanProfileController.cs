using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillBuilder.Models;
using SkillBuilder.Models.ViewModels;
using SkillBuilder.Data;

namespace SkillBuilder.Controllers
{
    [Route("ArtisanProfile")]
    public class ArtisanProfileController : Controller
    {
        private readonly AppDbContext _context;

        public ArtisanProfileController(AppDbContext context)
        {
            _context = context;
        }

        // For Artisan Dashboard (used by the artisan themselves)
        [HttpGet("{id}")]
        public IActionResult ArtisanProfile(string id)
        {
            var artisan = _context.Artisans
                .Include(a => a.User)
                .FirstOrDefault(a => a.ArtisanId == id);

            if (artisan == null)
                return NotFound();

            var courses = _context.Courses
                .Where(c => c.CreatedBy == id)
                .ToList();

            var works = _context.ArtisanWorks
                .Where(w => w.ArtisanId == id)
                .ToList();

            var viewModel = new ArtisanProfileViewModel
            {
                Artisan = artisan,
                Courses = courses,
                ArtisanWorks = works
            };

            return View("~/Views/Profile/ArtisanProfile.cshtml", viewModel);
        }

        // For public "View as Mentor"
        [HttpGet("/ArtisanViewAsMentor/{id}")]
        public IActionResult ViewAsMentor(string id)
        {
            var artisan = _context.Artisans
                .Include(a => a.User)
                .FirstOrDefault(a => a.ArtisanId == id);

            if (artisan == null)
                return NotFound();

            var courses = _context.Courses
                .Where(c => c.CreatedBy == id)
                .ToList();

            var works = _context.ArtisanWorks
                .Where(w => w.ArtisanId == id)
                .ToList();

            var viewModel = new ArtisanProfileViewModel
            {
                Artisan = artisan,
                Courses = courses,
                ArtisanWorks = works
            };

            return View("~/Views/Profile/ArtisanViewAsMentor.cshtml", viewModel);
        }
    }
}