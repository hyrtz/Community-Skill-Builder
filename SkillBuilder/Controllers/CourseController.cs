using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillBuilder.Data;
using SkillBuilder.Models;
using SkillBuilder.Models.ViewModels;
using SkillBuilder.Services;
using System.Reflection;
using System.Security.Claims;

namespace SkillBuilder.Controllers
{
    [Route("Courses")]
    public class CoursesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly INotificationService _notificationService;

        public CoursesController(AppDbContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        [HttpGet("")]
        public IActionResult CourseCatalog(string? selectedCourse = null, string? search = null)
        {
            ViewData["UseCourseNavbar"] = true;

            var courses = _context.Courses
                .Include(c => c.Artisan)
                .Include(c => c.Enrollments)
                .Include(c => c.Reviews).ThenInclude(r => r.User)
                .Include(c => c.CourseModules).ThenInclude(m => m.Contents)
                .AsQueryable();

            // Only show published courses for normal users
            if (!User.IsInRole("Admin"))
            {
                var userId = User.FindFirstValue("UserId");
                courses = courses.Where(c => c.IsPublished || c.Artisan.UserId == userId);
            }

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                courses = courses.Where(c => c.Title.ToLower().Contains(search));
            }

            var courseList = courses.ToList();

            if (!string.IsNullOrEmpty(selectedCourse))
            {
                var course = courseList.FirstOrDefault(c => c.Link == selectedCourse);
                if (course == null)
                    return NotFound();

                ViewData["ShowCourseDetails"] = true;
                return View("CourseCatalog", new CourseCatalogViewModel
                {
                    Courses = courseList,
                    SelectedCourse = course
                });
            }

            ViewData["ShowCourseDetails"] = false;

            return View("CourseCatalog", new CourseCatalogViewModel
            {
                Courses = courseList,
                SelectedCourse = null
            });
        }

        [HttpGet("RecommendedCourses")]
        public IActionResult RecommendedCourses()
        {
            var userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            // Get user's interests
            var userInterests = _context.Users
                .Where(u => u.Id == userId)
                .Select(u => u.SelectedInterests)
                .FirstOrDefault();

            // If user has no interests, just show 4 random courses
            if (string.IsNullOrWhiteSpace(userInterests))
            {
                var randomCourses = _context.Courses
                    .OrderBy(c => Guid.NewGuid())
                    .Take(4)
                    .ToList();

                return PartialView("_RecommendedCourses", randomCourses);
            }

            var interestList = userInterests
                .Split(',')
                .Select(i => i.Trim().ToLower())
                .ToList();

            // Step 1: Get courses that match interests (partial matching)
            var matchedCourses = _context.Courses
                .Where(c => c.IsPublished && interestList.Any(interest =>
                    c.Category.ToLower().Contains(interest) ||
                    c.Title.ToLower().Contains(interest)))
                .OrderBy(c => Guid.NewGuid())
                .Take(4)
                .ToList();

            // Step 2: If fewer than 4 matches, fill with random other courses
            if (matchedCourses.Count < 4)
            {
                var remainingCount = 4 - matchedCourses.Count;
                var remainingCourses = _context.Courses
                    .Where(c => !matchedCourses.Select(m => m.Id).Contains(c.Id))
                    .OrderBy(c => Guid.NewGuid())
                    .Take(remainingCount)
                    .ToList();

                matchedCourses.AddRange(remainingCourses);
            }

            return PartialView("_RecommendedCourses", matchedCourses);
        }

        [HttpPost("Enroll")]
        public async Task<IActionResult> Enroll([FromBody] EnrollRequest request)
        {
            var userId = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { success = false, message = "Login required." });

            var user = _context.Users
                .Include(u => u.Enrollments)
                .FirstOrDefault(u => u.Id == userId);

            if (user == null)
                return NotFound(new { success = false, message = "User not found." });

            if (!user.IsVerified)
                return BadRequest(new { success = false, message = "Please verify your email before enrolling in a course." });

            if (user.IsDeactivated)
                return BadRequest(new { success = false, message = "Your account is deactivated. Please contact support." });

