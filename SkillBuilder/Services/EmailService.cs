using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace SkillBuilder.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendVerificationEmail(string toEmail, string verificationCode)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_config["EmailSettings:Sender"]));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = "Verify your email - Tahi";
            var baseUrl = _config["EmailSettings:FrontendBaseUrl"];

            email.Body = new TextPart("html")
            {
                Text = $@"
                    <h2>Welcome to Tahi!</h2>
                    <p>Click the link below to verify your email:</p>
                    <a href='{baseUrl}/verify?code={verificationCode}'>Verify Email</a>
                    <p>This link will expire in 10 minutes.</p>"
            };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_config["EmailSettings:SmtpServer"], int.Parse(_config["EmailSettings:Port"]), SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_config["EmailSettings:Username"], _config["EmailSettings:Password"]);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}