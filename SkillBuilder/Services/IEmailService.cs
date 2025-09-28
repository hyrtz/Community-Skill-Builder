using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace SkillBuilder.Services
{
    public interface IEmailService
    {
        Task SendVerificationEmail(string email, string userId, IUrlHelper urlHelper);
        Task SendVerificationEmailWithLink(string email, string verificationLink);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendVerificationEmail(string email, string userId, IUrlHelper urlHelper)
        {
            var verificationLink = urlHelper.Action(
                "VerifyEmail",
                "Account",
                new { id = userId },
                urlHelper.ActionContext.HttpContext.Request.Scheme
            );

            await SendVerificationEmailWithLink(email, verificationLink);
        }

        public async Task SendVerificationEmailWithLink(string email, string verificationLink)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Tahi Support", _config["Email:Sender"]));
                message.To.Add(MailboxAddress.Parse(email));
                message.Subject = "Verify your Tahi account";

                var builder = new BodyBuilder
                {
                    TextBody = "Welcome to Tahi. Please verify your account.",
                    HtmlBody = $@"
                        <h2>Welcome to Tahi!</h2>
                        <p>Thank you for signing up. Please click the button below to verify your account:</p>
                        <p><a href='{verificationLink}' style='background:#364BE9;padding:10px 20px;color:white;text-decoration:none;border-radius:4px;'>Verify My Account</a></p>"
                };

                message.Body = builder.ToMessageBody();

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_config["Email:Sender"], _config["Email:Password"]);
                await smtp.SendAsync(message);
                await smtp.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                Console.WriteLine($"Email sending failed: {ex.Message}");

            }
        }
    }
}