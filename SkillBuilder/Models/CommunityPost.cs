namespace SkillBuilder.Models
{
    public class CommunityPost
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public string AuthorId { get; set; }
        public User Author { get; set; }

        public DateTime SubmittedAt { get; set; }
        public bool IsPublished { get; set; } = false;

        // Optional fields
        public string ImageUrl { get; set; }
    }
}
