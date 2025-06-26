namespace SkillBuilder.Models
{
    public class ModuleProgress
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int CourseModuleId { get; set; }
        public CourseModule CourseModule { get; set; }
        public List<int> CompletedModules { get; set; } = new();

        public bool IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime? LastUpdated { get; set; }
    }
}
