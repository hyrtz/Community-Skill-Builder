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

            var courseProgresses = user.Enrollments?.Select(e =>
            {
                var courseId = e.Course.Id;

                var completedModules = _context.ModuleProgress
                    .Include(mp => mp.CourseModule)
                    .Where(mp => mp.UserId == user.Id && mp.CourseModule.CourseId == courseId && mp.IsCompleted)
                    .Count();

                var totalModules = _context.CourseModules
                    .Count(cm => cm.CourseId == courseId);

                var percentage = totalModules > 0 ? (double)completedModules / totalModules * 100 : 0;

                return new CourseProgressViewModel
                {
                    CourseId = courseId,
                    CourseTitle = e.Course.Title,
                    CourseDescription = e.Course.Description,
                    Link = e.Course.Link,
                    ProgressPercentage = Math.Round(percentage, 0)
                };
            }).ToList() ?? new();

            var moduleProgress = _context.ModuleProgress
                .Include(mp => mp.CourseModule)
                .Where(mp => mp.UserId == user.Id)
                .ToList();

            var enrolledCourses = user.Enrollments?.Select(e => e.Course).ToList() ?? new List<Course>();

            var inProgressCourses = enrolledCourses.Select(course =>
            {
                var totalModules = _context.CourseModules.Count(cm => cm.CourseId == course.Id);
                var completed = moduleProgress
                    .Where(mp => mp.IsCompleted && mp.CourseModule.CourseId == course.Id)
                    .Count();
                var progress = totalModules == 0 ? 0 : (double)completed / totalModules * 100;

                return new CourseProgressViewModel
                {
                    UserId = user.Id,
                    CourseId = course.Id,
                    CourseTitle = course.Title,
                    CourseDescription = course.Description,
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

            var achievements = GetAchievementsForUser(user);

            var viewModel = new UserProfileViewModel
            {
                User = user,
                EnrolledCourses = enrolledCourses,
                AllCourses = allCourses,
                SubmittedProjects = submittedProjects,
                Achievements = achievements,
                CourseProgresses = inProgressCourses
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