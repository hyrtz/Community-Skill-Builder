namespace SkillBuilder.Models.ViewModels
{
    public class CourseMaterialViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? FileName { get; set; }
        public IFormFile? UploadFile { get; set; }
        public string? Description { get; set; }
        public long FileSize { get; set; }
    }
}
