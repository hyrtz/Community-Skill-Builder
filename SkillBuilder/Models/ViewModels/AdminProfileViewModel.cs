namespace SkillBuilder.Models.ViewModels
    {
        public class AdminProfileViewModel
        {
            public Admin Admin { get; set; }
            public List<User> Users { get; set; }
            public List<User> AllUsers { get; set; }
            public List<ArtisanApplication> PendingApplications { get; set; }
            public List<ArtisanApplication> AllPendingApplications { get; set; }
            public List<Course> SubmittedCourses { get; set; }
            public List<Course> AllSubmittedCourses { get; set; }
            public List<Community> Communities { get; set; }
            public List<Community> AllCommunities { get; set; }
            public List<CommunityPost> CommunitySubmissions { get; set; }
            public List<CommunityPost> AllCommunitySubmissions { get; set; }
        }
    }
