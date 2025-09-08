using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillBuilder.Models;
using SkillBuilder.Models.ViewModels;
using SkillBuilder.Data;
using System.Runtime.CompilerServices;
using System.Text;

public class HomeController : Controller
{
    private readonly AppDbContext _context;


    public HomeController(AppDbContext context)
    {
        _context = context;
    }
    public IActionResult Index()
    {
        List<AboutFeature> features;

        if (User.IsInRole("Artisan"))
        {
            // Artisan-specific features
            features = new List<AboutFeature>
        {
            new AboutFeature { Id = 101, IconPath = "/assets/Icons/Preservation.ico", Title = "Cultural Preservation", Description = "Play a vital role in preserving and promoting Filipino cultural heritage for future generations." },
            new AboutFeature { Id = 102, IconPath = "/assets/Icons/Course.ico", Title = "Teaching Support", Description = "Access tools and resources to help you create engaging courses and teaching materials." },
            new AboutFeature { Id = 103, IconPath = "/assets/Icons/Growth.ico", Title = "Growth Opportunities", Description = "Build your reputation and expand your influence as a recognized cultural expert." },
            new AboutFeature { Id = 104, IconPath = "/assets/Icons/Sessions.ico", Title = "Flexible Schedule", Description = "Teach on your own time with\r\nboth pre-recorded courses and scheduled live sessions." }
        };
        }
        else
        {
            // Default features for learners
            features = _context.AboutFeatures.ToList();
        }

        var courses = _context.Courses.ToList();
        var testimonials = _context.CommunityTestimonials.ToList();
        var highlights = _context.CommunityHighlights.ToList();

        var viewModel = new HomeViewModel
        {
            Features = features,
            Courses = courses,
            Testimonials = testimonials,
            Highlights = highlights
        };

        return View(viewModel);
    }

    public IActionResult Error()
    {
        return View("CustomError");
    }
}