using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using SkillBuilder.Data;
using SkillBuilder.Models;
using SkillBuilder.Models.ViewModels;
using SkillBuilder.Services;

namespace SkillBuilder.Controllers
{
    [Route("ArtisanActions")]
    [Authorize(Roles = "Artisan")]
    public class ArtisanCourseController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly INotificationService _notificationService;

        public ArtisanCourseController(AppDbContext context, IWebHostEnvironment env, INotificationService notificationService)
        {
            _context = context;
            _env = env;
            _notificationService = notificationService;
        }

        [HttpGet("CreateCourse")]
        public IActionResult CreateCourse()
        {
            var viewModel = new CourseBuilderViewModel
            {
                Course = new Course(),
                LearningObjectives = new List<string> { "" },
                Modules = new List<CourseModuleViewModel>(),
                Materials = new List<CourseMaterialViewModel>()
            };

            return View("~/Views/Actions/ArtisanActions/CreateCourse.cshtml", viewModel);
        }

        [HttpPost("CreateCourse")]
        [RequestSizeLimit(200 * 1024 * 1024)] // Allow up to 200 MB for this action
        public async Task<IActionResult> CreateCourse(CourseBuilderViewModel model)
        {
            if (!ModelState.IsValid)
                return View("~/Views/Actions/ArtisanActions/CreateCourse.cshtml", model);

            var userId = User.FindFirst("UserId")?.Value;
            var artisan = await _context.Artisans.FirstOrDefaultAsync(a => a.UserId == userId);
            if (artisan == null) return Unauthorized();

            var course = model.Course!;
            course.CreatedBy = artisan.ArtisanId;
            course.CreatedAt = DateTime.UtcNow;
            course.Duration = $"{model.DurationValue} {model.DurationUnit}";

            // ✅ File Uploads with Validation
            if (model.ImageFile != null)
            {
                if (model.ImageFile.Length > 5 * 1024 * 1024) // 5 MB
                {
                    ModelState.AddModelError("ImageFile", "Image file must be under 5 MB.");
                    return View("~/Views/Actions/ArtisanActions/CreateCourse.cshtml", model);
                }
                course.ImageUrl = await SaveFileAsync(model.ImageFile, "course-images");
            }

            if (model.VideoFile != null)
            {
                if (model.VideoFile.Length > 200 * 1024 * 1024) // 200 MB
                {
                    ModelState.AddModelError("VideoFile", "Video file must be under 200 MB.");
                    return View("~/Views/Actions/ArtisanActions/CreateCourse.cshtml", model);
                }
                course.Video = await SaveFileAsync(model.VideoFile, "course-videos");
            }

            if (model.ThumbnailFile != null)
            {
                if (model.ThumbnailFile.Length > 5 * 1024 * 1024) // 5 MB
                {
                    ModelState.AddModelError("ThumbnailFile", "Thumbnail must be under 5 MB.");
                    return View("~/Views/Actions/ArtisanActions/CreateCourse.cshtml", model);
                }
                course.Thumbnail = await SaveFileAsync(model.ThumbnailFile, "course-thumbnails");
            }

            // Generate course link if missing
            course.Link = string.IsNullOrWhiteSpace(course.Link)
                ? System.Text.RegularExpressions.Regex.Replace(
                    course.Title.ToLower(), @"[^a-z0-9]+", "-"
                  ).Trim('-') + "-" + Guid.NewGuid().ToString("N")[..8]
                : course.Link;

            // Learning objectives
            course.WhatToLearn = model.LearningObjectives != null
                ? string.Join("||", model.LearningObjectives.Where(o => !string.IsNullOrWhiteSpace(o)))
                : null;

            if (model.FinalProject != null)
            {
                course.FinalProjectTitle = model.FinalProject.Title;
                course.FinalProjectDescription = model.FinalProject.Description;
            }

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            // ✅ Modules
            if (model.Modules != null)
            {
                for (int i = 0; i < model.Modules.Count; i++)
                {
                    var moduleVm = model.Modules[i];
                    var courseModule = new CourseModule
                    {
                        CourseId = course.Id,
                        Title = moduleVm.Title,
                        Order = i
                    };

                    _context.CourseModules.Add(courseModule);
                    await _context.SaveChangesAsync();

                    for (int j = 0; j < moduleVm.Lessons.Count; j++)
                    {
                        var lesson = moduleVm.Lessons[j];
                        var moduleContent = new ModuleContent
                        {
                            CourseModuleId = courseModule.Id,
                            Title = lesson.Title,
                            ContentType = lesson.LessonType ?? "Text",
                            Order = j,
                            Duration = $"{lesson.DurationValue} {lesson.DurationUnit}",
                            ContentText = lesson.ContentText
                        };

                        if (lesson.VideoFile != null)
                            moduleContent.MediaUrl = await SaveFileAsync(lesson.VideoFile, "lesson-videos");
                        else if (lesson.ImageFile != null)
                            moduleContent.MediaUrl = await SaveFileAsync(lesson.ImageFile, "lesson-images");

                        _context.ModuleContents.Add(moduleContent);
                        await _context.SaveChangesAsync();

                        if (lesson.LessonType == "Quiz" && lesson.QuizQuestions.Any())
                        {
                            foreach (var q in lesson.QuizQuestions)
                            {
                                var quiz = new QuizQuestion
                                {
                                    ModuleContentId = moduleContent.Id,
                                    Question = q.QuestionText,
                                    OptionA = q.OptionA,
                                    OptionB = q.OptionB,
                                    OptionC = q.OptionC,
                                    OptionD = q.OptionD,
                                    CorrectAnswer = q.CorrectAnswer ?? ""
                                };
                                _context.QuizQuestions.Add(quiz);
                            }

                            await _context.SaveChangesAsync();
                        }
                    }
                }
            }

            // ✅ Materials
            if (model.Materials != null)
            {
                foreach (var mat in model.Materials)
                {
                    if (mat.UploadFile != null && mat.UploadFile.Length > 0)
                    {
                        var filePath = await SaveFileAsync(mat.UploadFile, "course-materials");

                        var courseMaterial = new CourseMaterial
                        {
                            CourseId = course.Id,
                            Title = mat.Title,
                            Description = mat.Description,
                            FilePath = filePath,
                            FileName = mat.UploadFile.FileName,
                            FileSize = mat.UploadFile.Length
                        };
                        _context.CourseMaterials.Add(courseMaterial);
                    }
                }
            }

            await _context.SaveChangesAsync();

            await _notificationService.AddNotificationAsync(
                artisan.UserId,
                $"✅ Your course '{course.Title}' has been successfully created!"
            );

            // ✅ Notify Admin(s)
            var adminUsers = await _context.Users
                .Where(u => u.Role == "Admin")
                .ToListAsync();

            foreach (var admin in adminUsers)
            {
                await _notificationService.AddNotificationAsync(
                    admin.Id,
                    $"📢 New course created: '{course.Title}' by {artisan.FirstName} {artisan.LastName}."
                );
            }

            return Redirect($"/ArtisanProfile/{artisan.ArtisanId}");
        }

