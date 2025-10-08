using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillBuilder.Migrations
{
    /// <inheritdoc />
    public partial class AddedIsPublished : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPublished",
                table: "Communities",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Communities",
                keyColumn: "Id",
                keyValue: 1,
                column: "IsPublished",
                value: true);

            migrationBuilder.UpdateData(
                table: "Communities",
                keyColumn: "Id",
                keyValue: 2,
                column: "IsPublished",
                value: true);

            migrationBuilder.UpdateData(
                table: "Communities",
                keyColumn: "Id",
                keyValue: 3,
                column: "IsPublished",
                value: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPublished",
                table: "Communities");
        }
    }
}
