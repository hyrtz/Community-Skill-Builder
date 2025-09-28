using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillBuilder.Data;
using SkillBuilder.Models;
using SkillBuilder.Models.ViewModels;
using SkillBuilder.Services;

namespace SkillBuilder.Controllers
{
    [Authorize(AuthenticationSchemes = "TahiAuth", Roles = "Learner")]
    [Route("UserProfile")]
    public class UserProfileController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;
        private readonly INotificationService _notificationService;

        public UserProfileController(AppDbContext context, IEmailService emailService, INotificationService notificationService)
        {
            _context = context;
            _emailService = emailService;
            _notificationService = notificationService;
        }

        [HttpGet("{id}")]
        public IActionResult UserProfile(string id)
        {
            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (id != currentUserId)
                return Forbid();

            var user = _context.Users
                .Include(u => u.Artisan)
                .Include(u => u.Enrollments)
                    .ThenInclude(e => e.Course)
                .Include(u => u.Reviews)
                .FirstOrDefault(u => u.Id == id);

            if (user == null) return NotFound();

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
                int completedModules = completedSet.Count;

                // Count project submission as completed
                var hasSubmission = _context.CourseProjectSubmissions
                    .Any(s => s.UserId == user.Id && s.CourseId == course.Id);
                if (hasSubmission) completedModules += 1;

                var progress = totalModules == 0 ? 0 : (double)completedModules / totalModules * 100;

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
                RecommendedCourses = recommendedCourses
            };

