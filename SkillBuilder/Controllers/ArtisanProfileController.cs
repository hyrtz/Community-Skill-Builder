using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillBuilder.Data;
using SkillBuilder.Models;
using SkillBuilder.Models.ViewModels;
using System.Security.Claims;

namespace SkillBuilder.Controllers
{
    [Authorize(AuthenticationSchemes = "TahiAuth", Roles = "Artisan")]
    [Route("ArtisanProfile")]
    public class ArtisanProfileController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;

        public ArtisanProfileController(AppDbContext context, IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        // Artisan Dashboard (Self view)
        [HttpGet("{id}")]
        public IActionResult ArtisanProfile(string id)
        {
            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (currentUserId == null || id != currentUserId)
                return Forbid();

            var artisan = _context.Artisans
                .Include(a => a.User)
                .FirstOrDefault(a => a.UserId == currentUserId);

            if (artisan == null)
                return NotFound();

            var courses = _context.Courses
                .Where(c => c.CreatedBy == artisan.UserId)
                .ToList();

            var works = _context.ArtisanWorks
                .Where(w => w.ArtisanId == artisan.ArtisanId)
                .ToList();

            var artisanSupportRequests = _context.SupportSessionRequests
                .Include(r => r.User)
                .Include(r => r.Course)
                .Where(r => r.Course != null && r.Course.CreatedBy == artisan.UserId)
                .ToList();

            var viewModel = new ArtisanProfileViewModel
            {
                Artisan = artisan,
                Courses = courses,
                ArtisanWorks = works,
                ArtisanSupportRequests = artisanSupportRequests
            };

            return View("~/Views/Profile/ArtisanProfile.cshtml", viewModel);
        }

        // Public view as Mentor
        [HttpGet("/ArtisanViewAsMentor/{id}")]
        public IActionResult ViewAsMentor(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();

            var artisan = _context.Artisans
                .Include(a => a.User)
                .FirstOrDefault(a => a.ArtisanId == id);

            if (artisan == null)
                return NotFound();

            var courses = _context.Courses
                .Where(c => c.CreatedBy == artisan.UserId)
                .ToList();

            var works = _context.ArtisanWorks
                .Where(w => w.ArtisanId == artisan.ArtisanId)
                .ToList();

            var viewModel = new ArtisanProfileViewModel
            {
                Artisan = artisan,
                Courses = courses,
                ArtisanWorks = works
            };

            return View("~/Views/Profile/ArtisanViewAsMentor.cshtml", viewModel);
        }

        // GET: Edit profile (Artisan-specific)
        [HttpGet("EditProfileArtisan")]
        public async Task<IActionResult> EditProfileArtisan()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var artisan = await _context.Artisans
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.UserId == userId);

            if (artisan == null)
                return NotFound();

            var model = new ArtisanProfileViewModel
            {
                Artisan = artisan
            };

