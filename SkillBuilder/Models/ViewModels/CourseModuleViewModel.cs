namespace SkillBuilder.Models.ViewModels
{
    public class CourseModuleViewModel
    {
        public string Title { get; set; }
        public int Order { get; set; }
        public List<LessonViewModel> Lessons { get; set; } = new();
    }
}