            return View("~/Views/Profile/UserProfile.cshtml", viewModel);
        }

        [HttpPost("SaveInterests")]
        public async Task<IActionResult> SaveInterests([FromBody] List<string> interests)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized("User not found.");

            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound("User not found.");

            // Save interests (empty allowed)
            user.SelectedInterests = interests != null && interests.Count > 0
                ? string.Join(",", interests)
                : null;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Interests saved successfully." });
        }

        [HttpPost("/api/UserProfile/UploadActivity")]
        public async Task<IActionResult> UploadActivity([FromForm] string courseId, [FromForm] IFormFile activityImage)
        {
            if (activityImage == null || activityImage.Length == 0)
                return BadRequest("No image uploaded.");

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
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
                MediaUrl = $"/uploads/projects/{fileName}",
                SubmittedAt = DateTime.Now
            };

            _context.CourseProjectSubmissions.Add(newSubmission);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Activity submitted!", imageUrl = newSubmission.MediaUrl });
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
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

            if (user == null) return NotFound("User not found.");

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

            if (user == null) return NotFound("User not found.");

            var progressToRemove = _context.ModuleProgress
                .Where(mp => mp.UserId == userId)
                .ToList();
            if (progressToRemove.Any()) _context.ModuleProgress.RemoveRange(progressToRemove);

            if (user.Enrollments != null && user.Enrollments.Any())
                _context.Enrollments.RemoveRange(user.Enrollments);

            var submissions = _context.CourseProjectSubmissions
                .Where(s => s.UserId == userId)
                .ToList();
            if (submissions.Any()) _context.CourseProjectSubmissions.RemoveRange(submissions);

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

            var model = new UserProfileViewModel { User = user };
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

            user.FirstName = FirstName;
            user.LastName = LastName;

            if (!string.Equals(user.Email, Email, StringComparison.OrdinalIgnoreCase))
            {
                bool emailExists = await _context.Users
                    .AnyAsync(u => u.Email == Email && u.Id != user.Id);
                if (emailExists)
                {
                    TempData["ErrorMessage"] = "This email is already in use.";
                    return RedirectToAction("EditProfile");
                }

                user.Email = Email;
                user.IsVerified = false;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                await _emailService.SendVerificationEmail(user.Email, user.Id, Url);

                await HttpContext.SignOutAsync("TahiAuth");
                TempData["SuccessMessage"] = "Your email has been updated. Please check your inbox to verify your new email address and log in again.";
                return RedirectToAction("Index", "Home");
            }

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

            if (!string.IsNullOrEmpty(CurrentPassword) ||
                !string.IsNullOrEmpty(NewPassword) ||
                !string.IsNullOrEmpty(ConfirmPassword))
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
                TempData["SuccessMessage"] = "Password updated successfully!";
            }

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            await AddNotificationAsync(
                userId,
                $"✅ You have successfully edited your Profile."
            );

            if (string.IsNullOrEmpty((string)TempData["SuccessMessage"]))
                TempData["SuccessMessage"] = "Profile updated successfully!";

            return RedirectToAction("EditProfile");
        }

        [AllowAnonymous]
        [HttpGet("CheckEmailExist")]
        public JsonResult CheckEmailExist(string email)
        {
            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            bool exists = false;

            if (!string.IsNullOrEmpty(currentUserId))
                exists = _context.Users.Any(u => u.Email == email && u.Id != currentUserId);
            else
                exists = _context.Users.Any(u => u.Email == email);

            return Json(new { exists });
        }

        [HttpGet("VerifyOldPassword")]
        public IActionResult VerifyOldPassword(string oldPassword)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null) return NotFound();

            bool isValid = BCrypt.Net.BCrypt.Verify(oldPassword, user.PasswordHash);
            return Json(new { isValid });
        }

        [HttpPost("ResubmitFinalProject/{projectId}")]
        public async Task<IActionResult> ResubmitFinalProject(
            int projectId,
            [FromForm] string Title,
            [FromForm] string Description,
            [FromForm] IFormFile projectFile)
        {
            if (projectFile == null || projectFile.Length == 0)
                return BadRequest(new { success = false, message = "No file uploaded." });

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var project = await _context.CourseProjectSubmissions
                .Include(p => p.Course)
                    .ThenInclude(c => c.Artisan)
                .FirstOrDefaultAsync(p => p.Id == projectId && p.UserId == userId);

            if (project == null)
                return NotFound(new { success = false, message = "Project not found." });

            // Save uploaded file
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/projects");
            Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(projectFile.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await projectFile.CopyToAsync(stream);
            }

            // Update project submission
            project.MediaUrl = $"/uploads/projects/{fileName}";
            project.SubmittedAt = DateTime.Now;
            project.Status = "Pending";

            // <-- Update title and description
            project.Title = Title;
            project.Description = Description;

            _context.CourseProjectSubmissions.Update(project);

            // Notifications
            var artisanUserId = project.Course.Artisan?.UserId;
            if (!string.IsNullOrEmpty(artisanUserId))
            {
                await AddNotificationAsync(
                    artisanUserId,
                    $"📌 Project resubmitted by {User.Identity.Name} for course '{project.Course.Title}'."
                );
            }

            await AddNotificationAsync(
                userId,
                $"✅ You have successfully resubmitted your project for course '{project.Course.Title}'."
            );

            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Project resubmitted successfully.", fileUrl = project.MediaUrl });
        }

        private async Task AddNotificationAsync(string userId, string message, string? actionText = null, string? actionUrl = null)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(message))
                return;

            var notification = new Notification
            {
                UserId = userId,
                Message = message,
                ActionText = actionText,
                ActionUrl = actionUrl,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
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

            user.IsArchived = true;

            if (user.Artisan != null)
            {
                user.Artisan.IsArchived = true;

                var artisanCourses = await _context.Courses
                    .Where(c => c.CreatedBy == user.Artisan.ArtisanId)
                    .ToListAsync();

                foreach (var course in artisanCourses)
                    course.IsArchived = true;
            }

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            await HttpContext.SignOutAsync("TahiAuth");
            return RedirectToAction("Index", "Home");
        }

        public class RescheduleSessionRequest
        {
            public DateTime NewDate { get; set; }
            public TimeSpan NewTime { get; set; }
            public string Message { get; set; } = "";
        }

        [HttpPost("RescheduleSessionByCourse/{courseId}")]
        public async Task<IActionResult> RescheduleSessionByCourse(int courseId, [FromBody] RescheduleSessionRequest data)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { success = false, message = "Unauthorized" });

            // Get the latest session for this user & course
            var session = await _context.SupportSessionRequests
                .Where(r => r.CourseId == courseId && r.UserId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .Include(r => r.Course)
                    .ThenInclude(c => c.Artisan)
                .FirstOrDefaultAsync();

            if (session == null)
                return NotFound(new { success = false, message = "Session not found" });

            session.SessionDate = data.NewDate;
            session.SessionTime = data.NewTime;
            session.Message = data.Message;
            session.Status = "Pending";
            session.ConfirmedAt = null;
            session.CompletedAt = null;

            _context.SupportSessionRequests.Update(session);
            await _context.SaveChangesAsync();

            // Notify artisan (simple notification, no action button)
            if (session.Course?.Artisan != null)
            {
                await AddNotificationAsync(
                    session.Course.Artisan.UserId,
                    $"📌 {session.User?.FirstName} has rescheduled a support session for '{session.Course.Title}'."
                );
            }

            // Notify learner (simple notification)
            await AddNotificationAsync(
                session.UserId,
                $"✅ You successfully rescheduled your support session for '{session.Course?.Title}'."
            );

            return Ok(new { success = true, message = "Session successfully rescheduled" });
        }

    }
}