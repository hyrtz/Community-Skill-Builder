namespace SkillBuilder.Models
{
    public class CommunityPostReport
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public string ReporterId { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public string? Details { get; set; }
        public DateTime ReportedAt { get; set; }

        // Navigation property
        public CommunityPost Post { get; set; } = null!;
    }
}
