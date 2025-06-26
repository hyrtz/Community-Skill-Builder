namespace SkillBuilder.Models.ViewModels
{
    public class CourseModuleViewModel
    {
        public string Title { get; set; }
        public List<ModuleContentViewModel> Contents { get; set; } = new();
    }
}
