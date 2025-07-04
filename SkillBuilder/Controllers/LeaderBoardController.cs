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
        public IActionResult UserLeaderboard()
        {
            var users = _context.Users
                .Include(u => u.Enrollments)
                .Where(u => u.Role == "Learner")
                .ToList();

            return View(users);
        }
    }
}
