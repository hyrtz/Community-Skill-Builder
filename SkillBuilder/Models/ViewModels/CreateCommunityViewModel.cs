namespace SkillBuilder.Models.ViewModels
{
    public class CreateCommunityViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string CategoryFinal { get; set; }

        public IFormFile Avatar { get; set; }       // optional
        public IFormFile Banner { get; set; }       // optional
    }
}
