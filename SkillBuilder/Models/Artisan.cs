using System.ComponentModel.DataAnnotations.Schema;

namespace SkillBuilder.Models
{
    public class Artisan
    {
        public string ArtisanId { get; set; }
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Profession { get; set; }
        public string Hometown { get; set; }
        public string UserAvatar { get; set; } = "/assets/Avatar/Sample10.svg";
        public string Introduction { get; set; }
        public string Role { get; set; } = "Artisan";

        public User User { get; set; }

        public List<ArtisanWork> Works { get; set; }
        public List<Course> Courses { get; set; }
    }
}