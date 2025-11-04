using System.Collections.Generic;

namespace SkillBuilder.Models.ViewModels
{
    public class CourseDetailsViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Link { get; set; }
        public string Description { get; set; }
        public List<CourseModuleViewModel> Modules { get; set; } = new();
        public string FinalProjectTitle { get; set; }
        public string FinalProjectDescription { get; set; }
    }
}