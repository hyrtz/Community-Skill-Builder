namespace SkillBuilder.Models.ViewModels
{
    public class CommunityPostViewModel
    {
        public int Id { get; set; }

        // Post details
        public string Title { get; set; }
        public string Content { get; set; }
        public string ImageUrl { get; set; }
        public string Category { get; set; }
        public DateTime SubmittedAt { get; set; }
        public bool IsPublished { get; set; }

        // Author (can be a User or an Artisan)
        public string AuthorId { get; set; }
        public string AuthorName { get; set; }
        public string AuthorAvatarUrl { get; set; } 
        public string? AuthorProfession { get; set; } // only if Artisan

        // Engagement
        public int CommentsCount { get; set; }

        // (Optional) later when communities are real entities
        public string? CommunityName { get; set; }
    }
}