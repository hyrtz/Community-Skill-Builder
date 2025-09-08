using Microsoft.EntityFrameworkCore;
using SkillBuilder.Models;

namespace SkillBuilder.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<UserInterest> UserInterests { get; set; }
        public DbSet<CourseProjectSubmission> CourseProjectSubmissions { get; set; }
        public DbSet<Artisan> Artisans { get; set; }
        public DbSet<ArtisanApplication> ArtisanApplications { get; set; }
        public DbSet<ArtisanWork> ArtisanWorks { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<AboutFeature> AboutFeatures { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<CourseModule> CourseModules { get; set; }
        public DbSet<CourseMaterial> CourseMaterials { get; set; }
        public DbSet<ModuleContent> ModuleContents { get; set; }
        public DbSet<QuizQuestion> QuizQuestions { get; set; }
        public DbSet<ModuleProgress> ModuleProgress { get; set; }
        public DbSet<SupportSessionRequest> SupportSessionRequests { get; set; }
        public DbSet<CourseReview> CourseReviews { get; set; }
        public DbSet<CommunityTestimonial> CommunityTestimonials { get; set; }
        public DbSet<CommunityHighlight> CommunityHighlights { get; set; }
        public DbSet<CommunityPost> CommunityPosts { get; set; }
        public DbSet<Community> Communities { get; set; }
        public DbSet<CommunityMembership> CommunityMemberships { get; set; }

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
                    UserAvatar = "/assets/Avatar/Sample7.ico",
                    Introduction = "Juan is a 3rd-generation artisan teaching pottery for 15 years."
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
            // Seed Courses first
            modelBuilder.Entity<Course>().HasData(
                new Course
                {
                    Id = 1,
                    Title = "Pottery",
                    Link = "pottery",
                    ImageUrl = "/assets/Courses Pics/Pottery.png",
                    Overview = "Pottery is the art and craft of shaping and firing clay to create objects like bowls, plates, and decorative items.",
                    Duration = "15 hours",
                    Category = "Pottery",
                    Difficulty = "Beginner",
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
                    Id = 2,
                    Title = "Woodcarving",
                    Link = "woodcarving",
                    ImageUrl = "/assets/Courses Pics/Woodcarving.png",
                    Overview = "Woodcarving is the art of shaping and sculpting wood into decorative or functional objects.",
                    Duration = "29 hours",
                    Category = "Wood Carving",
                    Difficulty = "Intermediate",
                    Video = "/assets/Videos/WoodCarving.mp4",
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
                    Id = 3,
                    Title = "Weaving",
                    Link = "weaving",
                    ImageUrl = "/assets/Courses Pics/Weaving.png",
                    Overview = "Weaving is the craft of interlacing threads or fibers to create fabric, textiles, or decorative art.",
                    Duration = "18 hours",
                    Category = "Weaving",
                    Difficulty = "Professional",
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

            // Seed ArtisanWorks **after Courses**
            modelBuilder.Entity<ArtisanWork>().HasData(
                new ArtisanWork
                {
                    Id = 1,
                    ArtisanId = "A1111111",
                    CourseId = 1, // Matches existing Pottery course
                    Title = "Classic Handcrafted Pottery",
                    ImageUrl = "/assets/Works/JuanWorks1.png",
                    Caption = "Handcrafted pottery made using traditional Vigan techniques.",
                    PublishDate = new DateTime(2025, 1, 1)
                },
                new ArtisanWork
                {
                    Id = 2,
                    ArtisanId = "A1111111",
                    CourseId = 2, // Matches existing Woodcarving course
                    Title = "Rustic Clay Pot",
                    ImageUrl = "/assets/Works/JuanWorks2.png",
                    Caption = "A functional yet beautiful rustic clay pot for everyday use.",
                    PublishDate = new DateTime(2025, 1, 1)
                },
                new ArtisanWork
                {
                    Id = 3,
                    ArtisanId = "A1111111",
                    CourseId = 3, // Matches existing Weaving course
                    Title = "Elegant Decorative Pot",
                    ImageUrl = "/assets/Works/JuanWorks3.png",
                    Caption = "An elegant pot showcasing intricate carvings and smooth finishing.",
                    PublishDate = new DateTime(2025, 1, 1)
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
                    Image = "/assets/Community Pics/Pottery.png",
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
                    Image = "/assets/Community Pics/Weaving.png",
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
                    Image = "/assets/Community Pics/Woodcarving.png",
                    Likes = 212,
                    Comments = 41
                }
            );

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = "user-1",
                    FirstName = "Alice",
                    LastName = "Artisan",
                    Email = "alice@test.com",
                    PasswordHash = "dummyhash1",
                    Role = "Learner",
                    IsVerified = true,
                    UserAvatar = "/assets/Avatar/Sample3.ico",
                    Points = 10,
                    SelectedInterests = "Crafts, Sewing",
                    CreatedAt = new DateTime(2025, 01, 01, 0, 0, 0, DateTimeKind.Utc)
                },
                new User
                {
                    Id = "user-2",
                    FirstName = "Bob",
                    LastName = "Builder",
                    Email = "bob@test.com",
                    PasswordHash = "dummyhash2",
                    Role = "Learner",
                    IsVerified = false,
                    UserAvatar = "/assets/Avatar/Sample6.ico",
                    Points = 5,
                    SelectedInterests = "Woodwork, DIY",
                    CreatedAt = new DateTime(2025, 01, 01, 0, 0, 0, DateTimeKind.Utc)
                },
                new User
                {
                    Id = "user-3",
                    FirstName = "Charlie",
                    LastName = "Craftsman",
                    Email = "charlie@test.com",
                    PasswordHash = "dummyhash3",
                    Role = "Learner",
                    IsVerified = true,
                    UserAvatar = "/assets/Avatar/Sample9.ico",
                    Points = 20,
                    SelectedInterests = "Painting, Pottery",
                    CreatedAt = new DateTime(2025, 01, 01, 0, 0, 0, DateTimeKind.Utc)
                }
            );

            // Seed Communities
            modelBuilder.Entity<Community>().HasData(
                new Community
                {
                    Id = 1,
                    Name = "Pottery Enthusiasts",
                    Description = "A community for learners and artisans passionate about pottery and ceramics.",
                    AvatarUrl = "/assets/Community Pics/CompletePottery.png",
                    MembersCount = 125,
                    CreatorId = "user-1", // optional
                    CreatedAt = new DateTime(2025, 01, 01, 0, 0, 0, DateTimeKind.Utc)
                },
                new Community
                {
                    Id = 2,
                    Name = "Weaving Circle",
                    Description = "Connecting artisans and learners who love weaving traditional and modern textiles.",
                    AvatarUrl = "/assets/Community Pics/CompleteWeaving.png",
                    MembersCount = 98,
                    CreatorId = "user-2", // optional
                    CreatedAt = new DateTime(2025, 01, 01, 0, 0, 0, DateTimeKind.Utc)
                },
                new Community
                {
                    Id = 3,
                    Name = "Woodcarving Masters",
                    Description = "A space for woodcarvers to share projects, tips, and showcase craftsmanship.",
                    AvatarUrl = "/assets/Community Pics/CompleteWoodcarving.png",
                    MembersCount = 150,
                    CreatorId = "user-3", // optional
                    CreatedAt = new DateTime(2025, 01, 01, 0, 0, 0, DateTimeKind.Utc)
                }
            );

            // Seed CommunityPosts
            modelBuilder.Entity<CommunityPost>().HasData(
                new CommunityPost
                {
                    Id = 1,
                    CommunityId = 1,
                    Title = "First Pottery Creation",
                    Content = "Just finished my first clay pot! Excited to share with everyone.",
                    AuthorId = "user-1",
                    SubmittedAt = new DateTime(2025, 1, 5, 0, 0, 0, DateTimeKind.Utc),
                    ImageUrl = "/assets/Community Pics/CompletePottery.png",
                    IsPublished = true  // ← explicitly added
                },
                new CommunityPost
                {
                    Id = 2,
                    CommunityId = 3,
                    Title = "Carving Progress",
                    Content = "Making progress on a wooden sculpture. Any tips for fine detailing?",
                    AuthorId = "user-3",
                    SubmittedAt = new DateTime(2025, 1, 6, 0, 0, 0, DateTimeKind.Utc),
                    ImageUrl = "/assets/Community Pics/CompleteWoodCarving.png",
                    IsPublished = true
                },
                new CommunityPost
                {
                    Id = 3,
                    CommunityId = 2,
                    Title = "New to Weaving",
                    Content = "Starting weaving for the first time. Happy to join this community!",
                    AuthorId = "user-2",
                    SubmittedAt = new DateTime(2025, 1, 7, 0, 0, 0, DateTimeKind.Utc),
                    ImageUrl = "/assets/Community Pics/CompleteWeaving.png",
                    IsPublished = true
                }
            );

            // USER ⇄ ARTISAN (1:1)
            modelBuilder.Entity<User>()
                .HasOne(u => u.Artisan)
                .WithOne(a => a.User) 
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
                .OnDelete(DeleteBehavior.Restrict);

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

            // COURSE MATERIAL ⇄ COURSE (M:1)
            modelBuilder.Entity<CourseMaterial>()
                .HasOne(cm => cm.Course)
                .WithMany(c => c.Materials)
                .HasForeignKey(cm => cm.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CommunityPost>()
                .HasOne(p => p.Author)
                .WithMany()   // or .WithMany(u => u.Posts) if you track them
                .HasForeignKey(p => p.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CommunityMembership>()
                .HasOne(cm => cm.User)
                .WithMany(u => u.Memberships)
                .HasForeignKey(cm => cm.UserId)
                .OnDelete(DeleteBehavior.Restrict);  // prevents cascade cycles

            modelBuilder.Entity<CommunityMembership>()
                .HasOne(cm => cm.Community)
                .WithMany(c => c.Memberships)
                .HasForeignKey(cm => cm.CommunityId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed CourseModules and ModuleContents for each Course
            modelBuilder.Entity<CourseModule>().HasData(
                new CourseModule { Id = 1, CourseId = 1, Title = "Introduction" },
                new CourseModule { Id = 2, CourseId = 1, Title = "History" },
                new CourseModule { Id = 3, CourseId = 1, Title = "Session" },
                new CourseModule { Id = 4, CourseId = 1, Title = "Quiz" },
                new CourseModule { Id = 5, CourseId = 1, Title = "Final Activity" },

                new CourseModule { Id = 6, CourseId = 2, Title = "Introduction" },
                new CourseModule { Id = 7, CourseId = 2, Title = "History" },
                new CourseModule { Id = 8, CourseId = 2, Title = "Session" },
                new CourseModule { Id = 9, CourseId = 2, Title = "Quiz" },
                new CourseModule { Id = 10, CourseId = 2, Title = "Final Activity" },

                new CourseModule { Id = 11, CourseId = 3, Title = "Introduction" },
                new CourseModule { Id = 12, CourseId = 3, Title = "History" },
                new CourseModule { Id = 13, CourseId = 3, Title = "Session" },
                new CourseModule { Id = 14, CourseId = 3, Title = "Quiz" },
                new CourseModule { Id = 15, CourseId = 3, Title = "Final Activity" }
            );

            modelBuilder.Entity<ModuleContent>().HasData(
                // Pottery Modules
                new ModuleContent { Id = 1, CourseModuleId = 1, Title = "Welcome to Pottery", ContentType = "Video", ContentText = "Pottery intro content something anything.", MediaUrl = "/assets/Videos/Pottery.mp4" },
                new ModuleContent { Id = 2, CourseModuleId = 2, Title = "History of Pottery", ContentType = "Image + Text", ContentText = "History content something anything.", MediaUrl = "/assets/Courses Pics/Pottery.png" },
                new ModuleContent { Id = 3, CourseModuleId = 3, Title = "Live Pottery Session", ContentType = "Session"},
                new ModuleContent { Id = 4, CourseModuleId = 5, Title = "Pottery Final Activity", ContentType = "Activity", ContentText = "Create your own pottery something anything." },

                // Woodcarving Modules
                new ModuleContent { Id = 5, CourseModuleId = 6, Title = "Welcome to Woodcarving", ContentType = "Video", ContentText = "Woodcarving intro content something anything.", MediaUrl = "/assets/Videos/Wood Carving.mp4" },
                new ModuleContent { Id = 6, CourseModuleId = 7, Title = "History of Woodcarving", ContentType = "Image + Text", ContentText = "History content something anything.", MediaUrl = "/assets/Courses Pics/Woodcarving.png" },
                new ModuleContent { Id = 7, CourseModuleId = 8, Title = "Live Woodcarving Session", ContentType = "Session"},
                new ModuleContent { Id = 8, CourseModuleId = 10, Title = "Woodcarving Final Activity", ContentType = "Activity", ContentText = "Carve a wooden spoon something anything." },


                // Weaving Modules
                new ModuleContent { Id = 9, CourseModuleId = 11, Title = "Welcome to Weaving", ContentType = "Video", ContentText = "Weaving intro content something anything.", MediaUrl = "/assets/Videos/Weaving.mp4" },
                new ModuleContent { Id = 10, CourseModuleId = 12, Title = "History of Weaving", ContentType = "Image + Text", ContentText = "History content something anything.", MediaUrl = "/assets/Courses Pics/Weaving.png" },
                new ModuleContent { Id = 11, CourseModuleId = 13, Title = "Live Weaving Session", ContentType = "Session"},
                new ModuleContent { Id = 12, CourseModuleId = 15, Title = "Weaving Final Activity", ContentType = "Activity", ContentText = "Weave a basic pattern something anything." }
            );

            // QUIZ ModuleContent (shared pattern)
            modelBuilder.Entity<ModuleContent>().HasData(
                new ModuleContent { Id = 13, CourseModuleId = 4, Title = "Pottery Quiz", ContentType = "Quiz" },
                new ModuleContent { Id = 14, CourseModuleId = 9, Title = "Woodcarving Quiz", ContentType = "Quiz" },
                new ModuleContent { Id = 15, CourseModuleId = 14, Title = "Weaving Quiz", ContentType = "Quiz" }
            );

            // QUIZ Questions
            modelBuilder.Entity<QuizQuestion>().HasData(
                // Pottery Quiz (ModuleContentId = 13)
                new QuizQuestion { Id = 1, ModuleContentId = 13, Question = "What is the main material used in pottery?", OptionA = "Wood", OptionB = "Metal", OptionC = "Clay", OptionD = "Plastic", CorrectAnswer = "Clay" },
                new QuizQuestion { Id = 2, ModuleContentId = 13, Question = "Which technique is used in pottery?", OptionA = "Sculpting", OptionB = "Weaving", OptionC = "Wheel Throwing", OptionD = "Casting", CorrectAnswer = "Wheel Throwing" },
                new QuizQuestion { Id = 3, ModuleContentId = 13, Question = "What temperature does a kiln usually reach?", OptionA = "100°C", OptionB = "400°C", OptionC = "1000°C", OptionD = "2000°C", CorrectAnswer = "1000°C" },
                new QuizQuestion { Id = 4, ModuleContentId = 13, Question = "What is glazing in pottery?", OptionA = "Painting", OptionB = "Coating", OptionC = "Mixing", OptionD = "Breaking", CorrectAnswer = "Coating" },
                new QuizQuestion { Id = 5, ModuleContentId = 13, Question = "Why do we wedge clay?", OptionA = "Decorate it", OptionB = "Remove air bubbles", OptionC = "Color it", OptionD = "Dry it faster", CorrectAnswer = "Remove air bubbles" },

                // Woodcarving Quiz (ModuleContentId = 14)
                new QuizQuestion { Id = 6, ModuleContentId = 14, Question = "What tool is essential in woodcarving?", OptionA = "Pencil", OptionB = "Chisel", OptionC = "Brush", OptionD = "Hammer", CorrectAnswer = "Chisel" },
                new QuizQuestion { Id = 7, ModuleContentId = 14, Question = "Which wood is best for beginners?", OptionA = "Oak", OptionB = "Basswood", OptionC = "Mahogany", OptionD = "Pine", CorrectAnswer = "Basswood" },
                new QuizQuestion { Id = 8, ModuleContentId = 14, Question = "What is chip carving?", OptionA = "Removing chips", OptionB = "Cutting out small designs", OptionC = "Joining wood", OptionD = "Painting wood", CorrectAnswer = "Cutting out small designs" },
                new QuizQuestion { Id = 9, ModuleContentId = 14, Question = "Safety gear includes?", OptionA = "Gloves", OptionB = "Mask", OptionC = "Both A and B", OptionD = "None", CorrectAnswer = "Both A and B" },
                new QuizQuestion { Id = 10, ModuleContentId = 14, Question = "How should tools be stored?", OptionA = "Wet", OptionB = "Rusty", OptionC = "Sharp and clean", OptionD = "Scattered", CorrectAnswer = "Sharp and clean" },

                // Weaving Quiz (ModuleContentId = 15)
                new QuizQuestion { Id = 11, ModuleContentId = 15, Question = "What tool is used to hold threads in weaving?", OptionA = "Hook", OptionB = "Loom", OptionC = "Needle", OptionD = "Stick", CorrectAnswer = "Loom" },
                new QuizQuestion { Id = 12, ModuleContentId = 15, Question = "Weft yarns go?", OptionA = "Vertically", OptionB = "Diagonally", OptionC = "Horizontally", OptionD = "Randomly", CorrectAnswer = "Horizontally" },
                new QuizQuestion { Id = 13, ModuleContentId = 15, Question = "Which is a basic weave pattern?", OptionA = "Zigzag", OptionB = "Twill", OptionC = "Spin", OptionD = "Knot", CorrectAnswer = "Twill" },
                new QuizQuestion { Id = 14, ModuleContentId = 15, Question = "Color theory helps with?", OptionA = "Sound", OptionB = "Texture", OptionC = "Design", OptionD = "Hardness", CorrectAnswer = "Design" },
                new QuizQuestion { Id = 15, ModuleContentId = 15, Question = "What is the purpose of warp threads?", OptionA = "Decoration", OptionB = "Structure", OptionC = "Noise", OptionD = "Glazing", CorrectAnswer = "Structure" }
            );

            // MODULE PROGRESS ⇄ COURSE MODULE (M:1)
            modelBuilder.Entity<ModuleProgress>()
                .HasOne(mp => mp.CourseModule)
                .WithMany(cm => cm.Progresses)
                .HasForeignKey(mp => mp.CourseModuleId);

            // QUIZ QUESTION ⇄ COURSE MODULE (M:1)
            modelBuilder.Entity<QuizQuestion>()
                .HasOne(q => q.ModuleContent)
                .WithMany(mc => mc.QuizQuestions)
                .HasForeignKey(q => q.ModuleContentId)
                .OnDelete(DeleteBehavior.Cascade);

            // COURSE ⇄ ARTISAN WORKS (1:M)
            modelBuilder.Entity<ArtisanWork>()
                .HasOne(aw => aw.Course)
                .WithMany(c => c.ArtisanWorks)
                .HasForeignKey(aw => aw.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
