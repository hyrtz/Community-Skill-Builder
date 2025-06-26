namespace SkillBuilder.Models.ViewModels
{
    public class CourseCatalogViewModel
    {
        public List<Course> Courses { get; set; }
        public Course? SelectedCourse { get; set; }
    }
}