using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SkillBuilder.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AboutFeatures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IconPath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AboutFeatures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CommunityHighlights",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Avatar = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Likes = table.Column<int>(type: "int", nullable: false),
                    Comments = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommunityHighlights", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CommunityTestimonials",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AvatarPath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommunityTestimonials", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false),
                    UserAvatar = table.Column<string>(type: "nvarchar(max)", nullable: true, defaultValue: "/assets/Avatar/Sample10.svg"),
                    Points = table.Column<int>(type: "int", nullable: false),
                    SelectedInterests = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Admins",
                columns: table => new
                {
                    AdminId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserAvatar = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admins", x => x.AdminId);
                    table.ForeignKey(
                        name: "FK_Admins_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ArtisanApplications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Profession = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Hometown = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AboutArtisan = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArtisanApplications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArtisanApplications_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Artisans",
                columns: table => new
                {
                    ArtisanId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Profession = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Hometown = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserAvatar = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Introduction = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Artisans", x => x.ArtisanId);
                    table.ForeignKey(
                        name: "FK_Artisans_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CommunityPosts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AuthorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsPublished = table.Column<bool>(type: "bit", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommunityPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommunityPosts_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ArtisanWorks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArtisanId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Caption = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArtisanWorks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArtisanWorks_Artisans_ArtisanId",
                        column: x => x.ArtisanId,
                        principalTable: "Artisans",
                        principalColumn: "ArtisanId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Link = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Overview = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Duration = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Difficulty = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Video = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Thumbnail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WhatToLearn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FullDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProjectDetails = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Requirements = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsPublished = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Courses_Artisans_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Artisans",
                        principalColumn: "ArtisanId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CourseMaterials",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileSize = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseMaterials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseMaterials_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CourseModules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseModules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseModules_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CourseProjectSubmissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseProjectSubmissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseProjectSubmissions_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CourseProjectSubmissions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CourseReviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseReviews_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CourseReviews_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Enrollments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    EnrolledAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enrollments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Enrollments_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Enrollments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SupportSessionRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SessionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SessionTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MeetingLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MeetingPlatform = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConfirmedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupportSessionRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupportSessionRequests_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SupportSessionRequests_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ModuleContents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseModuleId = table.Column<int>(type: "int", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContentText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MediaUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Duration = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Order = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModuleContents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModuleContents_CourseModules_CourseModuleId",
                        column: x => x.CourseModuleId,
                        principalTable: "CourseModules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ModuleProgress",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CourseModuleId = table.Column<int>(type: "int", nullable: false),
                    CompletedModules = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModuleProgress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModuleProgress_CourseModules_CourseModuleId",
                        column: x => x.CourseModuleId,
                        principalTable: "CourseModules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuizQuestions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Question = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OptionA = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OptionB = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OptionC = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OptionD = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CorrectAnswer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModuleContentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuizQuestions_ModuleContents_ModuleContentId",
                        column: x => x.ModuleContentId,
                        principalTable: "ModuleContents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AboutFeatures",
                columns: new[] { "Id", "Description", "IconPath", "Title" },
                values: new object[,]
                {
                    { 1, "Detailed learning paths from beginner to professional levels in traditional and contemporary art forms.", "/assets/Icons/Course.ico", "Structured Course" },
                    { 2, "Share insights, feedback, and experiences with fellow learners and master artisans.", "/assets/Icons/Community.ico", "Community Engagement" },
                    { 3, "Scheduled real-time query sessions with course instructor for personalized guidance.", "/assets/Icons/Sessions.ico", "Live Sessions" },
                    { 4, "Download courses for offline learning, ensuring accessibility regardless of internet connectivity.", "/assets/Icons/Download.ico", "Offline Access" }
                });

            migrationBuilder.InsertData(
                table: "CommunityHighlights",
                columns: new[] { "Id", "Avatar", "Comment", "Comments", "Image", "Likes", "Name", "Role" },
                values: new object[,]
                {
                    { 1, "/assets/Avatar/Sample1.ico", "Lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt lorem ipsum dolor sit amet.", 36, "/assets/Community Pics/CompletePottery.png", 128, "Maria Santos", "Learner" },
                    { 2, "/assets/Avatar/Sample9.ico", "Lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt lorem ipsum dolor sit amet.", 18, "/assets/Community Pics/CompleteWeaving.png", 89, "James dela Cruz", "Artisan" },
                    { 3, "/assets/Avatar/Sample5.ico", "Lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt lorem ipsum dolor sit amet.", 41, "/assets/Community Pics/CompleteWoodcarving.png", 212, "Kim Navarro", "Researcher" }
                });

            migrationBuilder.InsertData(
                table: "CommunityTestimonials",
                columns: new[] { "Id", "AvatarPath", "Comment", "Role", "UserName" },
                values: new object[,]
                {
                    { 1, "/assets/Avatar/Sample1.ico", "Our platform addresses the urgent need to preserve Philippine cultural and traditional art skills that are at risk of disappearing due to modernization.", "Learner", "Maria Santos" },
                    { 2, "/assets/Avatar/Sample2.ico", "Lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt. Lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor.", "Researcher", "Denise Velasco" },
                    { 3, "/assets/Avatar/Sample3.ico", "Lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt.", "Artisan", "Pamela Cruz" },
                    { 4, "/assets/Avatar/Sample4.ico", "Lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor aaa aaa aaa  incididunt lorem ipsum dolor sit.", "Artisan", "Angela Tiz" },
                    { 5, "/assets/Avatar/Sample5.ico", "Lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt. Lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor.", "Artisan", "Marlene Qul" },
                    { 6, "/assets/Avatar/Sample6.ico", "Lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt lorem ipsum dolor sit amet, consectetur.", "Artisan", "Brad Kiminda" },
                    { 7, "/assets/Avatar/Sample7.ico", "Lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt. Lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor.", "Artisan", "Michael Ramirez" },
                    { 8, "/assets/Avatar/Sample8.ico", "Lorem ipsum dolor sit amet, consectetur on aa aa aa aa adipiscing elit, eiusmod tempor incididunt lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod.", "Artisan", "Ella Parilla" },
                    { 9, "/assets/Avatar/Sample9.ico", "Lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt lorem .   aaa aaa aa aaa aa aa aa aa  ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt. Lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor.", "Artisan", "James Dawg" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "FirstName", "IsVerified", "LastName", "PasswordHash", "Points", "Role", "SelectedInterests", "UserAvatar" },
                values: new object[] { "A1111111", new DateTime(2024, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), "juan@example.com", "Juan", true, "Dela Cruz", "hashedpw", 0, "Learner", null, "/assets/Avatar/Sample10.svg" });

            migrationBuilder.InsertData(
                table: "Artisans",
                columns: new[] { "ArtisanId", "FirstName", "Hometown", "Introduction", "IsApproved", "LastName", "Profession", "Role", "UserAvatar", "UserId" },
                values: new object[] { "A1111111", "Juan", "Vigan, Ilocos Sur", "Juan is a 3rd-generation artisan teaching pottery for 15 years.", false, "Dela Cruz", "Pottery Artisan", "Artisan", "/assets/Mentors/juan.png", "A1111111" });

            migrationBuilder.InsertData(
                table: "ArtisanWorks",
                columns: new[] { "Id", "ArtisanId", "Caption", "ImageUrl" },
                values: new object[,]
                {
                    { 1, "A1111111", "Handcraft Pottery", "/assets/Works/JuanWorks1.png" },
                    { 2, "A1111111", "Clay Pot", "/assets/Works/JuanWorks2.png" },
                    { 3, "A1111111", "Pot Elegant", "/assets/Works/JuanWorks3.png" }
                });

            migrationBuilder.InsertData(
                table: "Courses",
                columns: new[] { "Id", "Category", "CreatedAt", "CreatedBy", "Difficulty", "Duration", "FullDescription", "ImageUrl", "IsPublished", "Link", "Overview", "ProjectDetails", "Requirements", "Thumbnail", "Title", "Video", "WhatToLearn" },
                values: new object[,]
                {
                    { 1, "Pottery", new DateTime(2024, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "A1111111", "Beginner", "15 hours", "This course provides a step-by-step guide to both traditional and modern methods of pottery. From preparing your clay to understanding kiln temperatures and finishing your work with beautiful glazes, this course is perfect for anyone interested in the craft of ceramics.", "/assets/Courses Pics/Pottery.png", false, "pottery", "Pottery is the art and craft of shaping and firing clay to create objects like bowls, plates, and decorative items.", "You'll complete a personal project: creating a glazed bowl or cup using wheel-throwing techniques.", "Clay, a pottery wheel or hand-building tools, access to a kiln, apron, and sponges.", "/assets/Courses Pics/Pottery.png", "Pottery", "/assets/Videos/Pottery.mp4", "You'll learn pottery basics, hand-building, wheel throwing, and glazing techniques." },
                    { 2, "Wood Carving", new DateTime(2024, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "A1111111", "Intermediate", "29 hours", "Explore the detailed world of woodcarving through this course. You'll understand wood grain, learn safe carving practices, and master techniques to transform blocks of wood into detailed figurines, signs, and functional items. Ideal for artists or hobbyists.", "/assets/Courses Pics/Woodcarving.png", false, "woodcarving", "Woodcarving is the art of shaping and sculpting wood into decorative or functional objects.", "Create your own carved decorative panel or wooden sculpture using techniques learned throughout the modules.", "Carving knives, gouges, mallet, sandpaper, safety gloves, and carving wood (basswood recommended).", "/assets/Courses Pics/Woodcarving.png", "Woodcarving", "/assets/Videos/Wood Carving.mp4", "You'll learn carving techniques like relief carving, whittling, chip carving, and finishing." },
                    { 3, "Weaving", new DateTime(2024, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "A1111111", "Professional", "18 hours", "This advanced course in weaving introduces students to both traditional and experimental textile design. Through projects and demonstrations, you’ll master loom warping, color theory in weaving, and understand how weaving traditions influence contemporary fiber arts.", "/assets/Courses Pics/Weaving.png", false, "weaving", "Weaving is the craft of interlacing threads or fibers to create fabric, textiles, or decorative art.", "You’ll complete a full-sized tapestry or wearable woven piece using your own pattern and chosen materials.", "Table or floor loom, warp and weft yarns, weaving comb, shuttles, and scissors.", "/assets/Courses Pics/Weaving.png", "Weaving", "/assets/Videos/Weaving.mp4", "You’ll explore techniques in tapestry weaving, loom setup, fiber selection, and pattern creation." }
                });

            migrationBuilder.InsertData(
                table: "CourseModules",
                columns: new[] { "Id", "CourseId", "Order", "Title" },
                values: new object[,]
                {
                    { 1, 1, 0, "Introduction" },
                    { 2, 1, 0, "History" },
                    { 3, 1, 0, "Session" },
                    { 4, 1, 0, "Quiz" },
                    { 5, 1, 0, "Final Activity" },
                    { 6, 2, 0, "Introduction" },
                    { 7, 2, 0, "History" },
                    { 8, 2, 0, "Session" },
                    { 9, 2, 0, "Quiz" },
                    { 10, 2, 0, "Final Activity" },
                    { 11, 3, 0, "Introduction" },
                    { 12, 3, 0, "History" },
                    { 13, 3, 0, "Session" },
                    { 14, 3, 0, "Quiz" },
                    { 15, 3, 0, "Final Activity" }
                });

            migrationBuilder.InsertData(
                table: "CourseReviews",
                columns: new[] { "Id", "Comment", "CourseId", "CreatedAt", "Rating", "UserId" },
                values: new object[,]
                {
                    { 1, "An excellent course! The mentor was very knowledgeable.", 1, new DateTime(2024, 6, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, "A1111111" },
                    { 2, "Really enjoyed learning pottery. A few lessons were a bit fast though.", 1, new DateTime(2024, 6, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, "A1111111" },
                    { 3, "Very hands-on and engaging! Perfect for intermediate learners.", 3, new DateTime(2024, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, "A1111111" }
                });

            migrationBuilder.InsertData(
                table: "ModuleContents",
                columns: new[] { "Id", "ContentText", "ContentType", "CourseModuleId", "Duration", "MediaUrl", "Order", "Title" },
                values: new object[,]
                {
                    { 1, "Pottery intro content something anything.", "Video", 1, null, "/assets/Videos/Pottery.mp4", 0, "Welcome to Pottery" },
                    { 2, "History content something anything.", "Image + Text", 2, null, "/assets/Courses Pics/Pottery.png", 0, "History of Pottery" },
                    { 3, null, "Session", 3, null, null, 0, "Live Pottery Session" },
                    { 4, "Create your own pottery something anything.", "Activity", 5, null, null, 0, "Pottery Final Activity" },
                    { 5, "Woodcarving intro content something anything.", "Video", 6, null, "/assets/Videos/Wood Carving.mp4", 0, "Welcome to Woodcarving" },
                    { 6, "History content something anything.", "Image + Text", 7, null, "/assets/Courses Pics/Woodcarving.png", 0, "History of Woodcarving" },
                    { 7, null, "Session", 8, null, null, 0, "Live Woodcarving Session" },
                    { 8, "Carve a wooden spoon something anything.", "Activity", 10, null, null, 0, "Woodcarving Final Activity" },
                    { 9, "Weaving intro content something anything.", "Video", 11, null, "/assets/Videos/Weaving.mp4", 0, "Welcome to Weaving" },
                    { 10, "History content something anything.", "Image + Text", 12, null, "/assets/Courses Pics/Weaving.png", 0, "History of Weaving" },
                    { 11, null, "Session", 13, null, null, 0, "Live Weaving Session" },
                    { 12, "Weave a basic pattern something anything.", "Activity", 15, null, null, 0, "Weaving Final Activity" },
                    { 13, null, "Quiz", 4, null, null, 0, "Pottery Quiz" },
                    { 14, null, "Quiz", 9, null, null, 0, "Woodcarving Quiz" },
                    { 15, null, "Quiz", 14, null, null, 0, "Weaving Quiz" }
                });

            migrationBuilder.InsertData(
                table: "QuizQuestions",
                columns: new[] { "Id", "CorrectAnswer", "ModuleContentId", "OptionA", "OptionB", "OptionC", "OptionD", "Question" },
                values: new object[,]
                {
                    { 1, "Clay", 13, "Wood", "Metal", "Clay", "Plastic", "What is the main material used in pottery?" },
                    { 2, "Wheel Throwing", 13, "Sculpting", "Weaving", "Wheel Throwing", "Casting", "Which technique is used in pottery?" },
                    { 3, "1000°C", 13, "100°C", "400°C", "1000°C", "2000°C", "What temperature does a kiln usually reach?" },
                    { 4, "Coating", 13, "Painting", "Coating", "Mixing", "Breaking", "What is glazing in pottery?" },
                    { 5, "Remove air bubbles", 13, "Decorate it", "Remove air bubbles", "Color it", "Dry it faster", "Why do we wedge clay?" },
                    { 6, "Chisel", 14, "Pencil", "Chisel", "Brush", "Hammer", "What tool is essential in woodcarving?" },
                    { 7, "Basswood", 14, "Oak", "Basswood", "Mahogany", "Pine", "Which wood is best for beginners?" },
                    { 8, "Cutting out small designs", 14, "Removing chips", "Cutting out small designs", "Joining wood", "Painting wood", "What is chip carving?" },
                    { 9, "Both A and B", 14, "Gloves", "Mask", "Both A and B", "None", "Safety gear includes?" },
                    { 10, "Sharp and clean", 14, "Wet", "Rusty", "Sharp and clean", "Scattered", "How should tools be stored?" },
                    { 11, "Loom", 15, "Hook", "Loom", "Needle", "Stick", "What tool is used to hold threads in weaving?" },
                    { 12, "Horizontally", 15, "Vertically", "Diagonally", "Horizontally", "Randomly", "Weft yarns go?" },
                    { 13, "Twill", 15, "Zigzag", "Twill", "Spin", "Knot", "Which is a basic weave pattern?" },
                    { 14, "Design", 15, "Sound", "Texture", "Design", "Hardness", "Color theory helps with?" },
                    { 15, "Structure", 15, "Decoration", "Structure", "Noise", "Glazing", "What is the purpose of warp threads?" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Admins_UserId",
                table: "Admins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ArtisanApplications_UserId",
                table: "ArtisanApplications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Artisans_UserId",
                table: "Artisans",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ArtisanWorks_ArtisanId",
                table: "ArtisanWorks",
                column: "ArtisanId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityPosts_AuthorId",
                table: "CommunityPosts",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseMaterials_CourseId",
                table: "CourseMaterials",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseModules_CourseId",
                table: "CourseModules",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseProjectSubmissions_CourseId",
                table: "CourseProjectSubmissions",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseProjectSubmissions_UserId",
                table: "CourseProjectSubmissions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseReviews_CourseId",
                table: "CourseReviews",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseReviews_UserId",
                table: "CourseReviews",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_CreatedBy",
                table: "Courses",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_CourseId",
                table: "Enrollments",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_UserId",
                table: "Enrollments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ModuleContents_CourseModuleId",
                table: "ModuleContents",
                column: "CourseModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_ModuleProgress_CourseModuleId",
                table: "ModuleProgress",
                column: "CourseModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizQuestions_ModuleContentId",
                table: "QuizQuestions",
                column: "ModuleContentId");

            migrationBuilder.CreateIndex(
                name: "IX_SupportSessionRequests_CourseId",
                table: "SupportSessionRequests",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_SupportSessionRequests_UserId",
                table: "SupportSessionRequests",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AboutFeatures");

            migrationBuilder.DropTable(
                name: "Admins");

            migrationBuilder.DropTable(
                name: "ArtisanApplications");

            migrationBuilder.DropTable(
                name: "ArtisanWorks");

            migrationBuilder.DropTable(
                name: "CommunityHighlights");

            migrationBuilder.DropTable(
                name: "CommunityPosts");

            migrationBuilder.DropTable(
                name: "CommunityTestimonials");

            migrationBuilder.DropTable(
                name: "CourseMaterials");

            migrationBuilder.DropTable(
                name: "CourseProjectSubmissions");

            migrationBuilder.DropTable(
                name: "CourseReviews");

            migrationBuilder.DropTable(
                name: "Enrollments");

            migrationBuilder.DropTable(
                name: "ModuleProgress");

            migrationBuilder.DropTable(
                name: "QuizQuestions");

            migrationBuilder.DropTable(
                name: "SupportSessionRequests");

            migrationBuilder.DropTable(
                name: "ModuleContents");

            migrationBuilder.DropTable(
                name: "CourseModules");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "Artisans");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
