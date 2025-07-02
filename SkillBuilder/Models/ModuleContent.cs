using System.ComponentModel.DataAnnotations;

namespace SkillBuilder.Models
{
    public class ModuleContent
    {
        public int Id { get; set; }

        public int CourseModuleId { get; set; }
        public CourseModule CourseModule { get; set; } = null!;

        [Required]
        public string ContentType { get; set; } = string.Empty;

        public string? Title { get; set; }
        public string? ContentText { get; set; }
        public string? MediaUrl { get; set; }
        public string? Duration { get; set; }
        public string? SessionLink { get; set; }

        public int Order { get; set; }
        public ICollection<QuizQuestion> QuizQuestions { get; set; } = new List<QuizQuestion>();
    }
}