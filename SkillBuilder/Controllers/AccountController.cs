using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using Org.BouncyCastle.Crypto.Generators;
using SkillBuilder.Data;
using SkillBuilder.Models;
using System.Security.Claims;
using BCrypt.Net;

namespace SkillBuilder.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AccountController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("/signup")]
        public async Task<IActionResult> Signup([FromBody] SignupRequest model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new { message = string.Join(" ", errors) });
            }

            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (existingUser != null)
                return BadRequest(new { message = "Email already exists." });

            var newUser = new User
            {
                Id = await GenerateUserId("Learner"),
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                Role = "Learner",
                CreatedAt = DateTime.UtcNow,
                IsVerified = false, // just to be explicit
                UserAvatar = "/assets/Avatar/Sample10.svg"
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            await SendVerificationEmail(newUser.Email, newUser.Id);

            var claims = new List<Claim>
            {
                new Claim("UserId", newUser.Id),
                new Claim(ClaimTypes.NameIdentifier, newUser.Id),
                new Claim(ClaimTypes.Name, newUser.FirstName + " " + newUser.LastName),
                new Claim(ClaimTypes.Email, newUser.Email),
                new Claim(ClaimTypes.Role, newUser.Role)
            };

            var identity = new ClaimsIdentity(claims, "TahiAuth");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("TahiAuth", principal);

            return Ok(new { message = "Account created. Please check your email to verify." });
        }

        [HttpPost("/signup-artisan")]
        public async Task<IActionResult> SignupArtisan([FromBody] ArtisanSignupRequest model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new { message = string.Join(" ", errors) });
            }

            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (existingUser != null)
                return BadRequest(new { message = "Email already exists." });

            var newUserId = await GenerateUserId("Artisan");

            var newUser = new User
            {
                Id = newUserId,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                Role = "Artisan",
                CreatedAt = DateTime.UtcNow,
                IsVerified = false,
                UserAvatar = "/assets/Avatar/Sample10.svg"
            };

            var artisan = new Artisan
            {
                ArtisanId = newUserId, // Use same ID convention
                UserId = newUserId,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Profession = model.Profession,
                Hometown = model.Hometown,
                Introduction = model.Introduction,
                Role = "Artisan",
                UserAvatar = newUser.UserAvatar
            };

            _context.Users.Add(newUser);
            _context.Artisans.Add(artisan);

            await _context.SaveChangesAsync();

            await SendVerificationEmail(newUser.Email, newUser.Id);

            var claims = new List<Claim>
            {
                new Claim("UserId", newUser.Id),
                new Claim(ClaimTypes.NameIdentifier, newUser.Id),
                new Claim(ClaimTypes.Name, newUser.FirstName + " " + newUser.LastName),
                new Claim(ClaimTypes.Email, newUser.Email),
                new Claim(ClaimTypes.Role, newUser.Role)
            };

            var identity = new ClaimsIdentity(claims, "TahiAuth");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("TahiAuth", principal);

            return Ok(new { message = "Artisan account created. Please check your email to verify." });
        }

        private async Task SendVerificationEmail(string email, string userId)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Tahi Support", _config["Email:Sender"]));
            message.To.Add(MailboxAddress.Parse(email));
            message.Subject = "Verify your Tahi account";

            var verificationLink = Url.Action("VerifyEmail", "Account", new { id = userId }, Request.Scheme);

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

            Console.WriteLine("📧 Sending email to: " + email);
            await smtp.SendAsync(message);

            await smtp.DisconnectAsync(true);
        }
        private async Task<string> GenerateUserId(string rolePrefix)
        {
            var prefix = rolePrefix switch
            {
                "Learner" => "L",
                "Artisan" => "A",
                "Admin" => "ADMIN",
                _ => "L"
            };

            string newId;
            bool exists;

            do
            {
                // Generate 7-digit random number
                var random = new Random();
                var digits = random.Next(0, 9999999).ToString("D7");
                newId = $"{prefix}{digits}";

                // Check if ID already exists
                exists = await _context.Users.AnyAsync(u => u.Id == newId);

            } while (exists); // Retry if duplicate

            return newId;
        }

        [HttpGet("/verify")]
        public async Task<IActionResult> VerifyEmail(string id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
                return NotFound("User not found.");

            if (!user.IsVerified)
            {
                user.IsVerified = true;
                await _context.SaveChangesAsync();
            }

            var claims = new List<Claim>
            {
                new Claim("UserId", user.Id),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.FirstName + " " + user.LastName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var identity = new ClaimsIdentity(claims, "TahiAuth");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("TahiAuth", principal);

            return Redirect("/?verified=true");
        }

        [HttpGet("/force-logout")]
        public async Task<IActionResult> ForceLogout()
        {
            await HttpContext.SignOutAsync("TahiAuth");
            return Redirect("/");
        }

        [HttpPost("/login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid input." });
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }

            if (!user.IsVerified)
            {
                return Unauthorized(new { message = "Please verify your email first." });
            }

            var claims = new List<Claim>
            {
                new Claim("UserId", user.Id),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.FirstName + " " + user.LastName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var identity = new ClaimsIdentity(claims, "TahiAuth");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("TahiAuth", principal);

            return Ok(new
            {
                success = true,
                message = "Login successful.",
                role = user.Role,
                userId = user.Id
            });
        }

        [HttpPost("/logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("TahiAuth");
            return Redirect("/");
        }

    }
}