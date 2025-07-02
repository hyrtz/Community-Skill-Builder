using System.ComponentModel.DataAnnotations;

namespace SkillBuilder.Models
{
    public class QuizQuestion
    {
        public int Id { get; set; }

        [Required]
        public string Question { get; set; } = string.Empty;
        public string OptionA { get; set; }
        public string OptionB { get; set; }
        public string OptionC { get; set; }
        public string OptionD { get; set; }
        public string CorrectAnswer { get; set; }

        public int ModuleContentId { get; set; }
        public ModuleContent ModuleContent { get; set; } = null!;
    }
}
