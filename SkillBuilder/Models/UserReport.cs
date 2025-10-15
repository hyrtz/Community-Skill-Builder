namespace SkillBuilder.Models
{
    public class UserReport
    {
        public int Id { get; set; }
        public string ReporterId { get; set; }
        public string ReportedUserId { get; set; }
        public string ReportType { get; set; }
        public DateTime ReportedAt { get; set; }
    }
}
