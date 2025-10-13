namespace SkillBuilder.Models
{
    public class CommunityJoinRequest
    {
        public int Id { get; set; }
        public int CommunityId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected
        public string? ShortMessage { get; set; }

        // Navigation properties
        public Community Community { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
