namespace SkillBuilder.Models
{
    public class CourseProjectSubmission
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; }

        public string Title { get; set; }
        public string Description { get; set; } 
        public string MediaUrl { get; set; }

        public string Status { get; set; } = "Pending";
        public DateTime SubmittedAt { get; set; }
        public string? SignatureUrl { get; set; }
    }
}
