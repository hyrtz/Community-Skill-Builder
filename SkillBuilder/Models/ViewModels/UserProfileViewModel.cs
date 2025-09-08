namespace SkillBuilder.Models.ViewModels
{
    public class UserProfileViewModel
    {
        public User User { get; set; } = new();
        public List<Enrollment> Enrollments { get; set; } = new();
        public List<CourseProgressViewModel> CourseProgresses { get; set; } = new();
        public List<Course> EnrolledCourses { get; set; } = new();
        public List<Course> AllCourses { get; set; } = new();
        public List<Course> RecommendedCourses { get; set; } = new();
        public List<CourseProjectSubmission> SubmittedProjects { get; set; } = new();
        public List<SupportSessionRequest> SupportRequests { get; set; } = new(); // Requests made by learner
        public List<SupportSessionRequest> ArtisanSupportRequests { get; set; } = new(); // Requests where user is the artisan
        public List<AchievementViewModel> Achievements { get; set; } = new();

    }
}