        private async Task<string> SaveFileAsync(IFormFile file, string folderName)
        {
            if (file == null || file.Length == 0)
                throw new InvalidOperationException("File is empty or missing.");

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            // ✅ File type and size validation (added .webp)
            if (extension is ".jpg" or ".jpeg" or ".png" or ".gif" or ".webp")
            {
                if (file.Length > 5 * 1024 * 1024)
                    throw new InvalidOperationException("Image files must be under 5 MB.");
            }
            else if (extension is ".pdf" or ".docx" or ".txt")
            {
                if (file.Length > 10 * 1024 * 1024)
                    throw new InvalidOperationException("Document files must be under 10 MB.");
            }
            else if (extension is ".mp4" or ".mov" or ".avi")
            {
                if (file.Length > 200 * 1024 * 1024)
                    throw new InvalidOperationException("Video files must be under 200 MB.");
            }
            else
            {
                throw new InvalidOperationException("Unsupported file type.");
            }

            // ✅ Save file to uploads folder
            var uploadsRoot = Path.Combine(_env.WebRootPath, "uploads", folderName);
            if (!Directory.Exists(uploadsRoot))
                Directory.CreateDirectory(uploadsRoot);

            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsRoot, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
                await file.CopyToAsync(stream);

            return $"/uploads/{folderName}/{fileName}";
        }

