namespace SkillBuilder.Models
{
    public class CourseModule
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;
        public string Title { get; set; } = string.Empty;
        public int Order { get; set; }
        public ICollection<ModuleProgress> Progresses { get; set; } = new List<ModuleProgress>();
        public ICollection<ModuleContent> Contents { get; set; } = new List<ModuleContent>();
    }
}