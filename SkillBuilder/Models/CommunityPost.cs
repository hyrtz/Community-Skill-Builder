using SkillBuilder.Models;

public class CommunityPost
{
    public int Id { get; set; }
    public int? CommunityId { get; set; }
    public Community? Community { get; set; }

    public string AuthorId { get; set; }
    public User Author { get; set; }

    public string Title { get; set; }
    public string Content { get; set; }
    public string? ImageUrl { get; set; }
    public string Category { get; set; } = string.Empty;
    public DateTime SubmittedAt { get; set; } = DateTime.Now;

    public bool IsPublished { get; set; } = true;  // ← add this
}