        [HttpGet("EditCourse/{courseId}")]
        public async Task<IActionResult> EditCourse(int courseId)
        {
            var userId = User.FindFirst("UserId")?.Value;
            var artisan = await _context.Artisans.FirstOrDefaultAsync(a => a.UserId == userId);
            if (artisan == null) return Unauthorized();

            var course = await _context.Courses
                .Include(c => c.Materials)
                .Include(c => c.CourseModules)
                .ThenInclude(m => m.Contents)
                .ThenInclude(mc => mc.QuizQuestions)
                .FirstOrDefaultAsync(c => c.Id == courseId && c.CreatedBy == artisan.ArtisanId);

            if (course == null) return NotFound();

            var viewModel = new CourseBuilderViewModel
            {
                Course = course,
                DurationValue = int.TryParse(course.Duration?.Split(' ')[0], out var val) ? val : 0,
                DurationUnit = course.Duration?.Split(' ').ElementAtOrDefault(1) ?? "hours",
                LearningObjectives = course.WhatToLearn?.Split("||").ToList() ?? new List<string> { "" },
                Modules = course.CourseModules
                    .OrderBy(m => m.Order)
                    .Select(m => new CourseModuleViewModel
                    {
                        Title = m.Title,
                        Order = m.Order,
                        Lessons = m.Contents
                            .OrderBy(c => c.Order)
                            .Select(l =>
                            {
                                int durationValue = 0;
                                string durationUnit = "minutes";

                                if (!string.IsNullOrWhiteSpace(l.Duration))
                                {
                                    var parts = l.Duration.Split(' ');
                                    int.TryParse(parts[0], out durationValue);
                                    durationUnit = parts.Length > 1 ? parts[1] : "minutes";
                                }

                                return new LessonViewModel
                                {
                                    Title = l.Title ?? "",
                                    LessonType = l.ContentType ?? "",
                                    DurationValue = durationValue,
                                    DurationUnit = durationUnit,
                                    ContentText = l.ContentText ?? "",
                                    ImageFile = null,
                                    VideoFile = null,
                                    ImageUrl = l.ContentType == "Image + Text" ? l.MediaUrl : null,
                                    VideoUrl = l.ContentType == "Video + Text" ? l.MediaUrl : null,
                                    QuizQuestions = l.QuizQuestions
                                        .Select(q => new QuizQuestionViewModel
                                        {
                                            Id = q.Id,
                                            QuestionText = q.Question,
                                            OptionA = q.OptionA,
                                            OptionB = q.OptionB,
                                            OptionC = q.OptionC,
                                            OptionD = q.OptionD,
                                            CorrectAnswer = q.CorrectAnswer
                                        }).ToList()
                                };
                            }).ToList()
                    }).ToList(),
                Materials = course.Materials
                    .Select(mat => new CourseMaterialViewModel
                    {
                        Id = mat.Id,
                        Title = mat.Title,
                        FileName = mat.FileName,
                        FilePath = mat.FilePath,
                        FileSize = mat.FileSize,
                        Description = mat.Description
                    }).ToList(),
                FinalProject = new FinalProjectViewModel
                {
                    Title = course.FinalProjectTitle,
                    Description = course.FinalProjectDescription
                },
            };

            return View("~/Views/Actions/ArtisanActions/EditCourse.cshtml", viewModel);
        }

