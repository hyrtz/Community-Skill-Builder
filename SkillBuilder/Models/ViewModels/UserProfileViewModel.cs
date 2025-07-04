﻿namespace SkillBuilder.Models.ViewModels
{
    public class UserProfileViewModel
    {
        public User User { get; set; }
        public List<Enrollment> Enrollments { get; set; }
        public List<CourseProgressViewModel> CourseProgresses { get; set; }
        public List<Course> Courses { get; set; }
        public List<Course> EnrolledCourses { get; set; } = new();
        public List<Course> AllCourses { get; set; } = new();
        public List<CourseProjectSubmission> SubmittedProjects { get; set; }
        public List<SupportSessionRequest> SupportRequests { get; set; } = new();
        public List<AchievementViewModel> Achievements { get; set; }
    }
}
