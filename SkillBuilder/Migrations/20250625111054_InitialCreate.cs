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
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false)
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
                    Link = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Duration = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Classes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Level = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Video = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Thumbnail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WhatToLearn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProjectDetails = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Requirements = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
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
                columns: new[] { "Id", "CreatedAt", "Email", "FirstName", "IsVerified", "LastName", "PasswordHash", "Role", "SelectedInterests", "UserAvatar" },
                values: new object[] { "A1111111", new DateTime(2024, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), "juan@example.com", "Juan", true, "Dela Cruz", "hashedpw", "Learner", null, "/assets/Avatar/Sample10.svg" });

            migrationBuilder.InsertData(
                table: "Artisans",
                columns: new[] { "ArtisanId", "FirstName", "Hometown", "Introduction", "LastName", "Profession", "Role", "UserAvatar", "UserId" },
                values: new object[] { "A1111111", "Juan", "Vigan, Ilocos Sur", "Juan is a 3rd-generation artisan teaching pottery for 15 years.", "Dela Cruz", "Pottery Artisan", "Artisan", "/assets/Mentors/juan.png", "A1111111" });

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
                columns: new[] { "Id", "Classes", "CreatedAt", "CreatedBy", "Description", "Duration", "FullDescription", "ImageUrl", "Level", "Link", "ProjectDetails", "Requirements", "Thumbnail", "Title", "Video", "WhatToLearn" },
                values: new object[,]
                {
                    { 1, "Pottery", new DateTime(2024, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "A1111111", "Pottery is the art and craft of shaping and firing clay to create objects like bowls, plates, and decorative items.", "15 hours", "This course provides a step-by-step guide to both traditional and modern methods of pottery. From preparing your clay to understanding kiln temperatures and finishing your work with beautiful glazes, this course is perfect for anyone interested in the craft of ceramics.", "/assets/Courses Pics/Pottery.png", "Beginner", "pottery", "You'll complete a personal project: creating a glazed bowl or cup using wheel-throwing techniques.", "Clay, a pottery wheel or hand-building tools, access to a kiln, apron, and sponges.", "/assets/Courses Pics/Pottery.png", "Pottery", "/assets/Videos/Pottery.mp4", "You'll learn pottery basics, hand-building, wheel throwing, and glazing techniques." },
                    { 2, "Wood Carving", new DateTime(2024, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "A1111111", "Woodcarving is the art of shaping and sculpting wood into decorative or functional objects.", "29 hours", "Explore the detailed world of woodcarving through this course. You'll understand wood grain, learn safe carving practices, and master techniques to transform blocks of wood into detailed figurines, signs, and functional items. Ideal for artists or hobbyists.", "/assets/Courses Pics/Woodcarving.png", "Intermediate", "woodcarving", "Create your own carved decorative panel or wooden sculpture using techniques learned throughout the modules.", "Carving knives, gouges, mallet, sandpaper, safety gloves, and carving wood (basswood recommended).", "/assets/Courses Pics/Woodcarving.png", "Woodcarving", "/assets/Videos/Wood Carving.mp4", "You'll learn carving techniques like relief carving, whittling, chip carving, and finishing." },
                    { 3, "Weaving", new DateTime(2024, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "A1111111", "Weaving is the craft of interlacing threads or fibers to create fabric, textiles, or decorative art.", "18 hours", "This advanced course in weaving introduces students to both traditional and experimental textile design. Through projects and demonstrations, you’ll master loom warping, color theory in weaving, and understand how weaving traditions influence contemporary fiber arts.", "/assets/Courses Pics/Weaving.png", "Professional", "weaving", "You’ll complete a full-sized tapestry or wearable woven piece using your own pattern and chosen materials.", "Table or floor loom, warp and weft yarns, weaving comb, shuttles, and scissors.", "/assets/Courses Pics/Weaving.png", "Weaving", "/assets/Videos/Weaving.mp4", "You’ll explore techniques in tapestry weaving, loom setup, fiber selection, and pattern creation." }
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

            migrationBuilder.CreateIndex(
                name: "IX_Admins_UserId",
                table: "Admins",
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AboutFeatures");

            migrationBuilder.DropTable(
                name: "Admins");

            migrationBuilder.DropTable(
                name: "ArtisanWorks");

            migrationBuilder.DropTable(
                name: "CommunityHighlights");

            migrationBuilder.DropTable(
                name: "CommunityPosts");

            migrationBuilder.DropTable(
                name: "CommunityTestimonials");

            migrationBuilder.DropTable(
                name: "CourseProjectSubmissions");

            migrationBuilder.DropTable(
                name: "CourseReviews");

            migrationBuilder.DropTable(
                name: "Enrollments");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "Artisans");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
