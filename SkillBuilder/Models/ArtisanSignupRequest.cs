using System.ComponentModel.DataAnnotations;

namespace SkillBuilder.Models
{
    public class ArtisanSignupRequest
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Profession { get; set; } = string.Empty;
        public string Hometown { get; set; } = string.Empty;
        public string Introduction { get; set; } = string.Empty;
    }
}
