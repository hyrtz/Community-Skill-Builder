namespace SkillBuilder.Models
{
    public class ProgressUpdateModel
    {
        public int CourseId { get; set; }
        public List<int> CompletedModules { get; set; }
    }
}
