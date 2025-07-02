using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillBuilder.Migrations
{
    /// <inheritdoc />
    public partial class RealMediaUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ModuleContents",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ContentType", "MediaUrl" },
                values: new object[] { "Video", "/assets/Videos/Pottery.mp4" });

            migrationBuilder.UpdateData(
                table: "ModuleContents",
                keyColumn: "Id",
                keyValue: 2,
                column: "MediaUrl",
                value: "/assets/Courses Pics/Pottery.png");

            migrationBuilder.UpdateData(
                table: "ModuleContents",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "ContentType", "MediaUrl" },
                values: new object[] { "Video", "/assets/Videos/Wood Carving.mp4" });

            migrationBuilder.UpdateData(
                table: "ModuleContents",
                keyColumn: "Id",
                keyValue: 6,
                column: "MediaUrl",
                value: "/assets/Courses Pics/Woodcarving.png");

            migrationBuilder.UpdateData(
                table: "ModuleContents",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "ContentType", "MediaUrl" },
                values: new object[] { "Video", "/assets/Videos/Weaving.mp4" });

            migrationBuilder.UpdateData(
                table: "ModuleContents",
                keyColumn: "Id",
                keyValue: 10,
                column: "MediaUrl",
                value: "/assets/Courses Pics/Weaving.png");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ModuleContents",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ContentType", "MediaUrl" },
                values: new object[] { "Text", null });

            migrationBuilder.UpdateData(
                table: "ModuleContents",
                keyColumn: "Id",
                keyValue: 2,
                column: "MediaUrl",
                value: "/assets/Images/pottery-history.jpg");

            migrationBuilder.UpdateData(
                table: "ModuleContents",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "ContentType", "MediaUrl" },
                values: new object[] { "Text", null });

            migrationBuilder.UpdateData(
                table: "ModuleContents",
                keyColumn: "Id",
                keyValue: 6,
                column: "MediaUrl",
                value: "/assets/Images/woodcarving-history.jpg");

            migrationBuilder.UpdateData(
                table: "ModuleContents",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "ContentType", "MediaUrl" },
                values: new object[] { "Text", null });

            migrationBuilder.UpdateData(
                table: "ModuleContents",
                keyColumn: "Id",
                keyValue: 10,
                column: "MediaUrl",
                value: "/assets/Images/weaving-history.jpg");
        }
    }
}
