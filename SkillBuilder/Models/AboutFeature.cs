using System.ComponentModel.DataAnnotations;

namespace SkillBuilder.Models
{
    public class AboutFeature
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string IconPath { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }
    }
}