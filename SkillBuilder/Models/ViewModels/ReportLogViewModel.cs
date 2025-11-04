namespace SkillBuilder.Models.ViewModels
{
    public class ReportLogViewModel
    {
        public int Id { get; set; }
        public string ReportType { get; set; }
        public string TargetName { get; set; }

        // IDs
        public string TargetUserId { get; set; }
        public string? TargetCourseLink { get; set; }
        public int? TargetCommunityId { get; set; }
        public int? TargetPostId { get; set; }

        public string ReportedBy { get; set; }
        public string Reason { get; set; }
        public DateTime DateReported { get; set; }
        public string Status { get; set; }
    }
}