            var course = _context.Courses
                .Include(c => c.Artisan)
                .FirstOrDefault(c => c.Id == request.CourseId);

            if (course == null)
                return NotFound(new { success = false, message = "Course not found." });

            if (user.Role == "Artisan" && course.Artisan.UserId == userId)
                return BadRequest(new { success = false, message = "You cannot enroll in your own course." });

            var alreadyEnrolled = user.Enrollments.Any(e => e.CourseId == request.CourseId);
            if (alreadyEnrolled)
                return BadRequest(new { success = false, message = "Already enrolled." });

            int previousCount = user.Enrollments?.Count() ?? 0;

            _context.Enrollments.Add(new Enrollment
            {
                UserId = userId,
                CourseId = request.CourseId,
                EnrolledAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync(); // async version of SaveChanges

            int currentCount = _context.Enrollments.Count(e => e.UserId == userId);

            List<string> achievements = new();
            if (previousCount == 0 && currentCount >= 1)
                achievements.Add("Welcome to Tahi!");
            if (previousCount < 3 && currentCount >= 3)
                achievements.Add("Lifelong Learner");

            // Notifications
            if (course.Artisan != null)
            {
                await _notificationService.AddNotificationAsync(
                    course.Artisan.UserId,
                    $"{user.FirstName} enrolled in your course \"{course.Title}\"."
                );
            }

            await _notificationService.AddNotificationAsync(
                userId,
                $"You successfully enrolled in the course \"{course.Title}\"."
            );

            return Json(new
            {
                success = true,
                showAchievement = achievements.Any(),
                achievements
            });
        }

        public class EnrollRequest
        {
            public int CourseId { get; set; }
        }

        [HttpGet("CourseModule/{id}")]
        public IActionResult CourseModule(int id)
        {
            var userId = User.FindFirst("UserId")?.Value;
            if (userId == null) return Unauthorized();

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null) return Unauthorized();

            if (user.IsDeactivated)
            {
                TempData["ErrorMessage"] = "Your account is deactivated. You cannot access courses.";
                return RedirectToAction("CourseCatalog");
            }

            var course = _context.Courses
                .Include(c => c.Artisan)
                .Include(c => c.Enrollments)
                .Include(c => c.Reviews)
                .Include(c => c.Materials)
                .Include(c => c.CourseModules)
                    .ThenInclude(m => m.Contents)
                        .ThenInclude(content => content.QuizQuestions)
                .FirstOrDefault(c => c.Id == id);

            if (course == null) return NotFound();

            // 🔒 Prevent access to unpublished courses for normal users
            if (!course.IsPublished && course.Artisan.UserId != userId && !User.IsInRole("Admin"))
            {
                TempData["ErrorMessage"] = "You cannot access this course because it is not published.";
                return RedirectToAction("CourseCatalog");
            }

            var isEnrolled = _context.Enrollments
                .Any(e => e.CourseId == id && e.UserId == userId);

            if (!isEnrolled && course.Artisan.UserId != userId)
            {
                return Forbid();
            }

            // Count all contents in this course
            int totalContents = course.CourseModules
                .SelectMany(m => m.Contents)
                .Count();

            // Find completed modules for this user
            var completedModules = _context.ModuleProgress
                .Where(mp => mp.UserId == userId && mp.CourseModule.CourseId == course.Id && mp.IsCompleted)
                .Select(mp => mp.CourseModuleId)
                .ToHashSet();

            var orderedModules = course.CourseModules
                .OrderBy(cm => cm.Order)
                .ToList();

            // Count completed contents = all contents belonging to completed modules
            int completedContents = course.CourseModules
                .Where(m => completedModules.Contains(m.Id))
                .SelectMany(m => m.Contents)
                .Count();

            // Calculate percentage
            double progress = totalContents == 0 ? 0 : (double)completedContents / totalContents * 100;
            ViewData["CourseProgress"] = Math.Round(progress, 0);

            // (Optional) figure out which module to show first: the first uncompleted one
            var nextModule = orderedModules.FirstOrDefault(m => !completedModules.Contains(m.Id))
                             ?? orderedModules.Last();
            ViewData["NextModuleId"] = nextModule?.Id;

            return View("CourseModules/CourseModule", course);
        }

