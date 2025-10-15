namespace SkillBuilder.Models.ViewModels
{
    public class ArtisanProfileViewModel
    {
        public Artisan Artisan { get; set; } = new();
        public List<Course> Courses { get; set; } = new();
        public List<ArtisanWork> ArtisanWorks { get; set; } = new();
        public List<SupportSessionRequest> ArtisanSupportRequests { get; set; } = new();
        public List<CourseProjectSubmission> ProjectSubmissions { get; set; }
        public List<CommunitiesViewModel> MyCommunities { get; set; } = new();
        public List<CommunityJoinRequest> PendingJoinRequests { get; set; } = new();
        public Dictionary<int, List<CommunityMemberViewModel>> CommunityMembers { get; set; } = new();
    }
}
