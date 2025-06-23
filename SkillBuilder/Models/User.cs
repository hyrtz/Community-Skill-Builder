namespace SkillBuilder.Models
{
    public class User
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; } = "Learner";
        public bool IsVerified { get; set; } = false;
        public string? SelectedInterests { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
