using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillBuilder.Data;
using SkillBuilder.Models;
using SkillBuilder.Models.ViewModels;

namespace SkillBuilder.Controllers
{
    [Route("Community")]
    public class CommunityController : Controller
    {
        private readonly AppDbContext _context;
        public CommunityController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("Hub")]
        public IActionResult CommunityHub(int? selectedCommunityId = null, string search = null)
        {
            ViewData["UseCourseNavbar"] = true;

            // Get communities
            var communitiesQuery = _context.Communities.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                communitiesQuery = communitiesQuery.Where(c => c.Name.ToLower().Contains(search));
            }

            var communities = communitiesQuery
                .OrderByDescending(c => c.MembersCount)
                .Take(20)
                .Select(c => new CommunitiesViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    AvatarUrl = !string.IsNullOrEmpty(c.AvatarUrl)
                    ? c.AvatarUrl
                    : "/uploads/community-profile/default-community.png",

                    CoverImageUrl = !string.IsNullOrEmpty(c.CoverImageUrl)
                    ? c.CoverImageUrl
                    : "/uploads/community-banner/default-banner.png",
                    MembersCount = c.MembersCount
                })
                .ToList();

            // Get posts
            var posts = _context.CommunityPosts
                .Include(p => p.Author)
                .OrderByDescending(p => p.SubmittedAt)
                .Take(10)
                .Select(p => new CommunityPostViewModel
                {
                    Id = p.Id,
                    Title = p.Title,
                    Content = p.Content,
                    SubmittedAt = p.SubmittedAt,
                    ImageUrl = p.ImageUrl,
                    IsPublished = p.IsPublished,
                    AuthorId = p.AuthorId,
                    AuthorName = p.Author.FirstName,
                    AuthorAvatarUrl = p.Author.UserAvatar,
                    Likes = 0,
                    CommentsCount = 0,
                    CommunityName = null
                }).ToList();

            // Determine selected community
            Community selectedCommunity = null;
            if (selectedCommunityId.HasValue)
            {
                selectedCommunity = _context.Communities
                    .Include(c => c.Memberships)
                    .Include(c => c.Posts)
                    .FirstOrDefault(c => c.Id == selectedCommunityId.Value);
            }

            var vm = new CommunityHubViewModel
            {
                Communities = communities,
                Posts = posts,
                SelectedCommunity = selectedCommunity
            };

            return View("CommunityHub", vm);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateCommunity(CreateCommunityViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid data.");

            // 1. Get current user ID
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            // 2. Prepare file paths
            string avatarPath = null;
            string bannerPath = null;

            if (model.Avatar != null)
                avatarPath = await SaveImage(model.Avatar, "community-profile");

            if (model.Banner != null)
                bannerPath = await SaveImage(model.Banner, "community-banner");

            // 3. Create community
            var community = new Community
            {
                Name = model.Name,
                Description = model.Description,
                AvatarUrl = avatarPath ?? "/assets/Images/default-community.png",
                CoverImageUrl = bannerPath ?? "/assets/Images/default-banner.png",
                MembersCount = 1,
                CreatorId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Communities.Add(community);
            await _context.SaveChangesAsync();

            // 4. Add membership for creator
            _context.CommunityMemberships.Add(new CommunityMembership
            {
                CommunityId = community.Id,
                UserId = userId,
                JoinedAt = DateTime.UtcNow,
                Role = "Owner"
            });

            await _context.SaveChangesAsync();

            // 5. Redirect to the newly created community page
            return RedirectToAction("CommunityHub", new { selectedCommunityId = community.Id });
        }

        private async Task<string> SaveImage(IFormFile file, string folderName)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", folderName);
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/uploads/{folderName}/{fileName}";
        }
    }

}