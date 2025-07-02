using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillBuilder.Data;
using SkillBuilder.Models;
using SkillBuilder.Models.ViewModels;
using System.IO;

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

        private string GetCorrectAnswerText(int index, string a, string b, string c, string d)
        {
            return index switch
            {
                1 => a,
                2 => b,
                3 => c,
                4 => d,
                _ => ""
            };
        }

        [HttpGet("CreateCourse")]
        public IActionResult CreateCourse()
        {
            return View("~/Views/Actions/ArtisanActions/CreateCourse.cshtml", new CourseBuilderViewModel());
        }

        [HttpPost("CreateCourse")]
        public async Task<IActionResult> CreateCourse(CourseBuilderViewModel model)
        {
            Console.WriteLine("🔥 POST method hit");

            if (!ModelState.IsValid)
            {
                foreach (var entry in ModelState)
                {
                    if (entry.Value.Errors.Count > 0)
                        Console.WriteLine($"{entry.Key}: {entry.Value.Errors[0].ErrorMessage}");
                }

                return View("~/Views/Actions/ArtisanActions/CreateCourse.cshtml", model);
            }

            var artisanId = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(artisanId))
                return Unauthorized();

            var course = model.Course;
            course.Link = string.IsNullOrWhiteSpace(course.Link)
                ? course.Title.Replace(" ", "-").ToLower() + "-" + Guid.NewGuid().ToString().Substring(0, 8)
                : course.Link;
            course.WhatToLearn = model.LearningObjectives != null
                ? string.Join("||", model.LearningObjectives.Where(o => !string.IsNullOrWhiteSpace(o)))
                : null;
            course.CreatedBy = artisanId;
            course.CreatedAt = DateTime.UtcNow;

            // Upload Image
            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                var fileName = Path.GetFileName(model.ImageFile.FileName);
                var imagePath = Path.Combine(_env.WebRootPath, "uploads/images", fileName);
                using var stream = new FileStream(imagePath, FileMode.Create);
                await model.ImageFile.CopyToAsync(stream);
                course.ImageUrl = "/uploads/images/" + fileName;
            }

            // Upload Video
            if (model.VideoFile != null && model.VideoFile.Length > 0)
            {
                var fileName = Path.GetFileName(model.VideoFile.FileName);
                var videoPath = Path.Combine(_env.WebRootPath, "uploads/videos", fileName);
                using var stream = new FileStream(videoPath, FileMode.Create);
                await model.VideoFile.CopyToAsync(stream);
                course.Video = "/uploads/videos/" + fileName;
            }

            // Upload Thumbnail
            if (model.ThumbnailFile != null && model.ThumbnailFile.Length > 0)
            {
                var fileName = Path.GetFileName(model.ThumbnailFile.FileName);
                var thumbPath = Path.Combine(_env.WebRootPath, "uploads/thumbnails", fileName);
                using var stream = new FileStream(thumbPath, FileMode.Create);
                await model.ThumbnailFile.CopyToAsync(stream);
                course.Thumbnail = "/uploads/thumbnails/" + fileName;
            }

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            // Save Modules
            if (model.Modules != null)
            {
                foreach (var moduleVm in model.Modules)
                {
                    var courseModule = new CourseModule
                    {
                        CourseId = course.Id,
                        Title = moduleVm.Title ?? string.Empty,
                        Order = model.Modules.IndexOf(moduleVm)
                    };

                    _context.CourseModules.Add(courseModule);
                    await _context.SaveChangesAsync();

                    if (moduleVm.Lessons != null)
                    {
                        foreach (var lesson in moduleVm.Lessons)
                        {
                            var moduleContent = new ModuleContent
                            {
                                CourseModuleId = courseModule.Id,
                                Title = lesson.Title ?? string.Empty,
                                ContentType = lesson.LessonType ?? "Text",
                                Order = moduleVm.Lessons.IndexOf(lesson),
                                Duration = lesson.Duration,
                                SessionLink = lesson.SessionLink,
                                MediaUrl = null
                            };

                            // Upload Image or Video based on type
                            var fileKeyPrefix = $"Modules[{model.Modules.IndexOf(moduleVm)}].Lessons[{moduleVm.Lessons.IndexOf(lesson)}]";

                            if (lesson.LessonType == "Image + Text")
                            {
                                var imageFile = Request.Form.Files.FirstOrDefault(f => f.Name == $"{fileKeyPrefix}.ImageFile");
                                if (imageFile != null && imageFile.Length > 0)
                                {
                                    var imageName = Path.GetFileName(imageFile.FileName);
                                    var imagePath = Path.Combine(_env.WebRootPath, "uploads/lesson-images", imageName);
                                    using var stream = new FileStream(imagePath, FileMode.Create);
                                    await imageFile.CopyToAsync(stream);
                                    moduleContent.MediaUrl = "/uploads/lesson-images/" + imageName;
                                }
                            }

                            if (lesson.LessonType == "Video")
                            {
                                var videoFile = Request.Form.Files.FirstOrDefault(f => f.Name == $"{fileKeyPrefix}.VideoFile");
                                if (videoFile != null && videoFile.Length > 0)
                                {
                                    var videoName = Path.GetFileName(videoFile.FileName);
                                    var videoPath = Path.Combine(_env.WebRootPath, "uploads/lesson-videos", videoName);
                                    using var stream = new FileStream(videoPath, FileMode.Create);
                                    await videoFile.CopyToAsync(stream);
                                    moduleContent.MediaUrl = "/uploads/lesson-videos/" + videoName;
                                }
                            }

                            _context.ModuleContents.Add(moduleContent);
                            await _context.SaveChangesAsync();

                            // Save quiz if present
                            if (lesson.LessonType == "Quiz" && lesson.QuizQuestions != null)
                            {
                                foreach (var quizVm in lesson.QuizQuestions)
                                {
                                    var correctAnswerText = GetCorrectAnswerText(
                                        quizVm.CorrectIndex,
                                        quizVm.OptionA,
                                        quizVm.OptionB,
                                        quizVm.OptionC,
                                        quizVm.OptionD
                                    );

                                    var quiz = new QuizQuestion
                                    {
                                        ModuleContentId = moduleContent.Id,
                                        Question = quizVm.QuestionText,
                                        OptionA = quizVm.OptionA,
                                        OptionB = quizVm.OptionB,
                                        OptionC = quizVm.OptionC,
                                        OptionD = quizVm.OptionD,
                                        CorrectAnswer = correctAnswerText
                                    };

                                    _context.QuizQuestions.Add(quiz);
                                }
                                await _context.SaveChangesAsync();
                            }
                        }
                    }
                }
            }

            // Save Materials
            if (model.Materials != null)
            {
                foreach (var materialVm in model.Materials)
                {
                    string? fileName = null;
                    string? filePath = null;
                    long fileSize = 0;

                    if (materialVm.UploadFile != null && materialVm.UploadFile.Length > 0)
                    {
                        fileName = Path.GetFileName(materialVm.UploadFile.FileName);
                        var savePath = Path.Combine(_env.WebRootPath, "uploads/materials", fileName);

                        using var stream = new FileStream(savePath, FileMode.Create);
                        await materialVm.UploadFile.CopyToAsync(stream);

                        filePath = "/uploads/materials/" + fileName;
                        fileSize = materialVm.UploadFile.Length;
                    }

                    var material = new CourseMaterial
                    {
                        CourseId = course.Id,
                        Title = materialVm.Title ?? "Untitled",
                        FileName = fileName,
                        FilePath = filePath,
                        FileSize = fileSize,
                        Description = materialVm.Description
                    };

                    _context.CourseMaterials.Add(material);
                }

                await _context.SaveChangesAsync();
            }

            var redirectUrl = Url.Action("ArtisanProfile", "ArtisanProfile", new { id = artisanId });
            if (redirectUrl == null)
                return BadRequest("Redirect URL failed to generate.");

            return Redirect(redirectUrl);
        }
    }
}