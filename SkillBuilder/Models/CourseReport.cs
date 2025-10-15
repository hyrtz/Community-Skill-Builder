namespace SkillBuilder.Models
{
    public class CourseReport
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string ReporterId { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string? Details { get; set; }
        public DateTime ReportedAt { get; set; }

        // Navigation
        public Course Course { get; set; }
        public User Reporter { get; set; }
    }
}
