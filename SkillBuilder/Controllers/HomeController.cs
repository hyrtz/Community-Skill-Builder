using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using SkillBuilder.Models;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        var features = new List<AboutFeature>
        {
            new AboutFeature {
                Title = "Structured Courses",
                Description = "Detailed learning paths from beginner to professional levels in traditional and contemporary art forms.",
                IconPath = "/assets/Icons/Course.ico" },

            new AboutFeature {
                Title = "Community Engagement",
                Description = "Share insights, feedback, and experiences with fellow learners and master artisans.",
                IconPath = "/assets/Icons/Community.ico" },

            new AboutFeature {
                Title = "Live Sessions",
                Description = "Scheduled real-time query sessions with course instructors for personalized guidance.",
                IconPath = "/assets/Icons/Sessions.ico" },

            new AboutFeature {
                Title = "Offline Access",
                Description = "Download courses for offline learning, ensuring accessibility regardless of internet connectivity.",
                IconPath = "/assets/Icons/Download.ico" },
        };

        var courses = new List<Course>
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
                Video = "/wwwroot/assets/Videos/Pottery.mp4",
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
                Video = "/wwwroot/assets/Videos/Wood Carving.mp4",
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
                Video = "/wwwroot/assets/Videos/Weaving.mp4",
                Thumbnail = "/assets/Courses Pics/Weaving.png",
                WhatToLearn = "You’ll explore techniques in tapestry weaving, loom setup, fiber selection, and pattern creation.",
                FullDescription = "This advanced course in weaving introduces students to both traditional and experimental textile design. Through projects and demonstrations, you’ll master loom warping, color theory in weaving, and understand how weaving traditions influence contemporary fiber arts.",
                ProjectDetails = "You’ll complete a full-sized tapestry or wearable woven piece using your own pattern and chosen materials.",
                Requirements = "Table or floor loom, warp and weft yarns, weaving comb, shuttles, and scissors.",
                CreatedBy = "Aling Pining"
            }

        };

        var testimonials = new List<CommunityTestimonial>
        {
            new CommunityTestimonial
            {
                Comment = "Our platform addresses the urgent need to aaaaaa preserve Philippine cultural and aaa traditional art skills that are at risk of aaaa disappearing due to modernization.",
                AvatarPath = "/assets/Avatar/Sample1.ico",
                UserName = "Maria Santos",
                Role = "Learner"
            },
            new CommunityTestimonial
            {
                Comment = "Lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt. Lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor.",
                AvatarPath = "/assets/Avatar/Sample2.ico",
                UserName = "Denise Velasco",
                Role = "Researcher"
            },
            new CommunityTestimonial
            {
                Comment = "Lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt. ",
                AvatarPath = "/assets/Avatar/Sample3.ico",
                UserName = "Pamela Cruz",
                Role = "Artisan"
            },
            new CommunityTestimonial
            {
                Comment = "Lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor aaa aaa aaa  incididunt lorem ipsum dolor sit.",
                AvatarPath = "/assets/Avatar/Sample4.ico",
                UserName = "Angela Tiz",
                Role = "Artisan"
            },
            new CommunityTestimonial
            {
                Comment = "Lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt. Lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor.",
                AvatarPath = "/assets/Avatar/Sample5.ico",
                UserName = "Marlene Qul",
                Role = "Artisan"
            },
            new CommunityTestimonial
            {
                Comment = "Lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt lorem ipsum dolor sit amet, consectetur.",
                AvatarPath = "/assets/Avatar/Sample6.ico",
                UserName = "Brad Kiminda",
                Role = "Artisan"
            },
            new CommunityTestimonial
            {
                Comment = "Lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt. Lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor.",
                AvatarPath = "/assets/Avatar/Sample7.ico",
                UserName = "Michael Ramirez",
                Role = "Artisan"
            },
            new CommunityTestimonial
            {
                Comment = "Lorem ipsum dolor sit amet, consectetur on aa aa aa aa adipiscing elit, eiusmod tempor incididunt lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod.",
                AvatarPath = "/assets/Avatar/Sample8.ico",
                UserName = "Ella Parilla",
                Role = "Artisan"
            },
            new CommunityTestimonial
            {
                Comment = "Lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt lorem .   aaa aaa aa aaa aa aa aa aa  ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt. Lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor.",
                AvatarPath = "/assets/Avatar/Sample9.ico",
                UserName = "James Dawg",
                Role = "Artisan"
            }

        };


        var highlights = new List<CommunityHighlight>
        {
            new CommunityHighlight {
                Name = "Maria Santos",
                Role = "Learner",
                Avatar = "/assets/Avatar/Sample1.ico",
                Comment = "Lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt lorem ipsum dolor sit amet.",
                Image = "/assets/Community Pics/CompletePottery.png",
                Likes = 128,
                Comments = 36
            },
            new CommunityHighlight {
                Name = "James dela Cruz",
                Role = "Artisan",
                Avatar = "/assets/Avatar/Sample9.ico",
                Comment = "Lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt lorem ipsum dolor sit amet.",
                Image = "/assets/Community Pics/CompleteWeaving.png",
                Likes = 89,
                Comments = 18
            },
            new CommunityHighlight {
                Name = "Kim Navarro",
                Role = "Researcher",
                Avatar = "/assets/Avatar/Sample5.ico",
                Comment = "Lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt lorem ipsum dolor sit amet.",
                Image = "/assets/Community Pics/CompleteWoodcarving.png",
                Likes = 212,
                Comments = 41
            }
        };

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
        return View("CustomError"); // Point to a custom error view if needed
    }
}