namespace SkillBuilder.Models
{
    public class Course
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Link { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public string Duration { get; set; }
        public string DurationIcon { get; set; }
        public string Users { get; set; }
        public string UsersIcon { get; set; }
        public string Rating { get; set; }
        public string RatingIcon { get; set; }
        public string Classes { get; set; }
        public string Level { get; set; }
        public string Video { get; set; }
        public string Thumbnail { get; set; }
        public string WhatToLearn { get; set; }
        public string FullDescription { get; set; }
        public string ProjectDetails { get; set; }
        public string Requirements { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}