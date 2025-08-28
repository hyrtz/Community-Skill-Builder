namespace SkillBuilder.Models.ViewModels
{
    public class CommunityHubViewModel
    {
        public List<CommunityPostViewModel> Posts { get; set; } = new();
        public List<CommunitiesViewModel> Communities { get; set; } = new();
        public Community SelectedCommunity { get; set; }
    }
}