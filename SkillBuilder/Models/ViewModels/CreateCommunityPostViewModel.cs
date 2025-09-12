using System.ComponentModel.DataAnnotations;

namespace SkillBuilder.Models.ViewModels
{
    public class CreateCommunityPostViewModel
    {
        public int CommunityId { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; }
        public IFormFile? Image { get; set; }
        public string? Category { get; set; }
    }
}
