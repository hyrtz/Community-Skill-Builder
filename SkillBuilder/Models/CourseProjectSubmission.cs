namespace SkillBuilder.Models
{
    public class CourseProjectSubmission
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; }

        public string ImageUrl { get; set; }

        public DateTime SubmittedAt { get; set; }
    }
}
