using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillBuilder.Data;
using SkillBuilder.Models.ViewModels;
using System.Linq;

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
            var usersQuery = _context.Users
                .Include(u => u.Enrollments)
                .Where(u => u.Role == "Learner");

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                usersQuery = usersQuery.Where(u =>
                    u.FirstName.ToLower().Contains(search) ||
                    u.LastName.ToLower().Contains(search));
            }

            var sortedUsers = usersQuery
                .OrderByDescending(u => u.Points)
                .ToList();

            var currentUserId = User.FindFirst("UserId")?.Value;
            var currentUser = _context.Users.FirstOrDefault(u => u.Id == currentUserId);

            var viewModel = new UserProfileViewModel
            {
                User = currentUser,
                LeaderboardUsers = sortedUsers
            };

            ViewBag.Search = search;
            return View(viewModel);
        }
    }
}