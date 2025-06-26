using Microsoft.EntityFrameworkCore;
using SkillBuilder.Models;

namespace SkillBuilder.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<CourseProjectSubmission> CourseProjectSubmissions { get; set; }
        public DbSet<Artisan> Artisans { get; set; }
        public DbSet<ArtisanApplication> ArtisanApplications { get; set; }
        public DbSet<ArtisanWork> ArtisanWorks { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<AboutFeature> AboutFeatures { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<CourseModule> CourseModules { get; set; }
        public DbSet<ModuleProgress> ModuleProgress { get; set; }
        public DbSet<CourseReview> CourseReviews { get; set; }
        public DbSet<CommunityTestimonial> CommunityTestimonials { get; set; }
        public DbSet<CommunityHighlight> CommunityHighlights { get; set; }
        public DbSet<CommunityPost> CommunityPosts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = "A1111111",
                    FirstName = "Juan",
                    LastName = "Dela Cruz",
                    Email = "juan@example.com",
                    PasswordHash = "hashedpw",
                    IsVerified = true,
                    CreatedAt = new DateTime(2024, 6, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );

            // User default value for Avatar
            modelBuilder.Entity<User>()
                .Property(u => u.UserAvatar)
                .HasDefaultValue("/assets/Avatar/Sample10.svg");

            // Artisan
            modelBuilder.Entity<Artisan>().HasData(
                new Artisan
                {
                    ArtisanId = "A1111111",
                    UserId = "A1111111",
                    FirstName = "Juan",
                    LastName = "Dela Cruz",
                    Profession = "Pottery Artisan",
                    Hometown = "Vigan, Ilocos Sur",
                    UserAvatar = "/assets/Mentors/juan.png",
                    Introduction = "Juan is a 3rd-generation artisan teaching pottery for 15 years."
                }
            );

            // Artisan Works
            modelBuilder.Entity<ArtisanWork>().HasData(
                new ArtisanWork
                {
                    Id = 001,
                    ArtisanId = "A1111111",
                    ImageUrl = "/assets/Works/JuanWorks1.png",
                    Caption = "Handcraft Pottery"
                },
                new ArtisanWork
                {
                    Id = 002,
                    ArtisanId = "A1111111",
                    ImageUrl = "/assets/Works/JuanWorks2.png",
                    Caption = "Clay Pot"
                },
                new ArtisanWork
                {
                    Id = 003,
                    ArtisanId = "A1111111",
                    ImageUrl = "/assets/Works/JuanWorks3.png",
                    Caption = "Pot Elegant"
                }
            );

            // About Features
            modelBuilder.Entity<AboutFeature>().HasData(
                new AboutFeature { Id = 001, IconPath = "/assets/Icons/Course.ico", Title = "Structured Course", Description = "Detailed learning paths from beginner to professional levels in traditional and contemporary art forms." },
                new AboutFeature { Id = 002, IconPath = "/assets/Icons/Community.ico", Title = "Community Engagement", Description = "Share insights, feedback, and experiences with fellow learners and master artisans." },
                new AboutFeature { Id = 003, IconPath = "/assets/Icons/Sessions.ico", Title = "Live Sessions", Description = "Scheduled real-time query sessions with course instructor for personalized guidance." },
                new AboutFeature { Id = 004, IconPath = "/assets/Icons/Download.ico", Title = "Offline Access", Description = "Download courses for offline learning, ensuring accessibility regardless of internet connectivity." }
            );

            // Courses
            modelBuilder.Entity<Course>().HasData(
                new Course
                {
                    Id = 00001,
                    Title = "Pottery",
                    Link = "pottery",
                    ImageUrl = "/assets/Courses Pics/Pottery.png",
                    Description = "Pottery is the art and craft of shaping and firing clay to create objects like bowls, plates, and decorative items.",
                    Duration = "15 hours",
                    Classes = "Pottery",
                    Level = "Beginner",
                    Video = "/assets/Videos/Pottery.mp4",
                    Thumbnail = "/assets/Courses Pics/Pottery.png",
                    WhatToLearn = "You'll learn pottery basics, hand-building, wheel throwing, and glazing techniques.",
                    FullDescription = "This course provides a step-by-step guide to both traditional and modern methods of pottery. From preparing your clay to understanding kiln temperatures and finishing your work with beautiful glazes, this course is perfect for anyone interested in the craft of ceramics.",
                    ProjectDetails = "You'll complete a personal project: creating a glazed bowl or cup using wheel-throwing techniques.",
                    Requirements = "Clay, a pottery wheel or hand-building tools, access to a kiln, apron, and sponges.",
                    CreatedBy = "A1111111",
                    CreatedAt = new DateTime(2024, 6, 1)
                },
                new Course
                {
                    Id = 00002,
                    Title = "Woodcarving",
                    Link = "woodcarving",
                    ImageUrl = "/assets/Courses Pics/Woodcarving.png",
                    Description = "Woodcarving is the art of shaping and sculpting wood into decorative or functional objects.",
                    Duration = "29 hours",
                    Classes = "Wood Carving",
                    Level = "Intermediate",
                    Video = "/assets/Videos/Wood Carving.mp4",
                    Thumbnail = "/assets/Courses Pics/Woodcarving.png",
                    WhatToLearn = "You'll learn carving techniques like relief carving, whittling, chip carving, and finishing.",
                    FullDescription = "Explore the detailed world of woodcarving through this course. You'll understand wood grain, learn safe carving practices, and master techniques to transform blocks of wood into detailed figurines, signs, and functional items. Ideal for artists or hobbyists.",
                    ProjectDetails = "Create your own carved decorative panel or wooden sculpture using techniques learned throughout the modules.",
                    Requirements = "Carving knives, gouges, mallet, sandpaper, safety gloves, and carving wood (basswood recommended).",
                    CreatedBy = "A1111111",
                    CreatedAt = new DateTime(2024, 6, 1)
                },
                new Course
                {
                    Id = 00003,
                    Title = "Weaving",
                    Link = "weaving",
                    ImageUrl = "/assets/Courses Pics/Weaving.png",
                    Description = "Weaving is the craft of interlacing threads or fibers to create fabric, textiles, or decorative art.",
                    Duration = "18 hours",
                    Classes = "Weaving",
                    Level = "Professional",
                    Video = "/assets/Videos/Weaving.mp4",
                    Thumbnail = "/assets/Courses Pics/Weaving.png",
                    WhatToLearn = "You’ll explore techniques in tapestry weaving, loom setup, fiber selection, and pattern creation.",
                    FullDescription = "This advanced course in weaving introduces students to both traditional and experimental textile design. Through projects and demonstrations, you’ll master loom warping, color theory in weaving, and understand how weaving traditions influence contemporary fiber arts.",
                    ProjectDetails = "You’ll complete a full-sized tapestry or wearable woven piece using your own pattern and chosen materials.",
                    Requirements = "Table or floor loom, warp and weft yarns, weaving comb, shuttles, and scissors.",
                    CreatedBy = "A1111111",
                    CreatedAt = new DateTime(2024, 6, 1)
                }
            );

            // Course Review
            modelBuilder.Entity<CourseReview>().HasData(
                new CourseReview
                {
                    Id = 1,
                    CourseId = 00001,
                    UserId = "A1111111",
                    Rating = 5,
                    Comment = "An excellent course! The mentor was very knowledgeable.",
                    CreatedAt = new DateTime(2024, 6, 5)
                },
                new CourseReview
                {
                    Id = 2,
                    CourseId = 00001,
                    UserId = "A1111111",
                    Rating = 4,
                    Comment = "Really enjoyed learning pottery. A few lessons were a bit fast though.",
                    CreatedAt = new DateTime(2024, 6, 10)
                },
                new CourseReview
                {
                    Id = 3,
                    CourseId = 00003,
                    UserId = "A1111111",
                    Rating = 5,
                    Comment = "Very hands-on and engaging! Perfect for intermediate learners.",
                    CreatedAt = new DateTime(2024, 6, 15)
                }
            );

            // Community Testimonials
            modelBuilder.Entity<CommunityTestimonial>().HasData(
                new CommunityTestimonial
                {
                    Id = 0000001,
                    Comment = "Our platform addresses the urgent need to preserve Philippine cultural and traditional art skills that are at risk of disappearing due to modernization.",
                    AvatarPath = "/assets/Avatar/Sample1.ico",
                    UserName = "Maria Santos",
                    Role = "Learner"
                },
                new CommunityTestimonial
                {
                    Id = 0000002,
                    Comment = "Lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt. Lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor.",
                    AvatarPath = "/assets/Avatar/Sample2.ico",
                    UserName = "Denise Velasco",
                    Role = "Researcher"
                },
                new CommunityTestimonial
                {
                    Id = 0000003,
                    Comment = "Lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt.",
                    AvatarPath = "/assets/Avatar/Sample3.ico",
                    UserName = "Pamela Cruz",
                    Role = "Artisan"
                },
                new CommunityTestimonial
                {
                    Id = 0000004,
                    Comment = "Lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor aaa aaa aaa  incididunt lorem ipsum dolor sit.",
                    AvatarPath = "/assets/Avatar/Sample4.ico",
                    UserName = "Angela Tiz",
                    Role = "Artisan"
                },
                new CommunityTestimonial
                {
                    Id = 0000005,
                    Comment = "Lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt. Lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor.",
                    AvatarPath = "/assets/Avatar/Sample5.ico",
                    UserName = "Marlene Qul",
                    Role = "Artisan"
                },
                new CommunityTestimonial
                {
                    Id = 0000006,
                    Comment = "Lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt lorem ipsum dolor sit amet, consectetur.",
                    AvatarPath = "/assets/Avatar/Sample6.ico",
                    UserName = "Brad Kiminda",
                    Role = "Artisan"
                },
                new CommunityTestimonial
                {
                    Id = 0000007,
                    Comment = "Lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt. Lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor.",
                    AvatarPath = "/assets/Avatar/Sample7.ico",
                    UserName = "Michael Ramirez",
                    Role = "Artisan"
                },
                new CommunityTestimonial
                {
                    Id = 0000008,
                    Comment = "Lorem ipsum dolor sit amet, consectetur on aa aa aa aa adipiscing elit, eiusmod tempor incididunt lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod.",
                    AvatarPath = "/assets/Avatar/Sample8.ico",
                    UserName = "Ella Parilla",
                    Role = "Artisan"
                },
                new CommunityTestimonial
                {
                    Id = 0000009,
                    Comment = "Lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt lorem .   aaa aaa aa aaa aa aa aa aa  ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt. Lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor.",
                    AvatarPath = "/assets/Avatar/Sample9.ico",
                    UserName = "James Dawg",
                    Role = "Artisan"
                }
            );

            // Community Highlights
            modelBuilder.Entity<CommunityHighlight>().HasData(
                new CommunityHighlight
                {
                    Id = 0000001,
                    Name = "Maria Santos",
                    Role = "Learner",
                    Avatar = "/assets/Avatar/Sample1.ico",
                    Comment = "Lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt lorem ipsum dolor sit amet.",
                    Image = "/assets/Community Pics/CompletePottery.png",
                    Likes = 128,
                    Comments = 36
                },
                new CommunityHighlight
                {
                    Id = 0000002,
                    Name = "James dela Cruz",
                    Role = "Artisan",
                    Avatar = "/assets/Avatar/Sample9.ico",
                    Comment = "Lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt lorem ipsum dolor sit amet.",
                    Image = "/assets/Community Pics/CompleteWeaving.png",
                    Likes = 89,
                    Comments = 18
                },
                new CommunityHighlight
                {
                    Id = 0000003,
                    Name = "Kim Navarro",
                    Role = "Researcher",
                    Avatar = "/assets/Avatar/Sample5.ico",
                    Comment = "Lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt lorem ipsum dolor sit amet.",
                    Image = "/assets/Community Pics/CompleteWoodcarving.png",
                    Likes = 212,
                    Comments = 41
                }
            );

            // USER ⇄ ARTISAN (1:1)
            modelBuilder.Entity<Artisan>()
                .HasOne(a => a.User)
                .WithOne(u => u.Artisan)
                .HasForeignKey<Artisan>(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ARTISAN ⇄ ARTISAN WORKS (1:M)
            modelBuilder.Entity<ArtisanWork>()
                .HasOne(w => w.Artisan)
                .WithMany(a => a.Works)
                .HasForeignKey(w => w.ArtisanId)
                .OnDelete(DeleteBehavior.Cascade);

            // COURSE ⇄ ARTISAN (1:M)
            modelBuilder.Entity<Course>()
                .HasOne(c => c.Artisan)
                .WithMany(a => a.Courses)
                .HasForeignKey(c => c.CreatedBy)
                .OnDelete(DeleteBehavior.Cascade);

            // ENROLLMENT ⇄ USER (M:1)
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.User)
                .WithMany(u => u.Enrollments)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ENROLLMENT ⇄ COURSE (M:1)
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            // COURSE REVIEW ⇄ COURSE (M:1)
            modelBuilder.Entity<CourseReview>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // COURSE REVIEW ⇄ USER (M:1)
            modelBuilder.Entity<CourseReview>()
                .HasOne(r => r.Course)
                .WithMany(c => c.Reviews)
                .HasForeignKey(r => r.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            // COURSE SUBMISSION ⇄ USER (M:1)
            modelBuilder.Entity<CourseProjectSubmission>()
                .HasOne(p => p.User)
                .WithMany(u => u.ProjectSubmissions)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // COURSE SUBMISSION ⇄ COURSE (M:1)
            modelBuilder.Entity<CourseProjectSubmission>()
                .HasOne(p => p.Course)
                .WithMany(c => c.ProjectSubmissions)
                .HasForeignKey(p => p.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            // COURSE MODULE ⇄ COURSE (M:1)
            modelBuilder.Entity<CourseModule>()
                .HasOne(cm => cm.Course)
                .WithMany(c => c.CourseModules)
                .HasForeignKey(cm => cm.CourseId);

            // MODULE PROGRESS ⇄ COURSE MODULE (M:1)
            modelBuilder.Entity<ModuleProgress>()
                .HasOne(mp => mp.CourseModule)
                .WithMany(cm => cm.Progresses)
                .HasForeignKey(mp => mp.CourseModuleId);

        }
    }
}
