namespace SkillBuilder.Models.ViewModels
{
    public class CourseCreationViewModel
    {
        public Course Course { get; set; }

        public List<CourseModuleViewModel> Modules { get; set; } = new();
    }
}
