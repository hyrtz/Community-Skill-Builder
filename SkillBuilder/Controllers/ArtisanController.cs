using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillBuilder.Data;

namespace SkillBuilder.Controllers
{
    public class ArtisansController : Controller
    {
        private readonly AppDbContext _context;

        public ArtisansController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> ArtisanList(string? search)
        {
            var query = _context.Artisans
                .Include(a => a.User)
                .Where(a => a.User != null && a.User.Role == "Artisan");

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                query = query.Where(a =>
                    (a.User.FirstName + " " + a.User.LastName).ToLower().Contains(search));
            }

            var artisans = await query.ToListAsync();
            return View(artisans);

        }
    }
}