namespace SkillBuilder.Models.ViewModels
{
    public class ArtisanProfileViewModel
    {
        public Artisan Artisan { get; set; } = new();
        public List<Course> Courses { get; set; } = new();
        public List<ArtisanWork> ArtisanWorks { get; set; } = new();
        public List<SupportSessionRequest> ArtisanSupportRequests { get; set; } = new();
    }
}
