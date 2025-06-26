namespace SkillBuilder.Models
{
    public class CourseModule
    {
        public int Id { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; }

        public string Title { get; set; }
        public string ContentType { get; set; }
        public int Order { get; set; }

        public string VideoUrl { get; set; }
        public string Description { get; set; }

        public ICollection<ModuleProgress> Progresses { get; set; }
        public ICollection<ModuleContent> Contents { get; set; } = new List<ModuleContent>();
    }
}
