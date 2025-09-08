using System.ComponentModel.DataAnnotations.Schema;

namespace SkillBuilder.Models
{
    public class ArtisanWork
    {
        public int Id { get; set; }
        public string ArtisanId { get; set; }
        public int CourseId { get; set; }

        public string Title { get; set; }

        public string ImageUrl { get; set; }

        public string Caption { get; set; }

        public DateTime PublishDate { get; set; } = DateTime.UtcNow;

        public Artisan Artisan { get; set; }
        public Course? Course { get; set; }

        [NotMapped]
        public IFormFile? ImageFile { get; set; }
    }
}