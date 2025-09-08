namespace SkillBuilder.Models.ViewModels
{
    public class CourseBuilderViewModel
    {
        public Course? Course { get; set; } = new();
        public IFormFile? ImageFile { get; set; }
        public IFormFile? VideoFile { get; set; }
        public IFormFile? ThumbnailFile { get; set; }
        public List<string> LearningObjectives { get; set; } = new List<string> { "" };
       public List<ArtisanWorkViewModel> ArtisanWorks { get; set; } = new();
        public List<CourseModuleViewModel> Modules { get; set; } = new List<CourseModuleViewModel>();
        public List<CourseMaterialViewModel> Materials { get; set; } = new List<CourseMaterialViewModel>();
    }
}
