using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillBuilder.Data;
using SkillBuilder.Models;
using SkillBuilder.Models.ViewModels;

namespace SkillBuilder.Controllers
{
    [Route("UserProfile")]
    public class UserProfileController : Controller
    {
        private readonly AppDbContext _context;

        public UserProfileController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public IActionResult UserProfile(string id)
        {
            var user = _context.Users
                .Include(u => u.Artisan)
                .Include(u => u.Enrollments)
                    .ThenInclude(e => e.Course)
                .Include(u => u.Reviews)
                .FirstOrDefault(u => u.Id == id);

            if (user == null)
                return NotFound();

            var moduleProgress = _context.ModuleProgress
                .Include(mp => mp.CourseModule)
                .Where(mp => mp.UserId == user.Id)
                .ToList();

            var enrolledCourses = user.Enrollments?.Select(e => e.Course).ToList() ?? new List<Course>();

            var inProgressCourses = enrolledCourses.Select(course =>
            {
                var courseModulesOrdered = _context.CourseModules
                    .Where(cm => cm.CourseId == course.Id)
                    .OrderBy(cm => cm.Order)
                    .Include(cm => cm.Contents)
                    .ToList();

                var completedSet = moduleProgress
                    .Where(mp => mp.IsCompleted && mp.CourseModule.CourseId == course.Id)
                    .Select(mp => mp.CourseModuleId)
                    .ToHashSet();

                int validCompletions = 0;
                for (int i = 0; i < courseModulesOrdered.Count; i++)
                {
                    var module = courseModulesOrdered[i];
                    if (i == 0 || completedSet.Contains(courseModulesOrdered[i - 1].Id))
                    {
                        if (completedSet.Contains(module.Id))
                        {
                            validCompletions++;
                        }
                        else break;
                    }
                    else break;
                }

                int totalModules = courseModulesOrdered.SelectMany(cm => cm.Contents).Count();

                var progress = totalModules == 0 ? 0 : (double)validCompletions / totalModules * 100;

                return new CourseProgressViewModel
                {
                    UserId = user.Id,
                    CourseId = course.Id,
                    CourseTitle = course.Title,
                    CourseDescription = course.Overview,
                    ProgressPercentage = Math.Round(progress, 0)
                };
            }).ToList();

            var allCourses = _context.Courses
                .Include(c => c.Artisan)
                .ToList();

            var submittedProjects = _context.CourseProjectSubmissions
                .Include(p => p.Course)
                .Where(p => p.UserId == user.Id)
                .ToList();

            var supportRequests = _context.SupportSessionRequests
                .Where(r => r.UserId == user.Id)
                .Include(r => r.Course)
                .OrderByDescending(r => r.CreatedAt)
                .ToList();

            var achievements = GetAchievementsForUser(user);

            var viewModel = new UserProfileViewModel
            {
                User = user,
                EnrolledCourses = enrolledCourses,
                AllCourses = allCourses,
                SubmittedProjects = submittedProjects,
                Achievements = achievements,
                CourseProgresses = inProgressCourses,
                SupportRequests = supportRequests
            };

            return View("~/Views/Profile/UserProfile.cshtml", viewModel);
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            var userId = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();
            return Redirect($"/UserProfile/{userId}");
        }

        private List<AchievementViewModel> GetAchievementsForUser(User user)
        {
            var achievements = new List<AchievementViewModel>();

            // Achievement 1: First Course
            achievements.Add(new AchievementViewModel
            {
                Title = "Welcome to Tahi!",
                Condition = "Enroll at least 1 course",
                IsAchieved = user.Enrollments != null && user.Enrollments.Any()
            });

            // Achievement 2: Enroll in 3 Courses
            achievements.Add(new AchievementViewModel
            {
                Title = "Lifelong Learner",
                Condition = "Enroll in at least 3 courses",
                IsAchieved = user.Enrollments != null && user.Enrollments.Count() >= 3
            });

            return achievements;
        }

        [HttpGet("ResetAchievements/{userId}")]
        public IActionResult ResetAchievements(string userId)
        {
            var user = _context.Users
                .Include(u => u.Enrollments)
                .FirstOrDefault(u => u.Id == userId);

            if (user == null)
                return NotFound("User not found.");

            if (user.Enrollments != null && user.Enrollments.Any())
            {
                _context.Enrollments.RemoveRange(user.Enrollments);
                _context.SaveChanges();
            }

            return Redirect($"/UserProfile/{userId}");
        }

        [HttpGet("ResetAllProgress/{userId}")]
        public IActionResult ResetAllProgress(string userId)
        {
            var user = _context.Users
                .Include(u => u.Enrollments)
                .FirstOrDefault(u => u.Id == userId);

            if (user == null)
                return NotFound("User not found.");

            // Remove all module progress
            var progressToRemove = _context.ModuleProgress
                .Where(mp => mp.UserId == userId)
                .ToList();

            if (progressToRemove.Any())
                _context.ModuleProgress.RemoveRange(progressToRemove);

            // Remove all enrollments
            if (user.Enrollments != null && user.Enrollments.Any())
                _context.Enrollments.RemoveRange(user.Enrollments);

            // Optionally remove submitted projects
            var submissions = _context.CourseProjectSubmissions
                .Where(s => s.UserId == userId)
                .ToList();

            if (submissions.Any())
                _context.CourseProjectSubmissions.RemoveRange(submissions);

            _context.SaveChanges();

            return Redirect($"/UserProfile/{userId}");
        }
    }
}