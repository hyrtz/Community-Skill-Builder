using Microsoft.AspNetCore.Mvc;
using SkillBuilder.Models;

namespace SkillBuilder.Controllers
{
    [Route("Courses")]
    public class CoursesController : Controller
    {
        [HttpGet("")]
        public IActionResult CourseCatalog(string? selectedCourse = null)
        {
            ViewData["UseCourseNavbar"] = true;

            var courses = GetCourseList();

            if (!string.IsNullOrEmpty(selectedCourse))
            {
                var course = courses.FirstOrDefault(c => c.Link == selectedCourse);
                if (course == null)
                    return NotFound();

                ViewData["ShowCourseDetails"] = true;
                return View("CourseCatalog", new CourseCatalogViewModel
                {
                    Courses = courses,
                    SelectedCourse = course
                });
            }

            ViewData["ShowCourseDetails"] = false;

            return View("CourseCatalog", new CourseCatalogViewModel
            {
                Courses = courses,
                SelectedCourse = null
            });
        }

        private List<Course> GetCourseList()
        {
            return new List<Course>
            {
                new Course {
                    Title = "Pottery",
                    Link = "pottery",
                    ImageUrl = "/assets/Courses Pics/Pottery.png",
                    Description = "Pottery is the art and craft of shaping and firing clay to create objects like bowls, plates, and decorative items.",
                    Duration = "15 hours",
                    DurationIcon = "/assets/Icons/Time.ico",
                    Users = "57.2k",
                    UsersIcon = "/assets/Icons/Users.ico",
                    Rating = "4.6",
                    RatingIcon = "/assets/Icons/Star.ico",
                    Classes = "Pottery",
                    Level = "Beginner",
                    Video = "/assets/Videos/Pottery.mp4",
                    Thumbnail = "/assets/Courses Pics/Pottery.png",
                    WhatToLearn = "You'll learn pottery basics, hand-building, wheel throwing, and glazing techniques.",
                    FullDescription = "This course provides a step-by-step guide to both traditional and modern methods of pottery. From preparing your clay to understanding kiln temperatures and finishing your work with beautiful glazes, this course is perfect for anyone interested in the craft of ceramics.",
                    ProjectDetails = "You'll complete a personal project: creating a glazed bowl or cup using wheel-throwing techniques.",
                    Requirements = "Clay, a pottery wheel or hand-building tools, access to a kiln, apron, and sponges.",
                    CreatedBy = "Juan Dela Cruz"
                },
                new Course {
                    Title = "Woodcarving",
                    Link = "woodcarving",
                    ImageUrl = "/assets/Courses Pics/Woodcarving.png",
                    Description = "Woodcarving is the art of shaping and sculpting wood into decorative or functional objects.",
                    Duration = "29 hours",
                    DurationIcon = "/assets/Icons/Time.ico",
                    Users = "23.4k",
                    UsersIcon = "/assets/Icons/Users.ico",
                    Rating = "4.9",
                    RatingIcon = "/assets/Icons/Star.ico",
                    Classes = "Wood Carving",
                    Level = "Intermediate",
                    Video = "/assets/Videos/Wood Carving.mp4",
                    Thumbnail = "/assets/Courses Pics/Woodcarving.png",
                    WhatToLearn = "You'll learn carving techniques like relief carving, whittling, chip carving, and finishing.",
                    FullDescription = "Explore the detailed world of woodcarving through this course. You'll understand wood grain, learn safe carving practices, and master techniques to transform blocks of wood into detailed figurines, signs, and functional items. Ideal for artists or hobbyists.",
                    ProjectDetails = "Create your own carved decorative panel or wooden sculpture using techniques learned throughout the modules.",
                    Requirements = "Carving knives, gouges, mallet, sandpaper, safety gloves, and carving wood (basswood recommended).",
                    CreatedBy = "Maria Santos"
                },
                new Course {
                    Title = "Weaving",
                    Link = "weaving",
                    ImageUrl = "/assets/Courses Pics/Weaving.png",
                    Description = "Weaving is the craft of interlacing threads or fibers to create fabric, textiles, or decorative art.",
                    Duration = "18 hours",
                    DurationIcon = "/assets/Icons/Time.ico",
                    Users = "43.2k",
                    UsersIcon = "/assets/Icons/Users.ico",
                    Rating = "4.4",
                    RatingIcon = "/assets/Icons/Star.ico",
                    Classes = "Weaving",
                    Level = "Professional",
                    Video = "/assets/Videos/Weaving.mp4",
                    Thumbnail = "/assets/Courses Pics/Weaving.png",
                    WhatToLearn = "You’ll explore techniques in tapestry weaving, loom setup, fiber selection, and pattern creation.",
                    FullDescription = "This advanced course in weaving introduces students to both traditional and experimental textile design. Through projects and demonstrations, you’ll master loom warping, color theory in weaving, and understand how weaving traditions influence contemporary fiber arts.",
                    ProjectDetails = "You’ll complete a full-sized tapestry or wearable woven piece using your own pattern and chosen materials.",
                    Requirements = "Table or floor loom, warp and weft yarns, weaving comb, shuttles, and scissors.",
                    CreatedBy = "Aling Pining"
                }
            };
        }
    }
}