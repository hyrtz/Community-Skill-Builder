using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillBuilder.Migrations
{
    /// <inheritdoc />
    public partial class SetCourseToPublish : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 1,
                column: "IsPublished",
                value: true);

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 2,
                column: "IsPublished",
                value: true);

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 3,
                column: "IsPublished",
                value: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 1,
                column: "IsPublished",
                value: false);

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 2,
                column: "IsPublished",
                value: false);

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 3,
                column: "IsPublished",
                value: false);
        }
    }
}