        private async Task RecalculateProgress(string userId, int courseId)
        {
            var allModuleIds = await _context.CourseModules
                .Where(m => m.CourseId == courseId)
                .Select(m => m.Id)
                .ToListAsync();

            var completedModuleIds = await _context.ModuleProgress
                .Where(mp => mp.UserId == userId && mp.CourseModule.CourseId == courseId && mp.IsCompleted)
                .Select(mp => mp.CourseModuleId)
                .ToListAsync();

            int totalModules = allModuleIds.Count;
            int completedCount = completedModuleIds.Count;

            double progress = totalModules == 0 ? 0 : (double)completedCount / totalModules * 100;

            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.UserId == userId && e.CourseId == courseId);

            var hasSubmittedFinalProject = await _context.CourseProjectSubmissions
                .AnyAsync(s => s.UserId == userId && s.CourseId == courseId &&
                               (s.Status == "Pending" || s.Status == "Approved"));

            if ((progress >= 100 || hasSubmittedFinalProject) && enrollment != null && !enrollment.IsCompleted)
            {
                enrollment.IsCompleted = true;
                enrollment.CompletedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        [HttpPost("SubmitFinalProject")]
        [RequestSizeLimit(200 * 1024 * 1024)]
        public async Task<IActionResult> SubmitFinalProject([FromForm] FinalProjectDto dto)
        {
            var userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound();

            var course = await _context.Courses
                .Include(c => c.Artisan) // include Artisan to get owner
                .FirstOrDefaultAsync(c => c.Id == dto.CourseId);
            if (course == null) return NotFound();

            string mediaUrl = null;

            if (dto.File != null && dto.File.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "projects");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                var fileName = $"{Guid.NewGuid()}_{dto.File.FileName}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.File.CopyToAsync(stream);
                }

                mediaUrl = $"/uploads/projects/{fileName}";
            }

            var submission = new CourseProjectSubmission
            {
                UserId = userId,
                CourseId = dto.CourseId,
                Title = dto.Title,
                Description = dto.Description,
                MediaUrl = mediaUrl,
                SubmittedAt = DateTime.UtcNow,
                Status = "Pending"
            };

            _context.CourseProjectSubmissions.Add(submission);
            await _context.SaveChangesAsync();

            await RecalculateProgress(userId, dto.CourseId);

            // ✅ Notifications
            await _notificationService.AddNotificationAsync(
                userId,
                $"Your final project \"{submission.Title}\" has been submitted successfully."
            );

            if (course.Artisan != null)
            {
                await _notificationService.AddNotificationAsync(
                    course.Artisan.UserId,
                    $"A new final project \"{submission.Title}\" has been submitted for your course \"{course.Title}\"."
                );
            }

