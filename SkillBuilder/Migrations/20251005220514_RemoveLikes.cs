using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillBuilder.Migrations
{
    /// <inheritdoc />
    public partial class RemoveLikes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Likes",
                table: "CommunityHighlights");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Likes",
                table: "CommunityHighlights",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "CommunityHighlights",
                keyColumn: "Id",
                keyValue: 1,
                column: "Likes",
                value: 128);

            migrationBuilder.UpdateData(
                table: "CommunityHighlights",
                keyColumn: "Id",
                keyValue: 2,
                column: "Likes",
                value: 89);

            migrationBuilder.UpdateData(
                table: "CommunityHighlights",
                keyColumn: "Id",
                keyValue: 3,
                column: "Likes",
                value: 212);
        }
    }
}
