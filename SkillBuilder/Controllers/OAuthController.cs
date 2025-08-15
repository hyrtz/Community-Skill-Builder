using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillBuilder.Data;
using SkillBuilder.Models;
using Appwrite;
using System;
using System.Threading.Tasks;

[Route("oauth")]
public class OAuthController : Controller
{
    private readonly AppDbContext _context;
    private readonly Client _appwriteClient;
    private readonly Appwrite.Services.Account _accountService;

    public OAuthController(AppDbContext context)
    {
        _context = context;

        _appwriteClient = new Client()
            .SetEndpoint("https://fra.cloud.appwrite.io/v1")
            .SetProject("689ad4040030d8902c02");

        _accountService = new Appwrite.Services.Account(_appwriteClient);
    }

    [HttpGet("success")]
    public async Task<IActionResult> Success()
    {
        try
        {
            // Only works after OAuth session is created!
            var appwriteUser = await _accountService.Get();

            if (appwriteUser == null || string.IsNullOrWhiteSpace(appwriteUser.Email))
                return Redirect("/?error=UnknownUser");

            string email = appwriteUser.Email.Trim().ToLower();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email);

            if (user == null)
                return Redirect("/?error=UserNotFound");

            var redirectUrl = user.Role switch
            {
                "Learner" => $"/UserProfile/{user.Id}?isVerified={user.IsVerified}",
                "Artisan" => $"/ArtisanProfile/{user.Id}?isVerified={user.IsVerified}",
                "Admin" => $"/AdminProfile/{user.Id}?isVerified={user.IsVerified}",
                _ => "/"
            };

            return Redirect(redirectUrl);
        }
        catch (Exception ex)
        {
            Console.WriteLine("[OAuth] Error: " + ex.Message);
            return Redirect("/?error=ServerError");
        }
    }
}