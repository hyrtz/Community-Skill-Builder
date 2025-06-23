namespace SkillBuilder.Services
{
    public interface IEmailService
    {
        Task SendVerificationEmail(string toEmail, string verificationCode);
    }
}