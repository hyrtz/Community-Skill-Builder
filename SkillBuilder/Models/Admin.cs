using System.ComponentModel.DataAnnotations.Schema;

namespace SkillBuilder.Models
{
    public class Admin
    {
        public string AdminId { get; set; }
        public string UserId { get; set; }
        public string Role { get; set; } = "Admin";

        public User User { get; set; }
    }
}