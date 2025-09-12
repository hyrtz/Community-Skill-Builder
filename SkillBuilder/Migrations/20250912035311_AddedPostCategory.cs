using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SkillBuilder.Migrations
{
    /// <inheritdoc />
    public partial class AddedPostCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "CommunityPosts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Artisans",
                keyColumn: "ArtisanId",
                keyValue: "A1111111",
                columns: new[] { "FirstName", "Introduction", "LastName", "UserAvatar", "UserId" },
                values: new object[] { "Alice", "Alice is a skilled pottery artisan teaching hand-building techniques.", "Artisan", "/assets/Avatar/Sample3.ico", "user-1" });

            migrationBuilder.InsertData(
                table: "Artisans",
                columns: new[] { "ArtisanId", "ApplicationFile", "FirstName", "Hometown", "Introduction", "IsApproved", "IsArchived", "LastName", "Profession", "Role", "UserAvatar", "UserId" },
                values: new object[,]
                {
                    { "A1111112", null, "Bob", "Cebu City", "Bob is an expert in woodcarving with 10 years of experience.", false, false, "Builder", "Woodcarving Artisan", "Artisan", "/assets/Avatar/Sample6.ico", "user-2" },
                    { "A1111113", null, "Charlie", "Davao City", "Charlie specializes in traditional and modern weaving techniques.", false, false, "Craftsman", "Weaving Artisan", "Artisan", "/assets/Avatar/Sample9.ico", "user-3" }
                });

            migrationBuilder.UpdateData(
                table: "Communities",
                keyColumn: "Id",
                keyValue: 1,
                column: "CoverImageUrl",
                value: "/uploads/community-banner/PotteryBanner.png");

            migrationBuilder.UpdateData(
                table: "Communities",
                keyColumn: "Id",
                keyValue: 2,
                column: "CoverImageUrl",
                value: "/uploads/community-banner/WeavingBanner.png");

            migrationBuilder.UpdateData(
                table: "Communities",
                keyColumn: "Id",
                keyValue: 3,
                column: "CoverImageUrl",
                value: "/uploads/community-banner/WoodCarvingBanner.png");

            migrationBuilder.UpdateData(
                table: "CommunityPosts",
                keyColumn: "Id",
                keyValue: 1,
                column: "Category",
                value: "");

            migrationBuilder.UpdateData(
                table: "CommunityPosts",
                keyColumn: "Id",
                keyValue: 2,
                column: "Category",
                value: "");

            migrationBuilder.UpdateData(
                table: "CommunityPosts",
                keyColumn: "Id",
                keyValue: 3,
                column: "Category",
                value: "");

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "FullDescription", "Overview" },
                values: new object[] { "This course provides a step-by-step guide to both traditional and modern methods of pottery...", "Pottery is the art and craft of shaping and firing clay..." });

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedBy", "FullDescription", "Overview" },
                values: new object[] { "A1111112", "Explore the detailed world of woodcarving through this course...", "Woodcarving is the art of shaping and sculpting wood..." });

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedBy", "FullDescription", "Overview" },
                values: new object[] { "A1111113", "This advanced course in weaving introduces students to both traditional and experimental textile design...", "Weaving is the craft of interlacing threads or fibers..." });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Artisans",
                keyColumn: "ArtisanId",
                keyValue: "A1111112");

            migrationBuilder.DeleteData(
                table: "Artisans",
                keyColumn: "ArtisanId",
                keyValue: "A1111113");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "CommunityPosts");

            migrationBuilder.UpdateData(
                table: "Artisans",
                keyColumn: "ArtisanId",
                keyValue: "A1111111",
                columns: new[] { "FirstName", "Introduction", "LastName", "UserAvatar", "UserId" },
                values: new object[] { "Juan", "Juan is a 3rd-generation artisan teaching pottery for 15 years.", "Dela Cruz", "/assets/Avatar/Sample7.ico", "A1111111" });

            migrationBuilder.UpdateData(
                table: "Communities",
                keyColumn: "Id",
                keyValue: 1,
                column: "CoverImageUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Communities",
                keyColumn: "Id",
                keyValue: 2,
                column: "CoverImageUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Communities",
                keyColumn: "Id",
                keyValue: 3,
                column: "CoverImageUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "FullDescription", "Overview" },
                values: new object[] { "This course provides a step-by-step guide to both traditional and modern methods of pottery. From preparing your clay to understanding kiln temperatures and finishing your work with beautiful glazes, this course is perfect for anyone interested in the craft of ceramics.", "Pottery is the art and craft of shaping and firing clay to create objects like bowls, plates, and decorative items." });

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedBy", "FullDescription", "Overview" },
                values: new object[] { "A1111111", "Explore the detailed world of woodcarving through this course. You'll understand wood grain, learn safe carving practices, and master techniques to transform blocks of wood into detailed figurines, signs, and functional items. Ideal for artists or hobbyists.", "Woodcarving is the art of shaping and sculpting wood into decorative or functional objects." });

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedBy", "FullDescription", "Overview" },
                values: new object[] { "A1111111", "This advanced course in weaving introduces students to both traditional and experimental textile design. Through projects and demonstrations, you’ll master loom warping, color theory in weaving, and understand how weaving traditions influence contemporary fiber arts.", "Weaving is the craft of interlacing threads or fibers to create fabric, textiles, or decorative art." });
        }
    }
}
