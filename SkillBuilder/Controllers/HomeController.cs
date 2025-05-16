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
            new Course{
                Title = "Pottery",
                Link = "/courses/csharp-basics",
                ImageUrl = "/assets/Courses Pics/Pottery.png",
                Description = "Pottery is the art and craft of shaping and firing clay to create objects like bowls, vases, and plates. It can be both functional and decorative, often involving techniques such as wheel-throwing or hand-building, and finished with glazing and kiln firing. Pottery is one of the oldest human crafts, blending creativity with utility.\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n",
                Duration = "15 hours",
                DurationIcon = "/assets/Icons/Time.ico",
                Users = "57.2k",
                UsersIcon = "/assets/Icons/Users.ico",
                Rating = "4.6",
                RatingIcon = "/assets/Icons/Star.ico"
            },

            new Course{
                Title = "Woodcarving",
                Link = "/courses/csharp-basics",
                ImageUrl = "/assets/Courses Pics/Woodcarving.png",
                Description = "Woodcarving is the art of shaping and sculpting wood into decorative or functional objects using tools like knives, chisels, and gouges. It involves cutting and removing parts of the wood to create designs, figures, or patterns. Woodcarving is a traditional craft used in furniture, art, and architecture.",
                Duration = "29 hours",
                DurationIcon = "/assets/Icons/Time.ico",
                Users = "23.4k",
                UsersIcon = "/assets/Icons/Users.ico",
                Rating = "4.9",
                RatingIcon = "/assets/Icons/Star.ico"
            },

            new Course{
                Title = "Weaving",
                Link = "/courses/csharp-basics",
                ImageUrl = "/assets/Courses Pics/Weaving.png",
                Description = "Weaving is the craft of interlacing threads or fibers to create fabric or textiles. It involves crossing warp (vertical) and weft (horizontal) threads on a loom to form patterns and materials. Weaving is used to make clothing, rugs, baskets, and other functional or decorative items, and is one of the oldest forms of textile production.",
                Duration = "18 hours",
                DurationIcon = "/assets/Icons/Time.ico",
                Users = "43.2k",
                UsersIcon = "/assets/Icons/Users.ico",
                Rating = "4.4",
                RatingIcon = "/assets/Icons/Star.ico"
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