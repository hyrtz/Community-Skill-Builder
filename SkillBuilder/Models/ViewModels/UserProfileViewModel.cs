namespace SkillBuilder.Models.ViewModels
{
    public class UserProfileViewModel
    {
        public string FirstName => User?.FirstName ?? "";
        public string LastName => User?.LastName ?? "";
        public string UserAvatar => User?.UserAvatar ?? "/images/default-avatar.png";
        public bool IsDeactivated => User?.IsDeactivated ?? false;
        public int Points => User?.Points ?? 0;
        public int? SelectedCommunityId { get; set; }
        public User User { get; set; } = new();
        public List<Artisan> Artisans { get; set; } = new();
        public List<Enrollment> Enrollments { get; set; } = new();
        public List<CourseProgressViewModel> CourseProgresses { get; set; } = new();
        public List<Course> EnrolledCourses { get; set; } = new();
        public List<Course> AllCourses { get; set; } = new();
        public List<Course> RecommendedCourses { get; set; } = new();
        public List<CommunitiesViewModel> MyCommunities { get; set; } = new();
        public List<CommunitiesViewModel> JoinedCommunities { get; set; } = new();
        public List<CommunityJoinRequest> PendingJoinRequests { get; set; } = new();
        public Dictionary<int, List<CommunityMemberViewModel>> CommunityMembers { get; set; } = new();
        public List<CourseProjectSubmission> SubmittedProjects { get; set; } = new();
        public List<SupportSessionRequest> SupportRequests { get; set; } = new(); // Requests made by learner
        public List<SupportSessionRequest> ArtisanSupportRequests { get; set; } = new(); // Requests where user is the artisan
        public List<User>? LeaderboardUsers { get; set; }
        public List<AchievementViewModel> Achievements { get; set; } = new();

    }
}