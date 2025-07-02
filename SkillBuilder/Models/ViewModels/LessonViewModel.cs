namespace SkillBuilder.Models.ViewModels
{
    public class LessonViewModel
    {
        public string Title { get; set; }
        public string LessonType { get; set; }
        public string? Duration { get; set; }
        public string? ContentText { get; set; }
        public IFormFile? ImageFile { get; set; }
        public IFormFile? VideoFile { get; set; }
        public string? SessionLink { get; set; }
        public List<QuizQuestionViewModel>? QuizQuestions { get; set; }
    }
}
