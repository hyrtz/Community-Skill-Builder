using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillBuilder.Data;
using SkillBuilder.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

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

        [HttpPost("AddPoints")]
        public async Task<IActionResult> AddPoints([FromBody] ModuleProgressDto model)
        {
            try
            {
                var userId = User.FindFirst("UserId")?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                    return NotFound("User not found.");

                // Determine points
                int points = model.LessonType switch
                {
                    "Text" => 10,
                    "Image + Text" => 15,
                    "Video" => 20,
                    "Quiz" => 80,
                    "Session" => 60,
                    "FinalProject" => 100,
                    _ => 5
                };

                // Check if module progress already exists
                if (model.ModuleIndex >= 0)
                {
                    var courseModules = await _context.CourseModules
                        .Where(cm => cm.CourseId == model.CourseId)
                        .OrderBy(cm => cm.Order)
                        .ToListAsync();

                    if (model.ModuleIndex >= courseModules.Count)
                        return BadRequest("Invalid module index.");

                    var courseModule = courseModules[model.ModuleIndex];

                    var existing = await _context.ModuleProgress
                        .FirstOrDefaultAsync(mp => mp.UserId == userId && mp.CourseModuleId == courseModule.Id);

                    if (existing == null)
                    {
                        _context.ModuleProgress.Add(new ModuleProgress
                        {
                            UserId = userId,
                            CourseModuleId = courseModule.Id,
                            IsCompleted = true,
                            CompletedAt = DateTime.UtcNow
                        });
                        user.Points += points;
                    }
                    else if (!existing.IsCompleted)
                    {
                        existing.IsCompleted = true;
                        existing.CompletedAt = DateTime.UtcNow;
                        _context.ModuleProgress.Update(existing);
                        user.Points += points;
                    }
                }
                else
                {
                    // Final Project
                    user.Points += points;
                }

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                return Ok(new { totalPoints = user.Points });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Server error: {ex.Message}");
            }
        }
    }

    public class ModuleProgressDto
    {
        public int CourseId { get; set; }
        public int ModuleIndex { get; set; } // use -1 for FinalProject
        public string LessonType { get; set; }
    }
}