namespace SkillBuilder.Models.ViewModels
{
    public class CourseBuilderViewModel
    {
        public Course? Course { get; set; } = new();
        public IFormFile? ImageFile { get; set; }
        public IFormFile? VideoFile { get; set; }
        public IFormFile? ThumbnailFile { get; set; }
        public List<CourseModuleViewModel> Modules { get; set; } = new();
        public List<CourseMaterialViewModel> Materials { get; set; } = new();   
        public List<string> LearningObjectives { get; set; } = new();
    }
}
