using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillBuilder.Data;
using SkillBuilder.Models;
using SkillBuilder.Models.ViewModels;
using System.Reflection;
using System.Security.Claims;

namespace SkillBuilder.Controllers
{
    [Route("Courses")]
    public class CoursesController : Controller
    {
        private readonly AppDbContext _context;

        public CoursesController(AppDbContext context)
        {
            _context = context;
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
                .Where(c => interestList.Any(interest =>
                    c.Category.ToLower().Contains(interest) ||
                    c.Title.ToLower().Contains(interest)))
                .OrderBy(c => Guid.NewGuid()) // randomize
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
        public IActionResult Enroll([FromBody] EnrollRequest request)
        {
            var userId = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { success = false, message = "Login required." });

            var user = _context.Users
                .Include(u => u.Enrollments)
                .FirstOrDefault(u => u.Id == userId);

            if (user == null)
                return NotFound(new { success = false, message = "User not found." });

            // ✅ Check verification
            if (!user.IsVerified)
                return BadRequest(new { success = false, message = "Please verify your email before enrolling in a course." });

            // ✅ Check deactivation
            if (user.IsDeactivated)
                return BadRequest(new { success = false, message = "Your account is deactivated. Please contact support." });

            // ✅ Prevent Artisan from enrolling in their own course
            var course = _context.Courses
                .Include(c => c.Artisan)
                .FirstOrDefault(c => c.Id == request.CourseId);

            if (course == null)
                return NotFound(new { success = false, message = "Course not found." });

            if (user.Role == "Artisan" && course.Artisan.UserId == userId)
                return BadRequest(new { success = false, message = "You cannot enroll in your own course." });

            // ✅ Prevent duplicate enrollments
            var alreadyEnrolled = user.Enrollments.Any(e => e.CourseId == request.CourseId);
            if (alreadyEnrolled)
                return BadRequest(new { success = false, message = "Already enrolled." });

            // Track achievement conditions before enrollment
            int previousCount = user.Enrollments?.Count() ?? 0;

            // Add the new enrollment
            _context.Enrollments.Add(new Enrollment
            {
                UserId = userId,
                CourseId = request.CourseId,
                EnrolledAt = DateTime.UtcNow
            });

            _context.SaveChanges();

            // Re-check current count
            int currentCount = _context.Enrollments.Count(e => e.UserId == userId);

            // Determine which achievements were just unlocked
            List<string> achievements = new();

            if (previousCount == 0 && currentCount >= 1)
                achievements.Add("Welcome to Tahi!");

            if (previousCount < 3 && currentCount >= 3)
                achievements.Add("Lifelong Learner");

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
                return RedirectToAction("CourseCatalog"); // or return Forbid();
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

            var isEnrolled = _context.Enrollments
                .Any(e => e.CourseId == id && e.UserId == userId);

            if (!isEnrolled && course.Artisan.UserId != userId)
            {
                return Forbid();
            }

            int totalModules = course.CourseModules
                .SelectMany(m => m.Contents)
                .Count();

            var completedSet = _context.ModuleProgress
                .Where(mp => mp.UserId == userId && mp.CourseModule.CourseId == course.Id && mp.IsCompleted)
                .Select(mp => mp.CourseModuleId)
                .ToHashSet();

            var orderedModules = course.CourseModules
                .OrderBy(cm => cm.Order)
                .ToList();

            int validCompletions = 0;
            for (int i = 0; i < orderedModules.Count; i++)
            {
                if (i == 0 || completedSet.Contains(orderedModules[i - 1].Id))
                {
                    if (completedSet.Contains(orderedModules[i].Id))
                    {
                        validCompletions++;
                    }
                    else break;
                }
                else break;
            }

            double progress = totalModules == 0 ? 0 : (double)validCompletions / totalModules * 100;
            ViewData["CourseProgress"] = Math.Round(progress, 0);

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

            if (progress >= 100 && enrollment != null && !enrollment.IsCompleted)
            {
                enrollment.IsCompleted = true;
                enrollment.CompletedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        [HttpPost("UploadActivity")]
        public async Task<IActionResult> UploadActivity(IFormFile file, int courseId, int moduleIndex)
        {
            var userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "activities");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var fileUrl = $"/uploads/activities/{uniqueFileName}";

            // Save submission (course-level)
            var submission = new CourseProjectSubmission
            {
                UserId = userId,
                CourseId = courseId,
                ImageUrl = fileUrl,
                SubmittedAt = DateTime.UtcNow
            };

            _context.CourseProjectSubmissions.Add(submission);

            // Attempt to find the CourseModule by Order == moduleIndex
            // NOTE: moduleIndex must match how you send it from the client (module order)
            var courseModule = await _context.CourseModules
                .FirstOrDefaultAsync(m => m.CourseId == courseId && m.Order == moduleIndex);

            if (courseModule != null)
            {
                var moduleProgress = await _context.ModuleProgress
                    .FirstOrDefaultAsync(mp => mp.UserId == userId && mp.CourseModuleId == courseModule.Id);

                if (moduleProgress == null)
                {
                    moduleProgress = new ModuleProgress
                    {
                        UserId = userId,
                        CourseModuleId = courseModule.Id,
                        IsCompleted = true,
                        CompletedAt = DateTime.UtcNow,
                        LastUpdated = DateTime.UtcNow
                    };
                    _context.ModuleProgress.Add(moduleProgress);
                }
                else if (!moduleProgress.IsCompleted)
                {
                    moduleProgress.IsCompleted = true;
                    moduleProgress.CompletedAt = DateTime.UtcNow;
                    moduleProgress.LastUpdated = DateTime.UtcNow;
                }
            }
            else
            {
                // If module not found, we still persist the submission, but we log (could be replaced with proper logging)
                // This prevents throwing an exception when client sends an index that doesn't match a module order.
                // Optionally return BadRequest here if that's desired.
            }

            await _context.SaveChangesAsync();

            // Recalculate progress (shared logic)
            await RecalculateProgress(userId, courseId);

            return Ok(new { url = fileUrl });
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
                .Where(p => p.UserId == userId && p.CourseModule.CourseId == model.CourseId)
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
    }
}