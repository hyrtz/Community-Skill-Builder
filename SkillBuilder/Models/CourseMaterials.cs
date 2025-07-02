using System.ComponentModel.DataAnnotations;

namespace SkillBuilder.Models
{
    public class CourseMaterial
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;

        [Required]
        public string Title { get; set; } = string.Empty;

        public string? FileName { get; set; }
        public string? FilePath { get; set; }
        public string? Description { get; set; }

        public long FileSize { get; set; }
    }
}