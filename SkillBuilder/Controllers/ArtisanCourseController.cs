﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillBuilder.Data;
using SkillBuilder.Models;
using SkillBuilder.Models.ViewModels;

namespace SkillBuilder.Controllers
{
    [Route("ArtisanActions")]
    public class ArtisanCourseController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ArtisanCourseController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
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
        public async Task<IActionResult> CreateCourse(CourseBuilderViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Actions/ArtisanActions/CreateCourse.cshtml", model); // ✅ Use the correct view path
            }

            var userId = User.FindFirst("UserId")?.Value;
            var artisan = await _context.Artisans.FirstOrDefaultAsync(a => a.UserId == userId);
            if (artisan == null) return Unauthorized();

            // Step 1: Save the Course
            var course = model.Course!;
            course.CreatedBy = artisan.ArtisanId;
            course.CreatedAt = DateTime.UtcNow;

            if (model.ImageFile != null)
                course.ImageUrl = await SaveFileAsync(model.ImageFile, "course-images");

            if (model.VideoFile != null)
                course.Video = await SaveFileAsync(model.VideoFile, "course-videos");

            if (model.ThumbnailFile != null)
                course.Thumbnail = await SaveFileAsync(model.ThumbnailFile, "course-thumbnails");

            course.Link = string.IsNullOrWhiteSpace(course.Link)
                ? System.Text.RegularExpressions.Regex.Replace(
                    course.Title.ToLower(), @"[^a-z0-9]+", "-"
                  ).Trim('-') + "-" + Guid.NewGuid().ToString("N")[..8]
                : course.Link;

            // Save WhatToLearn as joined string
            course.WhatToLearn = model.LearningObjectives != null
                ? string.Join("||", model.LearningObjectives.Where(o => !string.IsNullOrWhiteSpace(o)))
                : null;

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            // Step 2: Modules + Lessons
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
                    await _context.SaveChangesAsync(); // Get CourseModule.Id

                    for (int j = 0; j < moduleVm.Lessons.Count; j++)
                    {
                        var lesson = moduleVm.Lessons[j];
                        var moduleContent = new ModuleContent
                        {
                            CourseModuleId = courseModule.Id,
                            Title = lesson.Title,
                            ContentType = lesson.LessonType ?? "Text",
                            Order = j,
                            Duration = lesson.Duration,
                            ContentText = lesson.ContentText
                        };

                        // Uploads (image/video depending on type)
                        if (lesson.ImageFile != null)
                            moduleContent.MediaUrl = await SaveFileAsync(lesson.ImageFile, "lesson-images");

                        if (lesson.VideoFile != null)
                            moduleContent.MediaUrl = await SaveFileAsync(lesson.VideoFile, "lesson-videos");

                        _context.ModuleContents.Add(moduleContent);
                        await _context.SaveChangesAsync(); // Get ModuleContent.Id

                        // Quiz (if applicable)
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
                                    CorrectAnswer = q.CorrectAnswer
                                };
                                _context.QuizQuestions.Add(quiz);
                            }
                        }
                    }
                }
            }

            // Step 3: Materials (optional)
            if (model.Materials != null)
            {
                foreach (var mat in model.Materials)
                {
                    if (mat.UploadFile != null)
                    {
                        var filePath = await SaveFileAsync(mat.UploadFile, "course-materials");
                        var courseMaterial = new CourseMaterial
                        {
                            CourseId = course.Id,
                            Title = mat.Title,
                            Description = mat.Description,
                            FilePath = filePath,
                            FileSize = mat.UploadFile.Length
                        };
                        _context.CourseMaterials.Add(courseMaterial);
                    }
                }
            }

            await _context.SaveChangesAsync();

            var redirectUrl = Url.Action("ArtisanProfile", "ArtisanProfile", new { id = artisan.ArtisanId });
            if (string.IsNullOrEmpty(redirectUrl))
                return BadRequest("Redirect URL failed to generate.");

            return Redirect($"/ArtisanProfile/{artisan.ArtisanId}");
        }

        private async Task<string> SaveFileAsync(IFormFile file, string folderName)
        {
            var uploadsRoot = Path.Combine(_env.WebRootPath, "uploads", folderName);
            if (!Directory.Exists(uploadsRoot))
                Directory.CreateDirectory(uploadsRoot);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsRoot, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
                await file.CopyToAsync(stream);

            return $"/uploads/{folderName}/{fileName}";
        }
    }
}