namespace SkillBuilder.Models.ViewModels
{
    public class ArtisanApplicationViewModel
    {
        public string Profession { get; set; } = string.Empty;
        public string Hometown { get; set; } = string.Empty;
        public IFormFile? File { get; set; }
    }
}