        [HttpPost("EditCourse")]
        public async Task<IActionResult> EditCourse(int courseId, CourseBuilderViewModel model)
        {
            if (!ModelState.IsValid)
                return View("~/Views/Actions/ArtisanActions/EditCourse.cshtml", model);

            var userId = User.FindFirst("UserId")?.Value;
            var artisan = await _context.Artisans.FirstOrDefaultAsync(a => a.UserId == userId);
            if (artisan == null) return Unauthorized();

            var course = await _context.Courses
                .Include(c => c.Materials)
                .Include(c => c.CourseModules)
                .ThenInclude(m => m.Contents)
                .ThenInclude(l => l.QuizQuestions)
                .FirstOrDefaultAsync(c => c.Id == courseId && c.CreatedBy == artisan.ArtisanId);

            if (course == null) return NotFound();

            // ------------------ Update Basic Course Info ------------------
            course.Title = model.Course.Title;
            course.Category = model.Course.Category;
            course.Overview = model.Course.Overview;
            course.Difficulty = model.Course.Difficulty;
            course.Duration = $"{model.DurationValue} {model.DurationUnit}";
            course.Requirements = model.Course.Requirements;
            course.WhatToLearn = model.LearningObjectives != null
                ? string.Join("||", model.LearningObjectives.Where(o => !string.IsNullOrWhiteSpace(o)))
                : null;
            course.FullDescription = model.Course.FullDescription;

            // Update media
            if (model.ImageFile != null)
                course.ImageUrl = await SaveFileAsync(model.ImageFile, "course-images");
            if (model.VideoFile != null)
                course.Video = await SaveFileAsync(model.VideoFile, "course-videos");
            if (model.ThumbnailFile != null)
                course.Thumbnail = await SaveFileAsync(model.ThumbnailFile, "course-thumbnails");

            // ------------------ Update Final Project ------------------
            if (model.FinalProject != null)
            {
                course.FinalProjectTitle = model.FinalProject.Title;
                course.FinalProjectDescription = model.FinalProject.Description;
            }

            _context.Courses.Update(course);

            // ------------------ Update Modules and Lessons ------------------
            if (model.Modules != null)
            {
                var formModuleIds = model.Modules.Select(m => m.Id).ToList();

                // Remove deleted modules
                var modulesToDelete = course.CourseModules
                    .Where(m => !formModuleIds.Contains(m.Id))
                    .ToList();
                if (modulesToDelete.Any()) _context.CourseModules.RemoveRange(modulesToDelete);

                for (int i = 0; i < model.Modules.Count; i++)
                {
                    var moduleVm = model.Modules[i];
                    CourseModule courseModule;

                    // Existing module
                    if (moduleVm.Id > 0)
                    {
                        courseModule = course.CourseModules.First(m => m.Id == moduleVm.Id);
                        courseModule.Title = moduleVm.Title;
                        courseModule.Order = i;
                        _context.CourseModules.Update(courseModule);
                    }
                    else
                    {
                        // New module
                        courseModule = new CourseModule
                        {
                            CourseId = course.Id,
                            Title = moduleVm.Title,
                            Order = i
                        };
                        _context.CourseModules.Add(courseModule);
                        await _context.SaveChangesAsync(); // Need ID for lessons
                    }

                    // ------------------ Lessons ------------------
                    if (moduleVm.Lessons != null)
                    {
                        var lessonIds = moduleVm.Lessons.Select(l => l.Id).ToList();
                        var lessonsToDelete = courseModule.Contents
                            .Where(l => !lessonIds.Contains(l.Id))
                            .ToList();
                        if (lessonsToDelete.Any()) _context.ModuleContents.RemoveRange(lessonsToDelete);

                        for (int j = 0; j < moduleVm.Lessons.Count; j++)
                        {
                            var lessonVm = moduleVm.Lessons[j];
                            ModuleContent lesson;

                            // Existing lesson
                            if (lessonVm.Id > 0)
                            {
                                lesson = courseModule.Contents.First(l => l.Id == lessonVm.Id);
                                lesson.Title = lessonVm.Title;
                                lesson.ContentType = lessonVm.LessonType ?? "Text";
                                lesson.Duration = $"{lessonVm.DurationValue} {lessonVm.DurationUnit}";
                                lesson.ContentText = lessonVm.ContentText;

                                // --------- Media handling ---------
                                if (lessonVm.LessonType == "Image + Text")
                                {
                                    if (lessonVm.ImageFile != null)
                                    {
                                        lesson.MediaUrl = await SaveFileAsync(lessonVm.ImageFile, "lesson-media");
                                    }
                                    else if (!string.IsNullOrEmpty(lessonVm.ExistingImageUrl))
                                    {
                                        lesson.MediaUrl = lessonVm.ExistingImageUrl; // preserve old
                                    }
                                }

                                if (lessonVm.LessonType == "Video + Text")
                                {
                                    if (lessonVm.VideoFile != null)
                                    {
                                        lesson.MediaUrl = await SaveFileAsync(lessonVm.VideoFile, "lesson-media");
                                    }
                                    else if (!string.IsNullOrEmpty(lessonVm.ExistingVideoUrl))
                                    {
                                        lesson.MediaUrl = lessonVm.ExistingVideoUrl; // preserve old
                                    }
                                }
                                // Text/Quiz/Session: no MediaUrl needed

                                _context.ModuleContents.Update(lesson);
                            }
                            else
                            {
                                // New lesson
                                string mediaUrl = null;
                                if (lessonVm.LessonType == "Image + Text" && lessonVm.ImageFile != null)
                                    mediaUrl = await SaveFileAsync(lessonVm.ImageFile, "lesson-media");
                                else if (lessonVm.LessonType == "Video + Text" && lessonVm.VideoFile != null)
                                    mediaUrl = await SaveFileAsync(lessonVm.VideoFile, "lesson-media");

                                lesson = new ModuleContent
                                {
                                    CourseModuleId = courseModule.Id,
                                    Title = lessonVm.Title,
                                    ContentType = lessonVm.LessonType ?? "Text",
                                    Duration = $"{lessonVm.DurationValue} {lessonVm.DurationUnit}",
                                    ContentText = lessonVm.ContentText,
                                    MediaUrl = mediaUrl
                                };
                                _context.ModuleContents.Add(lesson);
                                await _context.SaveChangesAsync();
                            }

                            // ------------------ Quiz Questions ------------------
                            if (lessonVm.LessonType == "Quiz")
                            {
                                lesson.QuizQuestions ??= new List<QuizQuestion>();

                                var quizIds = lessonVm.QuizQuestions?.Select(q => q.Id).ToList() ?? new List<int>();
                                var quizzesToDelete = lesson.QuizQuestions
                                    .Where(q => !quizIds.Contains(q.Id))
                                    .ToList();
                                if (quizzesToDelete.Any()) _context.QuizQuestions.RemoveRange(quizzesToDelete);

                                for (int qIndex = 0; qIndex < (lessonVm.QuizQuestions?.Count ?? 0); qIndex++)
                                {
                                    var qVm = lessonVm.QuizQuestions[qIndex];
                                    QuizQuestion quiz;

                                    if (qVm.Id > 0)
                                    {
                                        quiz = lesson.QuizQuestions.First(q => q.Id == qVm.Id);
                                        quiz.Question = qVm.QuestionText;
                                        quiz.OptionA = qVm.OptionA;
                                        quiz.OptionB = qVm.OptionB;
                                        quiz.OptionC = qVm.OptionC;
                                        quiz.OptionD = qVm.OptionD;
                                        quiz.CorrectAnswer = qVm.CorrectAnswer;
                                        _context.QuizQuestions.Update(quiz);
                                    }
                                    else
                                    {
                                        quiz = new QuizQuestion
                                        {
                                            ModuleContentId = lesson.Id,
                                            Question = qVm.QuestionText,
                                            OptionA = qVm.OptionA,
                                            OptionB = qVm.OptionB,
                                            OptionC = qVm.OptionC,
                                            OptionD = qVm.OptionD,
                                            CorrectAnswer = qVm.CorrectAnswer
                                        };
                                        _context.QuizQuestions.Add(quiz);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (model.MaterialsToDelete != null && model.MaterialsToDelete.Any())
            {
                var materialsToDelete = course.Materials
                    .Where(m => model.MaterialsToDelete.Contains(m.Id) && m.Id > 0)
                    .ToList();
                if (materialsToDelete.Any())
                {
                    // Delete actual files
                    foreach (var mat in materialsToDelete)
                    {
                        if (!string.IsNullOrEmpty(mat.FilePath))
                        {
                            var path = Path.Combine(_env.WebRootPath, mat.FilePath);
                            if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
                        }
                    }

                    _context.CourseMaterials.RemoveRange(materialsToDelete);
                }
            }

            if (model.Materials != null && model.Materials.Any())
            {
                foreach (var matVm in model.Materials)
                {
                    CourseMaterial material;

                    if (matVm.Id > 0)
                    {
                        material = course.Materials.First(m => m.Id == matVm.Id);
                        material.Title = matVm.Title;
                        material.Description = matVm.Description;

                        if (matVm.UploadFile != null)
                        {
                            // Delete old file if exists
                            if (!string.IsNullOrEmpty(material.FilePath))
                            {
                                var oldPath = Path.Combine(_env.WebRootPath, material.FilePath);
                                if (System.IO.File.Exists(oldPath))
                                    System.IO.File.Delete(oldPath);
                            }

                            var filePath = await SaveFileAsync(matVm.UploadFile, "course-materials");
                            material.FilePath = filePath;
                            material.FileName = matVm.UploadFile.FileName;
                            material.FileSize = matVm.UploadFile.Length;
                        }

                        _context.CourseMaterials.Update(material);
                    }
                    else if (matVm.UploadFile != null)
                    {
                        // New material
                        var filePath = await SaveFileAsync(matVm.UploadFile, "course-materials");
                        material = new CourseMaterial
                        {
                            CourseId = course.Id,
                            Title = matVm.Title,
                            Description = matVm.Description,
                            FileName = matVm.UploadFile.FileName,
                            FileSize = matVm.UploadFile.Length,
                            FilePath = filePath
                        };
                        _context.CourseMaterials.Add(material);
                    }
                }
            }

            await _context.SaveChangesAsync();

            await _notificationService.AddNotificationAsync(
                artisan.UserId,
                $"✏️ Your course '{course.Title}' has been successfully updated!"
            );

            return RedirectToAction("ArtisanProfile", "ArtisanProfile", new { id = artisan.ArtisanId });
        }
    }
}