using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IconPath = table.Column<string>(type: "text", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AboutFeatures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CommunityHighlights",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false),
                    Avatar = table.Column<string>(type: "text", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: false),
                    Image = table.Column<string>(type: "text", nullable: false),
                    Comments = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommunityHighlights", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CommunityTestimonials",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Comment = table.Column<string>(type: "text", nullable: false),
                    AvatarPath = table.Column<string>(type: "text", nullable: false),
                    UserName = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommunityTestimonials", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserInterests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    Interest = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInterests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false),
                    IsVerified = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeactivated = table.Column<bool>(type: "boolean", nullable: false),
                    UserAvatar = table.Column<string>(type: "text", nullable: true, defaultValue: "/assets/Avatar/Sample10.svg"),
                    Points = table.Column<int>(type: "integer", nullable: false),
                    SelectedInterests = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EmailVerificationToken = table.Column<string>(type: "text", nullable: true),
                    PasswordResetOtp = table.Column<string>(type: "text", nullable: true),
                    OtpExpiry = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsArchived = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Admins",
                columns: table => new
                {
                    AdminId = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false),
                    UserAvatar = table.Column<string>(type: "text", nullable: false)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Profession = table.Column<string>(type: "text", nullable: false),
                    Hometown = table.Column<string>(type: "text", nullable: false),
                    Introduction = table.Column<string>(type: "text", nullable: false),
                    ApplicationFile = table.Column<string>(type: "text", nullable: true)
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
                    ArtisanId = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    Profession = table.Column<string>(type: "text", nullable: false),
                    Hometown = table.Column<string>(type: "text", nullable: false),
                    ApplicationFile = table.Column<string>(type: "text", nullable: true),
                    UserAvatar = table.Column<string>(type: "text", nullable: false),
                    Introduction = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false),
                    IsApproved = table.Column<bool>(type: "boolean", nullable: false),
                    IsArchived = table.Column<bool>(type: "boolean", nullable: false)
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
                name: "Communities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    AvatarUrl = table.Column<string>(type: "text", nullable: true),
                    CoverImageUrl = table.Column<string>(type: "text", nullable: true),
                    MembersCount = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<string>(type: "text", nullable: false),
                    IsArchived = table.Column<bool>(type: "boolean", nullable: false),
                    IsPublished = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Communities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Communities_Users_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false),
                    ActionText = table.Column<string>(type: "text", nullable: true),
                    ActionUrl = table.Column<string>(type: "text", nullable: true),
                    IsActioned = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Link = table.Column<string>(type: "text", nullable: true),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    Overview = table.Column<string>(type: "text", nullable: false),
                    Duration = table.Column<string>(type: "text", nullable: false),
                    Category = table.Column<string>(type: "text", nullable: false),
                    Difficulty = table.Column<string>(type: "text", nullable: false),
                    Video = table.Column<string>(type: "text", nullable: true),
                    Thumbnail = table.Column<string>(type: "text", nullable: true),
                    WhatToLearn = table.Column<string>(type: "text", nullable: true),
                    FullDescription = table.Column<string>(type: "text", nullable: false),
                    Requirements = table.Column<string>(type: "text", nullable: false),
                    FinalProjectTitle = table.Column<string>(type: "text", nullable: false),
                    FinalProjectDescription = table.Column<string>(type: "text", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsPublished = table.Column<bool>(type: "boolean", nullable: false),
                    IsArchived = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Courses_Artisans_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Artisans",
                        principalColumn: "ArtisanId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CommunityJoinRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CommunityId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    RequestedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    ShortMessage = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommunityJoinRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommunityJoinRequests_Communities_CommunityId",
                        column: x => x.CommunityId,
                        principalTable: "Communities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CommunityJoinRequests_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CommunityMemberships",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    CommunityId = table.Column<int>(type: "integer", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false),
                    JoinedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommunityMemberships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommunityMemberships_Communities_CommunityId",
                        column: x => x.CommunityId,
                        principalTable: "Communities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommunityMemberships_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CommunityPosts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CommunityId = table.Column<int>(type: "integer", nullable: true),
                    AuthorId = table.Column<string>(type: "text", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    Category = table.Column<string>(type: "text", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsPublished = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommunityPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommunityPosts_Communities_CommunityId",
                        column: x => x.CommunityId,
                        principalTable: "Communities",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CommunityPosts_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ArtisanWorks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ArtisanId = table.Column<string>(type: "text", nullable: false),
                    CourseId = table.Column<int>(type: "integer", nullable: true),
                    Title = table.Column<string>(type: "text", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: false),
                    Caption = table.Column<string>(type: "text", nullable: false),
                    PublishDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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
                    table.ForeignKey(
                        name: "FK_ArtisanWorks_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CourseMaterials",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CourseId = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    FileName = table.Column<string>(type: "text", nullable: true),
                    FilePath = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CourseId = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    CourseId = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    MediaUrl = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SignatureUrl = table.Column<string>(type: "text", nullable: true)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CourseId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    Rating = table.Column<int>(type: "integer", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    CourseId = table.Column<int>(type: "integer", nullable: false),
                    EnrolledAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FinalProjectStatus = table.Column<string>(type: "text", nullable: true),
                    DigitalSignatureUrl = table.Column<string>(type: "text", nullable: true)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    CourseId = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false),
                    SessionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SessionTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MeetingLink = table.Column<string>(type: "text", nullable: true),
                    MeetingPlatform = table.Column<string>(type: "text", nullable: true),
                    ConfirmedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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
                name: "CommunityPostReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PostId = table.Column<int>(type: "integer", nullable: false),
                    ReporterId = table.Column<string>(type: "text", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: false),
                    Details = table.Column<string>(type: "text", nullable: true),
                    ReportedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "ModuleContents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CourseModuleId = table.Column<int>(type: "integer", nullable: false),
                    ContentType = table.Column<string>(type: "text", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: true),
                    ContentText = table.Column<string>(type: "text", nullable: true),
                    MediaUrl = table.Column<string>(type: "text", nullable: true),
                    Duration = table.Column<string>(type: "text", nullable: true),
                    Order = table.Column<int>(type: "integer", nullable: false)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    CourseModuleId = table.Column<int>(type: "integer", nullable: false),
                    CompletedModules = table.Column<List<int>>(type: "integer[]", nullable: false),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Question = table.Column<string>(type: "text", nullable: false),
                    OptionA = table.Column<string>(type: "text", nullable: true),
                    OptionB = table.Column<string>(type: "text", nullable: true),
                    OptionC = table.Column<string>(type: "text", nullable: true),
                    OptionD = table.Column<string>(type: "text", nullable: true),
                    CorrectAnswer = table.Column<string>(type: "text", nullable: true),
                    ModuleContentId = table.Column<int>(type: "integer", nullable: false)
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
                columns: new[] { "Id", "Avatar", "Comment", "Comments", "Image", "Name", "Role" },
                values: new object[,]
                {
                    { 1, "/assets/Avatar/Sample1.ico", "Lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt lorem ipsum dolor sit amet.", 36, "/assets/Community Pics/Pottery.png", "Maria Santos", "Learner" },
                    { 2, "/assets/Avatar/Sample9.ico", "Lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt lorem ipsum dolor sit amet.", 18, "/assets/Community Pics/Weaving.png", "James dela Cruz", "Artisan" },
                    { 3, "/assets/Avatar/Sample5.ico", "Lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt lorem ipsum dolor sit amet, consectetur on adipiscing elit, eiusmod tempor incididunt lorem ipsum dolor sit amet.", 41, "/assets/Community Pics/Woodcarving.png", "Kim Navarro", "Researcher" }
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
                columns: new[] { "Id", "CreatedAt", "Email", "EmailVerificationToken", "FirstName", "IsArchived", "IsDeactivated", "IsVerified", "LastName", "OtpExpiry", "PasswordHash", "PasswordResetOtp", "Points", "Role", "SelectedInterests", "UserAvatar" },
                values: new object[,]
                {
                    { "A1111111", new DateTime(2024, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), "juan@example.com", null, "Juan", false, false, true, "Dela Cruz", null, "hashedpw", null, 0, "Learner", null, "/assets/Avatar/Sample10.svg" },
                    { "user-1", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "alice@test.com", null, "Alice", false, false, true, "Artisan", null, "dummyhash1", null, 10, "Learner", "Crafts, Sewing", "/assets/Avatar/Sample3.ico" },
                    { "user-2", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "bob@test.com", null, "Bob", false, false, false, "Builder", null, "dummyhash2", null, 5, "Learner", "Woodwork, DIY", "/assets/Avatar/Sample6.ico" },
                    { "user-3", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "charlie@test.com", null, "Charlie", false, false, true, "Craftsman", null, "dummyhash3", null, 20, "Learner", "Painting, Pottery", "/assets/Avatar/Sample9.ico" }
                });

            migrationBuilder.InsertData(
                table: "Artisans",
                columns: new[] { "ArtisanId", "ApplicationFile", "FirstName", "Hometown", "Introduction", "IsApproved", "IsArchived", "LastName", "Profession", "Role", "UserAvatar", "UserId" },
                values: new object[,]
                {
                    { "A1111111", null, "Alice", "Vigan, Ilocos Sur", "Alice is a skilled pottery artisan teaching hand-building techniques.", false, false, "Artisan", "Pottery Artisan", "Artisan", "/assets/Avatar/Sample3.ico", "user-1" },
                    { "A1111112", null, "Bob", "Cebu City", "Bob is an expert in woodcarving with 10 years of experience.", false, false, "Builder", "Woodcarving Artisan", "Artisan", "/assets/Avatar/Sample6.ico", "user-2" },
                    { "A1111113", null, "Charlie", "Davao City", "Charlie specializes in traditional and modern weaving techniques.", false, false, "Craftsman", "Weaving Artisan", "Artisan", "/assets/Avatar/Sample9.ico", "user-3" }
                });

            migrationBuilder.InsertData(
                table: "Communities",
                columns: new[] { "Id", "AvatarUrl", "CoverImageUrl", "CreatedAt", "CreatorId", "Description", "IsArchived", "IsPublished", "MembersCount", "Name" },
                values: new object[,]
                {
                    { 1, "/assets/Community Pics/CompletePottery.png", "/uploads/community-banner/PotteryBanner.png", new DateTime(2024, 12, 31, 16, 0, 0, 0, DateTimeKind.Utc), "user-1", "A community for learners and artisans passionate about pottery and ceramics.", false, true, 125, "Pottery Enthusiasts" },
                    { 2, "/assets/Community Pics/CompleteWeaving.png", "/uploads/community-banner/WeavingBanner.png", new DateTime(2024, 12, 31, 16, 0, 0, 0, DateTimeKind.Utc), "user-2", "Connecting artisans and learners who love weaving traditional and modern textiles.", false, true, 98, "Weaving Circle" },
                    { 3, "/assets/Community Pics/CompleteWoodcarving.png", "/uploads/community-banner/WoodCarvingBanner.png", new DateTime(2024, 12, 31, 16, 0, 0, 0, DateTimeKind.Utc), "user-3", "A space for woodcarvers to share projects, tips, and showcase craftsmanship.", false, true, 150, "Woodcarving Masters" }
                });

            migrationBuilder.InsertData(
                table: "CommunityPosts",
                columns: new[] { "Id", "AuthorId", "Category", "CommunityId", "Content", "ImageUrl", "IsPublished", "SubmittedAt", "Title" },
                values: new object[,]
                {
                    { 1, "user-1", "", 1, "Just finished my first clay pot! Excited to share with everyone.", "/assets/Community Pics/CompletePottery.png", true, new DateTime(2025, 1, 5, 0, 0, 0, 0, DateTimeKind.Utc), "First Pottery Creation" },
                    { 2, "user-3", "", 3, "Making progress on a wooden sculpture. Any tips for fine detailing?", "/assets/Community Pics/CompleteWoodCarving.png", true, new DateTime(2025, 1, 6, 0, 0, 0, 0, DateTimeKind.Utc), "Carving Progress" },
                    { 3, "user-2", "", 2, "Starting weaving for the first time. Happy to join this community!", "/assets/Community Pics/CompleteWeaving.png", true, new DateTime(2025, 1, 7, 0, 0, 0, 0, DateTimeKind.Utc), "New to Weaving" }
                });

            migrationBuilder.InsertData(
                table: "Courses",
                columns: new[] { "Id", "Category", "CreatedAt", "CreatedBy", "Difficulty", "Duration", "FinalProjectDescription", "FinalProjectTitle", "FullDescription", "ImageUrl", "IsArchived", "IsPublished", "Link", "Overview", "Requirements", "Thumbnail", "Title", "Video", "WhatToLearn" },
                values: new object[,]
                {
                    { 1, "Pottery", new DateTime(2024, 5, 31, 16, 0, 0, 0, DateTimeKind.Utc), "A1111111", "Beginner", "15 hours", "Design, shape, and glaze a personalized vase as your final pottery project.", "Create Your Own Clay Vase", "This course provides a step-by-step guide to both traditional and modern methods of pottery...", "/assets/Courses Pics/Pottery.png", false, true, "pottery", "Pottery is the art and craft of shaping and firing clay...", "Clay, a pottery wheel or hand-building tools, access to a kiln, apron, and sponges.", "/assets/Courses Pics/Pottery.png", "Pottery", "/assets/Videos/Pottery.mp4", "You'll learn pottery basics, hand-building, wheel throwing, and glazing techniques." },
                    { 2, "Wood Carving", new DateTime(2024, 5, 31, 16, 0, 0, 0, DateTimeKind.Utc), "A1111112", "Intermediate", "29 hours", "Apply your skills by carving a detailed animal figurine as your capstone project.", "Carve a Wooden Animal Figurine", "Explore the detailed world of woodcarving through this course...", "/assets/Courses Pics/Woodcarving.png", false, true, "woodcarving", "Woodcarving is the art of shaping and sculpting wood...", "Carving knives, gouges, mallet, sandpaper, safety gloves, and carving wood (basswood recommended).", "/assets/Courses Pics/Woodcarving.png", "Woodcarving", "/assets/Videos/Wood Carving.mp4", "You'll learn carving techniques like relief carving, whittling, chip carving, and finishing." },
                    { 3, "Weaving", new DateTime(2024, 5, 31, 16, 0, 0, 0, DateTimeKind.Utc), "A1111113", "Professional", "18 hours", "Plan and weave a wall hanging using advanced techniques to showcase your weaving skills.", "Design a Handwoven Wall Hanging", "This advanced course in weaving introduces students to both traditional and experimental textile design...", "/assets/Courses Pics/Weaving.png", false, true, "weaving", "Weaving is the craft of interlacing threads or fibers...", "Table or floor loom, warp and weft yarns, weaving comb, shuttles, and scissors.", "/assets/Courses Pics/Weaving.png", "Weaving", "/assets/Videos/Weaving.mp4", "You’ll explore techniques in tapestry weaving, loom setup, fiber selection, and pattern creation." }
                });

            migrationBuilder.InsertData(
                table: "ArtisanWorks",
                columns: new[] { "Id", "ArtisanId", "Caption", "CourseId", "ImageUrl", "PublishDate", "Title" },
                values: new object[,]
                {
                    { 1, "A1111111", "Handcrafted pottery made using traditional Vigan techniques.", 1, "/assets/Works/JuanWorks1.png", new DateTime(2024, 12, 31, 16, 0, 0, 0, DateTimeKind.Utc), "Classic Handcrafted Pottery" },
                    { 2, "A1111111", "A functional yet beautiful rustic clay pot for everyday use.", 2, "/assets/Works/JuanWorks2.png", new DateTime(2024, 12, 31, 16, 0, 0, 0, DateTimeKind.Utc), "Rustic Clay Pot" },
                    { 3, "A1111111", "An elegant pot showcasing intricate carvings and smooth finishing.", 3, "/assets/Works/JuanWorks3.png", new DateTime(2024, 12, 31, 16, 0, 0, 0, DateTimeKind.Utc), "Elegant Decorative Pot" }
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
                    { 6, 2, 0, "Introduction" },
                    { 7, 2, 0, "History" },
                    { 8, 2, 0, "Session" },
                    { 9, 2, 0, "Quiz" },
                    { 11, 3, 0, "Introduction" },
                    { 12, 3, 0, "History" },
                    { 13, 3, 0, "Session" },
                    { 14, 3, 0, "Quiz" }
                });

            migrationBuilder.InsertData(
                table: "CourseReviews",
                columns: new[] { "Id", "Comment", "CourseId", "CreatedAt", "Rating", "UserId" },
                values: new object[,]
                {
                    { 1, "An excellent course! The mentor was very knowledgeable.", 1, new DateTime(2024, 6, 4, 16, 0, 0, 0, DateTimeKind.Utc), 5, "A1111111" },
                    { 2, "Really enjoyed learning pottery. A few lessons were a bit fast though.", 1, new DateTime(2024, 6, 9, 16, 0, 0, 0, DateTimeKind.Utc), 4, "A1111111" },
                    { 3, "Very hands-on and engaging! Perfect for intermediate learners.", 3, new DateTime(2024, 6, 14, 16, 0, 0, 0, DateTimeKind.Utc), 5, "A1111111" }
                });

            migrationBuilder.InsertData(
                table: "ModuleContents",
                columns: new[] { "Id", "ContentText", "ContentType", "CourseModuleId", "Duration", "MediaUrl", "Order", "Title" },
                values: new object[,]
                {
                    { 1, "Pottery intro content something anything.", "Video", 1, null, "/assets/Videos/Pottery.mp4", 0, "Welcome to Pottery" },
                    { 2, "History content something anything.", "Image + Text", 2, null, "/assets/Courses Pics/Pottery2.png", 0, "History of Pottery" },
                    { 3, null, "Session", 3, null, null, 0, "Live Pottery Session" },
                    { 5, "Woodcarving intro content something anything.", "Video", 6, null, "/assets/Videos/Wood Carving.mp4", 0, "Welcome to Woodcarving" },
                    { 6, "History content something anything.", "Image + Text", 7, null, "/assets/Courses Pics/Woodcarving2.png", 0, "History of Woodcarving" },
                    { 7, null, "Session", 8, null, null, 0, "Live Woodcarving Session" },
                    { 9, "Weaving intro content something anything.", "Video", 11, null, "/assets/Videos/Weaving.mp4", 0, "Welcome to Weaving" },
                    { 10, "History content something anything.", "Image + Text", 12, null, "/assets/Courses Pics/Weaving2.png", 0, "History of Weaving" },
                    { 11, null, "Session", 13, null, null, 0, "Live Weaving Session" },
                    { 13, null, "Quiz", 4, null, null, 0, "Pottery Quiz" },
                    { 14, null, "Quiz", 9, null, null, 0, "Woodcarving Quiz" },
                    { 15, null, "Quiz", 14, null, null, 0, "Weaving Quiz" },
                    { 16, "Learn about clay, tools, and equipment.", "Image + Text", 2, null, "/assets/Courses Pics/Pottery.png", 0, "Pottery Materials" },
                    { 17, "Step by step techniques for beginners.", "Image + Text", 2, null, "/assets/Courses Pics/Pottery1.png", 0, "Basic Pottery Techniques" },
                    { 18, "Learn about wood types suitable for carving.", "Image + Text", 7, null, "/assets/Courses Pics/Woodcarving.png", 0, "Wood Types" },
                    { 19, "Introduction to essential carving tools.", "Image + Text", 7, null, "/assets/Courses Pics/Woodcarving1.png", 0, "Basic Carving Tools" },
                    { 20, "Learn about threads, yarns, and tools.", "Image + Text", 12, null, "/assets/Courses Pics/Weaving.png", 0, "Weaving Materials" },
                    { 21, "Step by step weaving techniques.", "Image + Text", 12, null, "/assets/Courses Pics/Weaving1.png", 0, "Basic Weaving Techniques" }
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
                name: "IX_ArtisanWorks_CourseId",
                table: "ArtisanWorks",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Communities_CreatorId",
                table: "Communities",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityJoinRequests_CommunityId",
                table: "CommunityJoinRequests",
                column: "CommunityId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityJoinRequests_UserId",
                table: "CommunityJoinRequests",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityMemberships_CommunityId",
                table: "CommunityMemberships",
                column: "CommunityId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityMemberships_UserId",
                table: "CommunityMemberships",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityPostReports_PostId",
                table: "CommunityPostReports",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityPosts_AuthorId",
                table: "CommunityPosts",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityPosts_CommunityId",
                table: "CommunityPosts",
                column: "CommunityId");

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
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");

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
                name: "CommunityJoinRequests");

            migrationBuilder.DropTable(
                name: "CommunityMemberships");

            migrationBuilder.DropTable(
                name: "CommunityPostReports");

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
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "QuizQuestions");

            migrationBuilder.DropTable(
                name: "SupportSessionRequests");

            migrationBuilder.DropTable(
                name: "UserInterests");

            migrationBuilder.DropTable(
                name: "CommunityPosts");

            migrationBuilder.DropTable(
                name: "ModuleContents");

            migrationBuilder.DropTable(
                name: "Communities");

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