            return View("~/Views/Actions/ArtisanActions/EditProfileArtisan.cshtml", model);
        }

        // POST: Edit profile (Artisan-specific)
        [HttpPost("EditProfileArtisan")]
        public async Task<IActionResult> EditProfileArtisan(
            string FirstName,
            string LastName,
            string Email,
            string Profession,
            string Hometown,
            string Introduction,
            IFormFile UserAvatar,
            string CurrentPassword,
            string NewPassword,
            string ConfirmPassword)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var artisan = await _context.Artisans
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.UserId == userId);

            if (artisan == null)
                return NotFound();

            bool hasChanges = false;

            // --- Sync basic fields between Artisan and User ---
            void SyncField(ref string artisanField, ref string userField, string newValue)
            {
                if (artisanField != newValue)
                {
                    artisanField = newValue;
                    if (userField != null) userField = newValue;
                    hasChanges = true;
                }
            }

            // --- Sync Artisan + User fields where applicable ---
            if (artisan.FirstName != FirstName)
            {
                artisan.FirstName = FirstName;
                if (artisan.User != null)
                    artisan.User.FirstName = FirstName;
                hasChanges = true;
            }

            if (artisan.LastName != LastName)
            {
                artisan.LastName = LastName;
                if (artisan.User != null)
                    artisan.User.LastName = LastName;
                hasChanges = true;
            }

            // Artisan-only fields
            if (artisan.Profession != Profession)
            {
                artisan.Profession = Profession;
                hasChanges = true;
            }

            if (artisan.Hometown != Hometown)
            {
                artisan.Hometown = Hometown;
                hasChanges = true;
            }

            if (artisan.Introduction != Introduction)
            {
                artisan.Introduction = Introduction;
                hasChanges = true;
            }

            // --- Update email only in User ---
            if (artisan.User != null && artisan.User.Email != Email)
            {
                artisan.User.Email = Email;
                _context.Entry(artisan.User).State = EntityState.Modified;
                hasChanges = true;
            }

            // --- Update avatar (sync Artisan + User) ---
            if (UserAvatar != null && UserAvatar.Length > 0 && artisan.User != null)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/avatars");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(UserAvatar.FileName)}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await UserAvatar.CopyToAsync(stream);
                }

                var avatarPath = $"/uploads/avatars/{fileName}?t={DateTime.UtcNow.Ticks}";
                artisan.User.UserAvatar = avatarPath;
                artisan.UserAvatar = avatarPath;
                _context.Entry(artisan.User).State = EntityState.Modified;
                hasChanges = true;
            }

            // --- Password change ---
            if (!string.IsNullOrEmpty(NewPassword) && NewPassword == ConfirmPassword && !string.IsNullOrEmpty(CurrentPassword))
            {
                if (!string.IsNullOrEmpty(artisan.User.PasswordHash))
                {
                    bool passwordMatches = BCrypt.Net.BCrypt.Verify(CurrentPassword, artisan.User.PasswordHash);
                    if (passwordMatches)
                    {
                        artisan.User.PasswordHash = BCrypt.Net.BCrypt.HashPassword(NewPassword);
                        _context.Entry(artisan.User).State = EntityState.Modified;
                        hasChanges = true;
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Current password is incorrect.";
                        return RedirectToAction("EditProfileArtisan");
                    }
                }
            }

            // --- Save changes if anything changed ---
            if (hasChanges)
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Profile updated successfully.";
            }
            else
            {
                TempData["SuccessMessage"] = "No changes were made.";
            }

            return RedirectToAction("EditProfileArtisan");
        }

        [HttpGet("VerifyOldPassword")]
        public IActionResult VerifyOldPassword(string oldPassword)
        {
            var artisanId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(artisanId)) return Unauthorized();

            var artisan = _context.Artisans.Include(a => a.User).FirstOrDefault(a => a.UserId == artisanId);
            if (artisan?.User == null) return NotFound();

            bool isValid = BCrypt.Net.BCrypt.Verify(oldPassword, artisan.User.PasswordHash);
            return Json(new { isValid });
        }

        [HttpPost("ArchiveArtisanAccount")]
        public async Task<IActionResult> ArchiveArtisanAccount()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            // Load artisan + related User
            var artisan = await _context.Artisans
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.UserId == userId);

            if (artisan == null || artisan.User == null)
                return NotFound();

            // Soft delete the artisan profile
            artisan.IsArchived = true;

            // Soft delete the related User
            artisan.User.IsArchived = true;

            // Soft delete all courses created by this artisan
            var artisanCourses = await _context.Courses
                .Where(c => c.CreatedBy == artisan.UserId)
                .ToListAsync();

            foreach (var course in artisanCourses)
            {
                course.IsArchived = true;
            }

            _context.Artisans.Update(artisan);
            _context.Users.Update(artisan.User);
            _context.Courses.UpdateRange(artisanCourses);

            await _context.SaveChangesAsync();

            // Sign out after archiving
            await HttpContext.SignOutAsync("TahiAuth");

            return RedirectToAction("Index", "Home");
        }
    }
}