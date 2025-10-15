namespace SkillBuilder.Models.ViewModels
{
    public class CommunityMemberViewModel
    {
        public string UserId { get; set; } = "";
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string AvatarUrl { get; set; } = "";
        public string Role { get; set; } = "";
        public DateTime JoinedAt { get; set; }
        public int CommunityId { get; set; }
    }
}
