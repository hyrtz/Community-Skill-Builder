using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillBuilder.Data;
using SkillBuilder.Models;

namespace SkillBuilder.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ModuleProgressController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ModuleProgressController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("CompleteModule")]
        public IActionResult CompleteModule([FromBody] ModuleProgressDto model)
        {
            try
            {
                var userId = User.FindFirst("UserId")?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                // Find the module by CourseId + ModuleIndex (if you store them like that)
                var courseModules = _context.CourseModules
                    .Where(cm => cm.CourseId == model.CourseId)
                    .Include(cm => cm.Contents)
                    .OrderBy(cm => cm.Order)
                    .ToList();

                if (model.ModuleIndex < 0 || model.ModuleIndex >= courseModules.Count)
                    return BadRequest("Invalid module index.");

                var courseModule = courseModules[model.ModuleIndex];
                if (courseModule == null)
                    return NotFound("Course module not found.");

                var lessonType = model.LessonType;

                if (string.IsNullOrEmpty(lessonType))
                    return BadRequest("Lesson type is required.");

                var existing = _context.ModuleProgress
                    .FirstOrDefault(mp => mp.UserId == userId && mp.CourseModuleId == courseModule.Id);

                bool isNewCompletion = false;

                if (existing == null)
                {
                    var progress = new ModuleProgress
                    {
                        UserId = userId,
                        CourseModuleId = courseModule.Id,
                        IsCompleted = true,
                        CompletedAt = DateTime.UtcNow
                    };
                    _context.ModuleProgress.Add(progress);
                    isNewCompletion = true;
                }
                else if (!existing.IsCompleted)
                {
                    existing.IsCompleted = true;
                    existing.CompletedAt = DateTime.UtcNow;
                    _context.ModuleProgress.Update(existing);
                    isNewCompletion = true;
                }

                if (isNewCompletion)
                {
                    var user = _context.Users.FirstOrDefault(u => u.Id == userId);
                    if (user == null)
                        return NotFound("User not found.");

                    int points = lessonType switch
                    {
                        "Text" => 10,
                        "Image + Text" => 15,
                        "Video" => 20,
                        "Quiz" => 80,
                        "Session" => 60,
                        "Activity" => 100,
                        _ => 5
                    };

                    user.Points += points;
                    _context.Users.Update(user);
                    _context.SaveChanges();
                }

                _context.SaveChanges();

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Server error: {ex.Message}\n{ex.StackTrace}");
            }
        }

        public class ModuleProgressDto
        {
            public int CourseId { get; set; }
            public int ModuleIndex { get; set; }
            public string LessonType { get; set; }
        }
    }
}
