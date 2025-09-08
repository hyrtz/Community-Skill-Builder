namespace SkillBuilder.Models
{
    public class ArtisanApplication
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }

        public string Status { get; set; } = "Pending";
        public DateTime SubmittedAt { get; set; }
        public string Profession { get; set; }
        public string Hometown { get; set; }
        public string Introduction { get; set; } = string.Empty;
        public string? ApplicationFile { get; set; }
    }
}
