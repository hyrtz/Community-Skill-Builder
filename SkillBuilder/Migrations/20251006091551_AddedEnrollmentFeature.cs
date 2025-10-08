using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillBuilder.Migrations
{
    /// <inheritdoc />
    public partial class AddedEnrollmentFeature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DigitalSignatureUrl",
                table: "Enrollments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FinalProjectStatus",
                table: "Enrollments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SignatureUrl",
                table: "CourseProjectSubmissions",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DigitalSignatureUrl",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "FinalProjectStatus",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "SignatureUrl",
                table: "CourseProjectSubmissions");
        }
    }
}
