using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkillBuilder.Models
{
    public class Notification
    {
        public int Id { get; set; }

        // Foreign key to User
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }

        public string Message { get; set; } 
        public bool IsRead { get; set; } = false;
        public string? ActionText { get; set; }
        public string? ActionUrl { get; set; }
        public bool IsActioned { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; 
    }
}