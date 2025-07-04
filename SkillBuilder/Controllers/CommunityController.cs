using Microsoft.AspNetCore.Mvc;

namespace SkillBuilder.Controllers
{
    public class CommunityController : Controller
    {
        public IActionResult CommunityHub(string? search)
        {
            ViewData["UseCourseNavbar"] = true;
            ViewData["SearchQuery"] = search;


            return View();
        }
    }
}