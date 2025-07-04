namespace SkillBuilder.Models.ViewModels
{
    public class LessonViewModel
    {
        public string Title { get; set; } = string.Empty;
        public string LessonType { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public string ContentText { get; set; } = string.Empty;
        public IFormFile? ImageFile { get; set; }
        public IFormFile? VideoFile { get; set; }
        public List<QuizQuestionViewModel> QuizQuestions { get; set; } = new();
    }
}
