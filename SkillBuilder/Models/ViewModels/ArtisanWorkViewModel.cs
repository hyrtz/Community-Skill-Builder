namespace SkillBuilder.Models.ViewModels
{
    public class ArtisanWorkViewModel
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public IFormFile? ImageFile { get; set; } // upload
        public string? Title { get; set; }
        public string? Caption { get; set; }
        public string? ImageUrl { get; set; }
    }
}
