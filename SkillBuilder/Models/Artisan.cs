using System.ComponentModel.DataAnnotations.Schema;

namespace SkillBuilder.Models
{
    public class Artisan
    {
        public string ArtisanId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Profession { get; set; } = string.Empty;
        public string Hometown { get; set; } = string.Empty;
        public string UserAvatar { get; set; } = "/assets/Avatar/Sample10.svg";
        public string Introduction { get; set; } = string.Empty;
        public string Role { get; set; } = "Artisan";
        public bool IsApproved { get; set; }

        public User? User { get; set; }

        public List<ArtisanWork> Works { get; set; } = new();
        public List<Course> Courses { get; set; } = new();
    }
}