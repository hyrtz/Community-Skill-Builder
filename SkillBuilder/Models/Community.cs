namespace SkillBuilder.Models
{
    public class Community
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? AvatarUrl { get; set; }
        public string? CoverImageUrl { get; set; }  // For the big top image
        public int MembersCount { get; set; }
        public DateTime CreatedAt { get; set; }

        // Relationships
        public string CreatorId { get; set; }  // FK to User
        public User Creator { get; set; }

        public List<CommunityJoinRequest> JoinRequests { get; set; } = new();
        public List<CommunityMembership> Memberships { get; set; } = new();
        public List<CommunityPost> Posts { get; set; } = new();

        public bool IsArchived { get; set; } = false;
        public bool IsPublished { get; set; } = true;
    }
}