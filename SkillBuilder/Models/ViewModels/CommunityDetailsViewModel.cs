namespace SkillBuilder.Models.ViewModels
{
    public class CommunityDetailsViewModel
    {
        public Community SelectedCommunity { get; set; }
        public bool IsOwner { get; set; }
        public bool IsJoined { get; set; }
        public List<CommunityPostViewModel> Posts { get; set; } = new();
    }
}
