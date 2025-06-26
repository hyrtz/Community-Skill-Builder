using Microsoft.AspNetCore.Mvc;
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
            var userId = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            // Find the module by CourseId + ModuleIndex (if you store them like that)
            var courseModules = _context.CourseModules
                .Where(cm => cm.CourseId == model.CourseId)
                .OrderBy(cm => cm.Order) // assuming you have this
                .ToList();

            if (model.ModuleIndex >= courseModules.Count)
                return BadRequest("Invalid module index.");

            var courseModule = courseModules[model.ModuleIndex];

            var existing = _context.ModuleProgress
                .FirstOrDefault(mp => mp.UserId == userId && mp.CourseModuleId == courseModule.Id);

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
            }
            else if (!existing.IsCompleted)
            {
                existing.IsCompleted = true;
                existing.CompletedAt = DateTime.UtcNow;
                _context.ModuleProgress.Update(existing);
            }

            _context.SaveChanges();

            return Ok();
        }

        public class ModuleProgressDto
        {
            public int CourseId { get; set; }
            public int ModuleIndex { get; set; } // position in the moduleData array
        }
    }
}
