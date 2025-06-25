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
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public ICollection<Enrollment> Enrollments { get; set; }
        public ICollection<CourseReview> Reviews { get; set; }
        public List<CourseProjectSubmission> ProjectSubmissions { get; set; }

        // Not mapped to DB — calculated properties
        [NotMapped]
        public int UserCount => Enrollments?.Count ?? 0;

        [NotMapped]
        public double AverageRating => (Reviews?.Any() == true) ? Reviews.Average(r => r.Rating) : 0;

        public Artisan Artisan { get; set; }
    }
}