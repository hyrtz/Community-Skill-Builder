namespace SkillBuilder.Models
{
    public class ModuleContent
    {
        public int Id { get; set; }

        public int CourseModuleId { get; set; }
        public CourseModule CourseModule { get; set; }

        public string ContentType { get; set; }  // e.g., "Text", "ImageText", "Video", "Quiz", "Session", "Project"

        public string Title { get; set; }        // Optional display title
        public string ContentText { get; set; }  // For text or description
        public string MediaUrl { get; set; }     // Image or Video URL if applicable

        public int Order { get; set; }           // Position within the module
    }
}
