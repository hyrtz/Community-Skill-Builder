namespace SkillBuilder.Models.ViewModels
{
    public class CommunitiesViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string AvatarUrl { get; set; }
        public int MembersCount { get; set; }
        public string CoverImageUrl { get; set; }
        public string CreatorId { get; set; }
        public string? BannerUrl => CoverImageUrl;
        public string? Category { get; set; }

    }
}
