using Microsoft.AspNetCore.Mvc;
using SkillBuilder.Models;

namespace SkillBuilder.Controllers
{
    [Route("ArtisanProfile")]
    public class ArtisanProfileController : Controller
    {
        public static class DataStore
        {
            public static List<User> UserList = new List<User>();
            public static List<Artisan> ArtisanList = new List<Artisan>();
            public static List<Course> CourseList = new List<Course>();

            // Static constructor to initialize the lists
            static DataStore()
            {
                var userJuan = new User
                {
                    Id = "A1111111",
                    FirstName = "Juan",
                    LastName = "Dela Cruz",
                    Email = "juan@example.com",
                    PasswordHash = "hashedpw",
                    IsVerified = true,
                    CreatedAt = DateTime.Now
                };

                var artisanJuan = new Artisan
                {
                    ArtisanId = "A1111111",
                    UserId = userJuan.Id,
                    User = userJuan,
                    AvatarUrl = "/assets/Mentors/juan.png",
                    Profession = "Pottery Artisan",
                    Hometown = "Vigan, Ilocos Sur",
                    Introduction = "Juan is a 3rd-generation artisan teaching pottery for 15 years."
                };

                UserList.Add(userJuan);
                ArtisanList.Add(artisanJuan);

                CourseList.Add(new Course
                {
                    Title = "Pottery",
                    Link = "pottery",
                    ImageUrl = "/assets/Courses Pics/Pottery.png",
                    Description = "Pottery is the art and craft of shaping and firing clay...",
                    Duration = "15 hours",
                    DurationIcon = "/assets/Icons/Time.ico",
                    Users = "57.2k",
                    UsersIcon = "/assets/Icons/Users.ico",
                    Rating = "4.6",
                    RatingIcon = "/assets/Icons/Star.ico",
                    Classes = "Pottery",
                    Level = "Beginner",
                    Video = "/assets/Videos/Pottery.mp4",
                    WhatToLearn = "You'll learn pottery basics, hand-building, wheel throwing, and glazing techniques.",
                    FullDescription = "This course provides a step-by-step guide...",
                    ProjectDetails = "You'll complete a personal project: a glazed bowl or cup.",
                    Requirements = "Clay, tools, kiln access, apron, and sponges.",
                    CreatedBy = "A1111111"
                });
            }
        }

        [HttpGet("{id}")]
        public IActionResult ArtisanProfile(string id)
        {
            var artisan = DataStore.ArtisanList.FirstOrDefault(a => a.ArtisanId == id);
            var courses = DataStore.CourseList.Where(c => c.CreatedBy == id).ToList();

            if (artisan == null)
                return NotFound();

            var viewModel = new ArtisanProfileViewModel
            {
                Artisan = artisan,
                Courses = courses
            };

            return View("~/Views/Artisan/ArtisanProfile.cshtml", viewModel);
        }
    }
}