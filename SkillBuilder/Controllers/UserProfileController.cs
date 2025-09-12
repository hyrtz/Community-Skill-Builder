using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillBuilder.Data;
using SkillBuilder.Models;
using SkillBuilder.Models.ViewModels;

namespace SkillBuilder.Controllers
{
    [Authorize(AuthenticationSchemes = "TahiAuth", Roles = "Learner")]
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
            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (id != currentUserId)
            {
                return Forbid();
            }

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
                    .ToList();

                var completedSet = moduleProgress
                    .Where(mp => mp.IsCompleted && mp.CourseModule.CourseId == course.Id)
                    .Select(mp => mp.CourseModuleId)
                    .ToHashSet();

                int totalModules = courseModulesOrdered.Count;

                // ✅ Add project as a "virtual module" if the course has one
                if (!string.IsNullOrEmpty(course.ProjectDetails))
                    totalModules += 1;

                int completedModules = completedSet.Count;

                // ✅ Count project submission as completed
                var hasSubmission = _context.CourseProjectSubmissions
                    .Any(s => s.UserId == user.Id && s.CourseId == course.Id);

                if (hasSubmission)
                    completedModules += 1;

                var progress = totalModules == 0
                    ? 0
                    : (double)completedModules / totalModules * 100;

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

            List<SupportSessionRequest> artisanSupportRequests = new List<SupportSessionRequest>();
            if (user.Artisan != null)
            {
                var artisanId = user.Artisan.ArtisanId;
                artisanSupportRequests = _context.SupportSessionRequests
                    .Include(r => r.User)
                    .Include(r => r.Course)
                    .Where(r => r.Course.CreatedBy == artisanId)
                    .OrderByDescending(r => r.CreatedAt)
                    .ToList();
            }

            var achievements = GetAchievementsForUser(user);

            // ✅ RECOMMENDED COURSES LOGIC HERE
            var userInterests = user.SelectedInterests?
                .Split(',')
                .Select(i => i.Trim())
                .ToList() ?? new List<string>();

            var recommendedCourses = _context.Courses
                .Where(c => userInterests.Contains(c.Category))
                .OrderBy(c => Guid.NewGuid())
                .Take(4)
                .ToList();

            var viewModel = new UserProfileViewModel
            {
                User = user,
                EnrolledCourses = enrolledCourses,
                AllCourses = allCourses,
                SubmittedProjects = submittedProjects,
                Achievements = achievements,
                CourseProgresses = inProgressCourses,
                SupportRequests = supportRequests,
                ArtisanSupportRequests = artisanSupportRequests,
                RecommendedCourses = recommendedCourses // <-- add to ViewModel
            };

