namespace SkillBuilder.Models.ViewModels
{
    public class CourseProgressViewModel
    {
        public string UserId { get; set; }
        public int CourseId { get; set; }
        public string CourseTitle { get; set; }
        public string CourseDescription { get; set; }
        public double ProgressPercentage { get; set; }
        public string Link { get; set; }
        public int TotalLessons { get; set; }
        public bool IsPublished { get; set; }
    }
}
