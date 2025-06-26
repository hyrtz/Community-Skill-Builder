using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillBuilder.Migrations
{
    /// <inheritdoc />
    public partial class ChangeModuleModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ModuleProgress_Users_UserId",
                table: "ModuleProgress");

            migrationBuilder.DropIndex(
                name: "IX_ModuleProgress_UserId",
                table: "ModuleProgress");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "ModuleProgress",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CompletedAt",
                table: "ModuleProgress",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "CompletedModules",
                table: "ModuleProgress",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdated",
                table: "ModuleProgress",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompletedModules",
                table: "ModuleProgress");

            migrationBuilder.DropColumn(
                name: "LastUpdated",
                table: "ModuleProgress");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "ModuleProgress",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CompletedAt",
                table: "ModuleProgress",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ModuleProgress_UserId",
                table: "ModuleProgress",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ModuleProgress_Users_UserId",
                table: "ModuleProgress",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
