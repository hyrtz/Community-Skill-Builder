using System.ComponentModel.DataAnnotations;

namespace SkillBuilder.Models
{
    public class SignupRequest
    {
        [Required, RegularExpression(@"^[A-Za-z\s]{2,50}$", ErrorMessage = "First name must be 2-50 letters only.")]
        public string FirstName { get; set; }

        [Required, RegularExpression(@"^[A-Za-z\s]{2,50}$", ErrorMessage = "Last name must be 2-50 letters only.")]
        public string LastName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, MinLength(8), RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[!@#\^*_\-]).+$", ErrorMessage = "Password must be at least 8 characters with uppercase, number, and symbol.")]
        public string Password { get; set; }
    }
}