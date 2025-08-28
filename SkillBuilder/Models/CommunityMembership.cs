namespace SkillBuilder.Models
{
    public class CommunityMembership
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }

        public int CommunityId { get; set; }
        public Community Community { get; set; }
        public string Role { get; set; }

        public DateTime JoinedAt { get; set; } = DateTime.Now;
    }
}
