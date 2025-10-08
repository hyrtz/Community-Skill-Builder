using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillBuilder.Migrations
{
    /// <inheritdoc />
    public partial class AddReportPost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CommunityPostReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PostId = table.Column<int>(type: "int", nullable: false),
                    ReporterId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Details = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReportedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommunityPostReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommunityPostReports_CommunityPosts_PostId",
                        column: x => x.PostId,
                        principalTable: "CommunityPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommunityPostReports_PostId",
                table: "CommunityPostReports",
                column: "PostId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommunityPostReports");
        }
    }
}