            return View("~/Views/Profile/UserProfile.cshtml", viewModel);
        }

        [HttpPost("SaveInterests")]
        public async Task<IActionResult> SaveInterests([FromBody] List<string> interests)
        {
            if (interests == null || interests.Count == 0)
                return BadRequest("Please select at least one interest.");

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not found.");

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound("User not found.");

            // Save as comma-separated values
            user.SelectedInterests = string.Join(",", interests);
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Interests saved successfully." });
        }

        [HttpPost("/api/UserProfile/UploadActivity")]
        public async Task<IActionResult> UploadActivity([FromForm] string courseId, [FromForm] IFormFile activityImage)
        {
            if (activityImage == null || activityImage.Length == 0)
                return BadRequest("No image uploaded.");

            var userId = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            if (!int.TryParse(courseId, out int parsedCourseId))
                return BadRequest("Invalid course ID.");

            var course = await _context.Courses.FindAsync(parsedCourseId);
            if (course == null) return NotFound("Course not found.");

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/projects");
            Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(activityImage.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await activityImage.CopyToAsync(stream);
            }

            var newSubmission = new CourseProjectSubmission
            {
                UserId = userId,
                CourseId = course.Id,
                ImageUrl = $"/uploads/projects/{fileName}",
                SubmittedAt = DateTime.Now
            };

            _context.CourseProjectSubmissions.Add(newSubmission);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Activity submitted!", imageUrl = newSubmission.ImageUrl });
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

            achievements.Add(new AchievementViewModel
            {
                Title = "Welcome to Tahi!",
                Condition = "Enroll at least 1 course",
                IsAchieved = user.Enrollments != null && user.Enrollments.Any()
            });

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

            var progressToRemove = _context.ModuleProgress
                .Where(mp => mp.UserId == userId)
                .ToList();

            if (progressToRemove.Any())
                _context.ModuleProgress.RemoveRange(progressToRemove);

            if (user.Enrollments != null && user.Enrollments.Any())
                _context.Enrollments.RemoveRange(user.Enrollments);

            var submissions = _context.CourseProjectSubmissions
                .Where(s => s.UserId == userId)
                .ToList();

            if (submissions.Any())
                _context.CourseProjectSubmissions.RemoveRange(submissions);

            _context.SaveChanges();

            return Redirect($"/UserProfile/{userId}");
        }

        [HttpGet("EditProfile")]
        public async Task<IActionResult> EditProfile()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return NotFound();

            var model = new UserProfileViewModel
            {
                User = user
            };

            return View("~/Views/Actions/EditProfile.cshtml", model);
        }

        [HttpPost("EditProfile")]
        public async Task<IActionResult> EditProfile(
            string FirstName,
            string LastName,
            string Email,
            IFormFile UserAvatar,
            string CurrentPassword,
            string NewPassword,
            string ConfirmPassword)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return NotFound();

            // Update basic info
            user.FirstName = FirstName;
            user.LastName = LastName;
            user.Email = Email;

            // Handle avatar upload
            if (UserAvatar != null && UserAvatar.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/avatars");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(UserAvatar.FileName)}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await UserAvatar.CopyToAsync(stream);
                }

                user.UserAvatar = $"/uploads/avatars/{fileName}";
            }

            // Handle password change
            if (!string.IsNullOrEmpty(CurrentPassword) || !string.IsNullOrEmpty(NewPassword) || !string.IsNullOrEmpty(ConfirmPassword))
            {
                if (string.IsNullOrEmpty(CurrentPassword) || string.IsNullOrEmpty(NewPassword) || string.IsNullOrEmpty(ConfirmPassword))
                {
                    TempData["ErrorMessage"] = "Please fill all password fields to update your password.";
                    return RedirectToAction("EditProfile");
                }

                if (!BCrypt.Net.BCrypt.Verify(CurrentPassword, user.PasswordHash))
                {
                    TempData["ErrorMessage"] = "Current password is incorrect.";
                    return RedirectToAction("EditProfile");
                }

                if (NewPassword != ConfirmPassword)
                {
                    TempData["ErrorMessage"] = "New password and confirmation do not match.";
                    return RedirectToAction("EditProfile");
                }

                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(NewPassword);
            }

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Profile updated successfully!";
            return RedirectToAction("EditProfile");
        }

        [HttpGet("VerifyOldPassword")]
        public IActionResult VerifyOldPassword(string oldPassword)
        {
            // Get the currently logged-in user
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null) return NotFound();

            // Check the password using BCrypt
            bool isValid = BCrypt.Net.BCrypt.Verify(oldPassword, user.PasswordHash);

            return Json(new { isValid });
        }

        [HttpPost("ArchiveAccount")]
        public async Task<IActionResult> ArchiveAccount()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var user = await _context.Users
                .Include(u => u.Artisan)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return NotFound();

            // ✅ Soft delete user
            user.IsArchived = true;

            // ✅ Soft delete artisan profile if exists
            if (user.Artisan != null)
            {
                user.Artisan.IsArchived = true;

                // ✅ Soft delete all courses created by this artisan
                var artisanCourses = await _context.Courses
                    .Where(c => c.CreatedBy == user.Artisan.ArtisanId)
                    .ToListAsync();

                foreach (var course in artisanCourses)
                {
                    course.IsArchived = true;
                }
            }

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            // ✅ Sign out after archive
            await HttpContext.SignOutAsync("TahiAuth");

            return RedirectToAction("Index", "Home");
        }
    }
}