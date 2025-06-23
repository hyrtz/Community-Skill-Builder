using Microsoft.AspNetCore.Mvc;

namespace SkillBuilder.Controllers
{
    public class CommunityController : Controller
    {
        public IActionResult CommunityHub()
        {
            ViewData["UseCourseNavbar"] = true;
            return View();
        }
    }
}