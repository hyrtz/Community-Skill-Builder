using System.ComponentModel.DataAnnotations.Schema;

namespace SkillBuilder.Models
{
    public class Course
    {
        public int Id { get; set; }

        public string Title { get; set; }
        public string Link { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public string Duration { get; set; }
        public string Classes { get; set; }
        public string Level { get; set; }
        public string Video { get; set; }
        public string Thumbnail { get; set; }
        public string WhatToLearn { get; set; }
        public string FullDescription { get; set; }
        public string ProjectDetails { get; set; }
        public string Requirements { get; set; }

        // FK to Artisan (CreatedBy)
        public string CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsPublished { get; set; } = false;

        // Navigation Properties
        public Artisan Artisan { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public ICollection<CourseReview> Reviews { get; set; } = new List<CourseReview>();
        public ICollection<CourseModule> CourseModules { get; set; } = new List<CourseModule>();
        public ICollection<CourseProjectSubmission> ProjectSubmissions { get; set; } = new List<CourseProjectSubmission>();

        // Computed properties
        [NotMapped]
        public int UserCount => Enrollments?.Count ?? 0;

        [NotMapped]
        public int TotalModules => CourseModules?.Count ?? 0;

        [NotMapped]
        public double AverageRating => (Reviews != null && Reviews.Any()) ? Reviews.Average(r => r.Rating) : 0;
    }
}