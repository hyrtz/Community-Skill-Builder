using Appwrite.Models;
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
        public async Task<IActionResult> CommunityHub(int? selectedCommunityId = null, string search = null)
        {
            ViewData["UseCourseNavbar"] = true;

            var communitiesQuery = _context.Communities.AsQueryable();

            if (!string.IsNullOrEmpty(search))
                communitiesQuery = communitiesQuery.Where(c => c.Name.ToLower().Contains(search.ToLower()));

            var communities = await communitiesQuery
                .OrderByDescending(c => c.MembersCount)
                .Take(20)
                .Select(c => new CommunitiesViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    AvatarUrl = !string.IsNullOrEmpty(c.AvatarUrl) ? c.AvatarUrl : "/uploads/community-profile/default-community.png",
                    CoverImageUrl = !string.IsNullOrEmpty(c.CoverImageUrl) ? c.CoverImageUrl : "/uploads/community-banner/default-banner.png",
                    MembersCount = c.MembersCount
                })
                .ToListAsync();

            Community selectedCommunity = null;
            List<CommunityPostViewModel> posts = new List<CommunityPostViewModel>();

            if (selectedCommunityId.HasValue)
            {
                selectedCommunity = await _context.Communities
                    .Include(c => c.Memberships)
                    .FirstOrDefaultAsync(c => c.Id == selectedCommunityId.Value);

                if (selectedCommunity != null)
                {
                    posts = await _context.CommunityPosts
                        .Include(p => p.Author)
                        .Where(p => p.CommunityId == selectedCommunity.Id && p.IsPublished)
                        .OrderByDescending(p => p.SubmittedAt)
                        .Take(10)
                        .Select(p => new CommunityPostViewModel
                        {
                            Id = p.Id,
                            Title = p.Title,
                            Content = p.Content,
                            SubmittedAt = p.SubmittedAt,
                            ImageUrl = p.ImageUrl,
                            AuthorId = p.AuthorId,
                            AuthorName = p.Author.FirstName,
                            AuthorAvatarUrl = p.Author.UserAvatar,
                            CommunityName = p.Community.Name,
                            Likes = 0,
                            CommentsCount = 0
                        })
                        .ToListAsync();
                }
            }
            else
            {
                posts = await _context.CommunityPosts
                    .Include(p => p.Author)
                    .Where(p => p.IsPublished)
                    .OrderByDescending(p => p.SubmittedAt)
                    .Take(10)
                    .Select(p => new CommunityPostViewModel
                    {
                        Id = p.Id,
                        Title = p.Title,
                        Content = p.Content,
                        SubmittedAt = p.SubmittedAt,
                        ImageUrl = p.ImageUrl,
                        AuthorId = p.AuthorId,
                        AuthorName = p.Author.FirstName,
                        AuthorAvatarUrl = p.Author.UserAvatar,
                        CommunityName = p.Community.Name,
                        Likes = 0, 
                        CommentsCount = 0
                    })
                    .ToListAsync();
            }

            var vm = new CommunityHubViewModel
            {
                Communities = communities,
                Posts = posts,
                SelectedCommunity = selectedCommunity
            };

            return View("CommunityHub", vm);
        }

        [HttpGet("GetCommunityDetailsPartial")]
        public async Task<IActionResult> GetCommunityDetailsPartial(int id)
        {
            var community = await _context.Communities
                .Include(c => c.Memberships)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (community == null)
                return NotFound();

            var currentUserId = User.FindFirst("UserId")?.Value;

            var detailsVm = new CommunityDetailsViewModel
            {
                SelectedCommunity = community,
                IsOwner = community.CreatorId == currentUserId,
                IsJoined = community.Memberships.Any(m => m.UserId == currentUserId)
            };

            return PartialView("~/Views/Shared/Sections/_CommunityDetailsSection.cshtml", detailsVm);
        }

        [HttpPost("CreatePost")]
        public async Task<IActionResult> CreatePost([FromForm] CreateCommunityPostViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid post data." });

            var userId = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound(new { success = false, message = "User not found." });

            string? imagePath = null;
            if (model.Image != null)
                imagePath = await SaveImage(model.Image, "community-posts");

            var post = new CommunityPost
            {
                CommunityId = null,
                Title = model.Title,
                Content = model.Content,
                AuthorId = userId,
                Category = model.Category, 
                ImageUrl = imagePath,
                SubmittedAt = DateTime.UtcNow,
                IsPublished = true
            };

            _context.CommunityPosts.Add(post);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Post created successfully!",
                post = new
                {
                    id = post.Id,
                    title = post.Title,
                    content = post.Content,
                    imageUrl = post.ImageUrl,
                    submittedAt = post.SubmittedAt,
                    authorName = user.FirstName,
                    communityName = "Public" // since no community
                }
            });
        }

        [HttpPost("CreatePostInsideCommunity")]
        public async Task<IActionResult> CreatePostInsideCommunity([FromForm] CreateCommunityPostViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid post data." });

            var userId = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound(new { success = false, message = "User not found." });

            if (model.CommunityId <= 0)
                return BadRequest(new { success = false, message = "Community not selected." });

            var community = await _context.Communities.FindAsync(model.CommunityId);
            if (community == null)
                return NotFound(new { success = false, message = "Community not found." });

            string? imagePath = null;
            if (model.Image != null)
                imagePath = await SaveImage(model.Image, "community-posts");

            var post = new CommunityPost
            {
                CommunityId = model.CommunityId,
                Title = model.Title,
                Content = model.Content,
                AuthorId = userId,
                Category = string.IsNullOrEmpty(model.Category) ? "General" : model.Category,
                ImageUrl = imagePath,
                SubmittedAt = DateTime.UtcNow,
                IsPublished = true
            };

            _context.CommunityPosts.Add(post);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Post created successfully!",
                post = new
                {
                    id = post.Id,
                    title = post.Title,
                    content = post.Content,
                    imageUrl = post.ImageUrl,
                    submittedAt = post.SubmittedAt,
                    authorName = user.FirstName,
                    communityName = community.Name
                }
            });
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateCommunity(CreateCommunityViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid data.");

            var userId = User.FindFirst("UserId")?.Value; // ✅ fixed
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return NotFound(new { success = false, message = "User not found." });

            if (!user.IsVerified)
                return BadRequest(new { success = false, message = "Please verify your email before creating a community." });

            if (user.IsDeactivated)
                return Forbid();

            string avatarPath = null;
            string bannerPath = null;

            if (model.Avatar != null)
                avatarPath = await SaveImage(model.Avatar, "community-profile");

            if (model.Banner != null)
                bannerPath = await SaveImage(model.Banner, "community-banner");

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

            _context.CommunityMemberships.Add(new CommunityMembership
            {
                CommunityId = community.Id,
                UserId = userId,
                JoinedAt = DateTime.UtcNow,
                Role = "Owner"
            });

            await _context.SaveChangesAsync();

            return RedirectToAction("CommunityHub", new { selectedCommunityId = community.Id });
        }

        [HttpPost("Join")]
        public async Task<IActionResult> JoinCommunity(int communityId)
        {
            var userId = User.FindFirst("UserId")?.Value; // ✅ fixed
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var existingMembership = await _context.CommunityMemberships
                .FirstOrDefaultAsync(m => m.CommunityId == communityId && m.UserId == userId);

            if (existingMembership != null)
                return BadRequest(new { success = false, message = "You are already a member of this community." });

            var membership = new CommunityMembership
            {
                CommunityId = communityId,
                UserId = userId,
                JoinedAt = DateTime.UtcNow,
                Role = "Member"
            };

            _context.CommunityMemberships.Add(membership);

            var community = await _context.Communities.FindAsync(communityId);
            if (community != null)
                community.MembersCount += 1;

            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Joined successfully!", membersCount = community.MembersCount });
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