namespace SkillBuilder.Models
{
    public class ArtisanProfileViewModel
    {
        public Artisan Artisan { get; set; }
        public List<Course> Courses { get; set; }
        public List<ArtisanWork> ArtisanWorks { get; set; }
    }
}
