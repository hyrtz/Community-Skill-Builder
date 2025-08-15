using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillBuilder.Data;

namespace SkillBuilder.Controllers
{
    public class LeaderboardController : Controller
    {
        private readonly AppDbContext _context;

        public LeaderboardController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("UserLeaderboard")]
        public IActionResult UserLeaderboard(string? search)
        {
            var users = _context.Users
                .Include(u => u.Enrollments)
                .Where(u => u.Role == "Learner");

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                users = users.Where(u =>
                    u.FirstName.ToLower().Contains(search) ||
                    u.LastName.ToLower().Contains(search));
            }

            var sortedUsers = users
                .OrderByDescending(u => u.Points)
                .ToList();

            ViewBag.Search = search;
            return View(sortedUsers);
        }
    }
}
