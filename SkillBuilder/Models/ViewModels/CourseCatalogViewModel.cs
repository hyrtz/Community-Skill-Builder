namespace SkillBuilder.Models.ViewModels
{
    public class CourseCatalogViewModel
    {
        public List<Course> Courses { get; set; }
        public List<Course> RecommendedCourses { get; set; } = new();
        public Course? SelectedCourse { get; set; }
    }
}