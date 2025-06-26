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
        var features = _context.AboutFeatures.ToList();
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