namespace SkillBuilder.Models
{
    public class ArtisanWork
    {
        public int Id { get; set; }
        public string ArtisanId { get; set; }
        public string ImageUrl { get; set; }
        public string Caption { get; set; }

        public Artisan Artisan { get; set; }
    }
}
