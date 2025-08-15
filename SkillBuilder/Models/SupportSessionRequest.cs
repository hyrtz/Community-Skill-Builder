namespace SkillBuilder.Models
{
    public class SupportSessionRequest
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; }

        public string Title { get; set; }
        public string Message { get; set; }

        public DateTime SessionDate { get; set; }
        public TimeSpan SessionTime { get; set; }

        public string Status { get; set; } = "Pending";
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string? MeetingLink { get; set; }
        public string? MeetingPlatform { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