            return Ok(new { success = true, submissionId = submission.Id });
        }

        public class FinalProjectDto
        {
            public int CourseId { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public IFormFile File { get; set; }
        }

        [HttpPost("SubmitReview")]
        public async Task<IActionResult> SubmitReview([FromBody] SubmitReviewRequest request)
        {
            var userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { success = false, message = "Login required." });

            // Check if user is enrolled in the course
            var isEnrolled = _context.Enrollments.Any(e => e.CourseId == request.CourseId && e.UserId == userId);
            if (!isEnrolled)
                return BadRequest(new { success = false, message = "You are not enrolled in this course." });

            var review = new CourseReview
            {
                CourseId = request.CourseId,
                UserId = userId,
                Rating = request.Rating,
                Comment = request.Comment ?? "",
                CreatedAt = DateTime.UtcNow
            };

            _context.CourseReviews.Add(review);
            await _context.SaveChangesAsync();

            // Recalculate average rating
            var reviews = _context.CourseReviews.Where(r => r.CourseId == request.CourseId);
            var average = reviews.Any() ? reviews.Average(r => r.Rating) : 0;
            var totalReviews = reviews.Count();

            var course = _context.Courses.Include(c => c.Artisan).FirstOrDefault(c => c.Id == request.CourseId);
            if (course != null && course.Artisan != null)
            {
                await _notificationService.AddNotificationAsync(
                    course.Artisan.UserId,
                    $"{User.Identity.Name} submitted a review for your course \"{course.Title}\"."
                );
            }

            await _notificationService.AddNotificationAsync(
                userId,
                $"You submitted a review for the course \"{course?.Title}\"."
            );

            return Ok(new
            {
                success = true,
                averageRating = average,
                totalReviews
            });
        }

        public class SubmitReviewRequest
        {
            public int CourseId { get; set; }
            public int Rating { get; set; }
            public string? Comment { get; set; }
        }

        [HttpPost("UpdateProgress")]
        public async Task<IActionResult> UpdateProgress([FromBody] ProgressUpdateModel model)
        {
            var userId = User.FindFirst("UserId")?.Value;
            if (userId == null)
                return Unauthorized();

            foreach (var moduleIndex in model.CompletedModules)
            {
                var courseModule = await _context.CourseModules
                    .FirstOrDefaultAsync(m => m.CourseId == model.CourseId && m.Order == moduleIndex);

                if (courseModule == null) continue;

                var existing = await _context.ModuleProgress
                    .FirstOrDefaultAsync(p => p.UserId == userId && p.CourseModuleId == courseModule.Id);

                if (existing == null)
                {
                    _context.ModuleProgress.Add(new ModuleProgress
                    {
                        UserId = userId,
                        CourseModuleId = courseModule.Id,
                        IsCompleted = true,
                        CompletedAt = DateTime.UtcNow,
                        LastUpdated = DateTime.UtcNow
                    });
                }
                else
                {
                    existing.IsCompleted = true;
                    existing.LastUpdated = DateTime.UtcNow;
                }
            }

            await _context.SaveChangesAsync();

            // Recalculate overall progress (shared)
            await RecalculateProgress(userId, model.CourseId);

            return Ok();
        }

        [HttpPost("ResetProgress")]
        public IActionResult ResetProgress([FromBody] ProgressUpdateModel model)
        {
            var userId = User.FindFirstValue("UserId");

            // Remove module progress
            var toDelete = _context.ModuleProgress
                .Where(p => p.UserId == userId &&
                            _context.CourseModules
                                    .Where(m => m.CourseId == model.CourseId)
                                    .Select(m => m.Id)
                                    .Contains(p.CourseModuleId))
                .ToList();

            _context.ModuleProgress.RemoveRange(toDelete);

            // Reset enrollment completion
            var enrollment = _context.Enrollments
                .FirstOrDefault(e => e.UserId == userId && e.CourseId == model.CourseId);

            if (enrollment != null)
            {
                enrollment.IsCompleted = false;
                enrollment.CompletedAt = null;
            }

            var submissions = _context.CourseProjectSubmissions
                .Where(s => s.UserId == userId && s.CourseId == model.CourseId)
                .ToList();

            _context.CourseProjectSubmissions.RemoveRange(submissions);

            _context.SaveChanges();

            return Ok(new { message = "Reset successful" });
        }

        [HttpPost("SaveUserPoints")]
        public async Task<IActionResult> SaveUserPoints([FromBody] SavePointsRequest request)
        {
            try
            {
                // Check if user is authenticated
                var userId = User.FindFirstValue("UserId");
                if (string.IsNullOrEmpty(userId))
                {
                    Console.WriteLine("⚠️ SaveUserPoints: No user ID found (unauthenticated request).");
                    return Unauthorized(new { success = false, message = "User not logged in." });
                }

                // Find user
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                {
                    Console.WriteLine($"⚠️ SaveUserPoints: User not found for ID {userId}");
                    return NotFound(new { success = false, message = "User not found." });
                }

                // Add points safely
                user.Points += request.Points;
                await _context.SaveChangesAsync();

                Console.WriteLine($"✅ SaveUserPoints: Added {request.Points} points for user {user.Email}. New total: {user.Points}");

                return Ok(new
                {
                    success = true,
                    totalPoints = user.Points
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ SaveUserPoints error: {ex.Message}\n{ex.StackTrace}");
                return StatusCode(500, new { success = false, message = "Internal server error while saving points." });
            }
        }

        public class SavePointsRequest
        {
            public int Points { get; set; }
        }
    }
}