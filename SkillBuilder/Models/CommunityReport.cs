using System;

namespace SkillBuilder.Models.Entities
{
    public class CommunityReport
    {
        public int Id { get; set; }

        public int CommunityId { get; set; }
        public Community Community { get; set; }

        public string ReporterId { get; set; }
        public User Reporter { get; set; }

        public string Reason { get; set; }
        public string? Details { get; set; }

        public DateTime ReportedAt { get; set; } = DateTime.UtcNow;
    }
}