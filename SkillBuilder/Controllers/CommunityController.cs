using Appwrite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillBuilder.Data;
using SkillBuilder.Models;
using SkillBuilder.Models.ViewModels;
using SkillBuilder.Services;

namespace SkillBuilder.Controllers
{
    [Route("Community")]
    public class CommunityController : Controller
    {
        private readonly AppDbContext _context;
        private readonly INotificationService _notificationService;
        public CommunityController(AppDbContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        [HttpGet("Hub")]
        public async Task<IActionResult> CommunityHub(int? selectedCommunityId = null, string search = null)
        {
            ViewData["UseCourseNavbar"] = true;

            var communitiesQuery = _context.Communities
                .Where(c => c.IsPublished) // ✅ Only published communities
                .AsQueryable();

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
                        MembersCount = c.MembersCount,
                        CreatorId = c.CreatorId
                    })
                    .ToListAsync();

                Community selectedCommunity = null;
                List<CommunityPostViewModel> posts = new List<CommunityPostViewModel>();

                if (selectedCommunityId.HasValue)
                {
                    selectedCommunity = await _context.Communities
                        .Include(c => c.Memberships)
                        .FirstOrDefaultAsync(c => c.Id == selectedCommunityId.Value && c.IsPublished); // Only published

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
                                CommentsCount = 0,
                                Category = p.Category
                            })
                            .ToListAsync();
                    }
                    else
                    {
                        // Redirect if the community is unpublished or doesn't exist
                        return RedirectToAction("CommunityHub");
                    }
                }
            else
            {
                posts = await _context.CommunityPosts
                    .Include(p => p.Author)
                    .Where(p => p.IsPublished && p.CommunityId == null)
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
                        CommunityName = "Public", 
                        CommentsCount = 0,
                        Category = p.Category
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

            await _notificationService.AddNotificationAsync(
                userId,
                $"✅ Your post '{post.Title}' has been successfully created in the public feed."
            );

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
                    authorAvatarUrl = user.UserAvatar,
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

            var memberIds = await _context.CommunityMemberships
                .Where(m => m.CommunityId == post.CommunityId && m.UserId != userId)
                .Select(m => m.UserId)
                .ToListAsync();

            foreach (var memberId in memberIds)
            {
                await _notificationService.AddNotificationAsync(
                    memberId,
                    $"📝 New post '{post.Title}' has been added in the community '{community.Name}'."
                );
            }

            await _notificationService.AddNotificationAsync(
                userId,
                $"✅ Your post '{post.Title}' has been successfully created in '{community.Name}'."
            );

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

        public class EditCommunityPostViewModel
        {
            public int PostId { get; set; }
            public string Title { get; set; } = string.Empty;
            public string Content { get; set; } = string.Empty;
            public IFormFile? Image { get; set; }
            public bool RemoveImage { get; set; }
        }

        [HttpPost("EditPost")]
        public async Task<IActionResult> EditPost([FromForm] EditCommunityPostViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid post data." });

            var userId = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var post = await _context.CommunityPosts
                .Include(p => p.Author)
                .Include(p => p.Community)
                .FirstOrDefaultAsync(p => p.Id == model.PostId);

            if (post == null)
                return NotFound(new { success = false, message = "Post not found." });

            if (post.AuthorId != userId)
                return Forbid();

            post.Title = model.Title;
            post.Content = model.Content;

            // ✅ Handle image update/removal logic
            if (model.RemoveImage)
            {
                post.ImageUrl = null; // remove image from DB
            }
            else if (model.Image != null)
            {
                post.ImageUrl = await SaveImage(model.Image, "community-posts");
            }

            _context.CommunityPosts.Update(post);
            await _context.SaveChangesAsync();

            // ✅ Notify the user who edited
            await _notificationService.AddNotificationAsync(
                userId,
                $"✏️ Your post '{post.Title}' has been successfully updated."
            );

            return Ok(new
            {
                success = true,
                message = "Post updated successfully!",
                post = new
                {
                    id = post.Id,
                    title = post.Title,
                    content = post.Content,
                    imageUrl = post.ImageUrl,
                    submittedAt = post.SubmittedAt,
                    authorName = post.Author.FirstName,
                    communityName = post.Community?.Name ?? "Public"
                }
            });
        }

        public class ReportCommunityPostViewModel
        {
            public int PostId { get; set; }
            public string Reason { get; set; } = string.Empty;
            public string? Details { get; set; }
        }

        [HttpPost("ReportPost")]
        public async Task<IActionResult> ReportPost([FromForm] ReportCommunityPostViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid report data." });

            var userId = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var post = await _context.CommunityPosts
                .Include(p => p.Author)
                .Include(p => p.Community)
                .FirstOrDefaultAsync(p => p.Id == model.PostId);

            if (post == null)
                return NotFound(new { success = false, message = "Post not found." });

            // Save report
            var report = new CommunityPostReport
            {
                PostId = post.Id,
                ReporterId = userId,
                Reason = model.Reason,
                Details = model.Details,
                ReportedAt = DateTime.UtcNow
            };
            _context.CommunityPostReports.Add(report);
            await _context.SaveChangesAsync();

            // 🔔 Notify the reporting user
            await _notificationService.AddNotificationAsync(
                userId,
                $"⚠️ You successfully reported the post '{post.Title}' for '{model.Reason}'."
            );

            // 🔔 Notify all admins
            var adminIds = await _context.Users
                .Where(u => u.Role == "Admin")
                .Select(u => u.Id)
                .ToListAsync();

            foreach (var adminId in adminIds)
            {
                await _notificationService.AddNotificationAsync(
                    adminId,
                    $"⚠️ Post '{post.Title}' has been reported by {post.Author.FirstName} {post.Author.LastName}. Reason: {model.Reason}"
                );
            }

            return Ok(new { success = true, message = "Post reported successfully." });
        }

        [HttpPost("DeletePost")]
        public async Task<IActionResult> DeletePost(int postId)
        {
            var userId = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var post = await _context.CommunityPosts
                .Include(p => p.Author)
                .Include(p => p.Community)
                .FirstOrDefaultAsync(p => p.Id == postId);

            if (post == null)
                return NotFound(new { success = false, message = "Post not found." });

            var communityOwnerId = post.Community?.CreatorId;

            // Only author or community owner can delete
            if (post.AuthorId != userId && communityOwnerId != userId)
                return Forbid();

            _context.CommunityPosts.Remove(post);
            await _context.SaveChangesAsync();

            // 🔔 Notify the user who deleted the post
            await _notificationService.AddNotificationAsync(
                userId,
                $"🗑️ You have successfully deleted the post '{post.Title}'."
            );

            // 🔔 Notify the community owner if different from the deleter
            if (!string.IsNullOrEmpty(communityOwnerId) && communityOwnerId != userId)
            {
                await _notificationService.AddNotificationAsync(
                    communityOwnerId,
                    $"🗑️ The post '{post.Title}' in your community '{post.Community.Name}' was deleted by {post.Author.FirstName} {post.Author.LastName}."
                );
            }

            return Ok(new { success = true, message = "Post deleted successfully." });
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

            await _notificationService.AddNotificationAsync(
                userId,
                $"✅ Your Community '{community.Name}' has been created successfully."
            );

            // ✅ Notify all Admins
            var adminIds = await _context.Users
                .Where(u => u.Role == "Admin")
                .Select(u => u.Id)
                .ToListAsync();

            foreach (var adminId in adminIds)
            {
                await _notificationService.AddNotificationAsync(
                    adminId,
                    $"🌐 A new community '{community.Name}' was created by {user.FirstName} {user.LastName}."
                );
            }

            return RedirectToAction("CommunityHub", new { selectedCommunityId = community.Id });
        }

        [HttpPost("Join")]
        public async Task<IActionResult> JoinCommunity(int communityId, string joinMessage)
        {
            var userId = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            // Check if the user is already a member
            var existingMembership = await _context.CommunityMemberships
                .FirstOrDefaultAsync(m => m.CommunityId == communityId && m.UserId == userId);
            if (existingMembership != null)
                return BadRequest(new { success = false, message = "You are already a member of this community." });

            // Check if there's already a pending join request
            var existingRequest = await _context.CommunityJoinRequests
                .FirstOrDefaultAsync(r => r.CommunityId == communityId && r.UserId == userId && r.Status == "Pending");
            if (existingRequest != null)
                return BadRequest(new { success = false, message = "You have already requested to join this community." });

            // Create a new join request
            var joinRequest = new CommunityJoinRequest
            {
                CommunityId = communityId,
                UserId = userId,
                ShortMessage = joinMessage?.Trim(),
                RequestedAt = DateTime.UtcNow,
                Status = "Pending"
            };

            _context.CommunityJoinRequests.Add(joinRequest);
            await _context.SaveChangesAsync();

            // Notify the community owner
            var community = await _context.Communities.FindAsync(communityId);
            if (community != null && !string.IsNullOrEmpty(community.CreatorId) && community.CreatorId != userId)
            {
                await _notificationService.AddNotificationAsync(
                    community.CreatorId,
                    $"👤 {User.Identity.Name} has requested to join your community '{community.Name}'."
                );
            }

            // Notify the user
            await _notificationService.AddNotificationAsync(
                userId,
                $"✅ Your request to join '{community?.Name}' has been sent and is pending approval."
            );

            return Ok(new { success = true, message = "Join request sent successfully!" });
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

        [HttpGet("HasJoinRequest")]
        public async Task<IActionResult> HasJoinRequest(int communityId)
        {
            var userId = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var exists = await _context.CommunityJoinRequests
                .AnyAsync(r => r.CommunityId == communityId && r.UserId == userId && r.Status == "Pending");

            return Ok(new { success = true, hasRequest = exists });
        }

        [HttpPost("HandleJoinRequest")]
        public async Task<IActionResult> HandleJoinRequest([FromBody] JoinRequestActionModel model)
        {
            if (model == null)
                return BadRequest(new { success = false, message = "Invalid request data." });

            var userId = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var joinRequest = await _context.CommunityJoinRequests
                .Include(r => r.User)
                .Include(r => r.Community)
                .FirstOrDefaultAsync(r => r.Id == model.RequestId);

            if (joinRequest == null)
                return NotFound(new { success = false, message = "Join request not found." });

            // Only the community owner can approve/reject
            if (joinRequest.Community.CreatorId != userId)
                return Forbid();

            if (model.Approve)
            {
                // ✅ Approve request: add member to community
                _context.CommunityMemberships.Add(new CommunityMembership
                {
                    CommunityId = joinRequest.CommunityId,
                    UserId = joinRequest.UserId,
                    JoinedAt = DateTime.UtcNow,
                    Role = "Member"
                });

                // Update community member count
                joinRequest.Community.MembersCount += 1;

                // Notify both user and owner
                await _notificationService.AddNotificationAsync(
                    joinRequest.UserId,
                    $"🎉 Your join request for '{joinRequest.Community.Name}' has been approved!"
                );

                await _notificationService.AddNotificationAsync(
                    joinRequest.Community.CreatorId,
                    $"✅ You approved {joinRequest.User.FirstName} {joinRequest.User.LastName}'s join request for '{joinRequest.Community.Name}'."
                );
            }
            else
            {
                // ❌ Reject request: notify both user and owner
                string reasonText = string.IsNullOrWhiteSpace(model.Reason)
                    ? ""
                    : $"\n\n📝 Reason: {model.Reason}";

                // Notify rejected user
                await _notificationService.AddNotificationAsync(
                    joinRequest.UserId,
                    $"🚫 Your join request for '{joinRequest.Community.Name}' has been rejected.{reasonText}"
                );

                // Notify owner about their own action
                await _notificationService.AddNotificationAsync(
                    joinRequest.Community.CreatorId,
                    $"🚫 You rejected {joinRequest.User.FirstName} {joinRequest.User.LastName}'s join request for '{joinRequest.Community.Name}'.{reasonText}"
                );
            }

            // 🔹 Delete the join request from database
            _context.CommunityJoinRequests.Remove(joinRequest);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = model.Approve
                    ? $"✅ {joinRequest.User.FirstName} {joinRequest.User.LastName} has been added to the community."
                    : $"❌ {joinRequest.User.FirstName} {joinRequest.User.LastName}'s request has been rejected."
            });
        }

        public class JoinRequestActionModel
        {
            public int RequestId { get; set; }
            public bool Approve { get; set; }
            public string? Reason { get; set; } // ✅ Added reason
        }

    }

}