using System;
using System.Collections.Generic;
using CloudSync.Modules.EmployeeManagement.Models;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CloudSync.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Department",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    ParentDepartmentId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Department", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Department_Department_ParentDepartmentId",
                        column: x => x.ParentDepartmentId,
                        principalTable: "Department",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Permission",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permission", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProjectTeam",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectTeam", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Employee",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    LastName = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    ProjxonEmail = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    Phone = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true),
                    Address = table.Column<string>(type: "json", nullable: true),
                    PersonalEmail = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    EmergencyContactName = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    EmergencyContactPhone = table.Column<string>(type: "text", nullable: true),
                    SocialSecurityNumber = table.Column<string>(type: "character varying(9)", maxLength: 9, nullable: true),
                    IdNumber = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    IdType = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    IdCountry = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    IdState = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    BirthCountry = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    BirthState = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    BirthCity = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    BirthDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Gender = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    PreferredPronouns = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    PreferredName = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    JobTitle = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    DepartmentId = table.Column<int>(type: "integer", nullable: false),
                    ManagerId = table.Column<int>(type: "integer", nullable: false),
                    CoachId = table.Column<int>(type: "integer", nullable: false),
                    HierarchyLevel = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    OnboardingDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    OffboardingDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CreateDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProfilePictureUrl = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ResumeUrl = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CoverLetterUrl = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    LinkedInUrl = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    GitHubUrl = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    PersonalWebsiteUrl = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    WorkAuthorizationType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    WorkAuthorizationDocumentUrl = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    VisaNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    VisaExpirationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    WorkPermitNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    WorkExpirationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EducationLevel = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    UniversitiesAttended = table.Column<List<string>>(type: "jsonb", nullable: true),
                    DegreesEarned = table.Column<List<string>>(type: "jsonb", nullable: true),
                    TimeZone = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    City = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    State = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    RecruitingSource = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CanvasCoursesCompleted = table.Column<List<string>>(type: "jsonb", nullable: true),
                    CanvasCertificates = table.Column<List<string>>(type: "jsonb", nullable: true),
                    NewCompany = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    OnboardingChecklist = table.Column<List<string>>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employee", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employee_Department_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Department",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Employee_Employee_CoachId",
                        column: x => x.CoachId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Employee_Employee_ManagerId",
                        column: x => x.ManagerId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Candidate",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    LastName = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    Email = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    Phone = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true),
                    JobAppliedFor = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    AvailabilityDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    OnboardingDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CreateDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ResumeUrl = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CoverLetterUrl = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    LinkedInUrl = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    GitHubUrl = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    PersonalWebsiteUrl = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    WorkAuthorizationType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    EducationLevel = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    UniversitiesAttended = table.Column<List<string>>(type: "jsonb", nullable: true),
                    TimeZone = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    City = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    State = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    RecruitingSource = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    InterviewerId = table.Column<int>(type: "integer", nullable: true),
                    Interviewers = table.Column<List<Employee>>(type: "jsonb", nullable: true),
                    InterviewDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Notes = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candidate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Candidate_Employee_InterviewerId",
                        column: x => x.InterviewerId,
                        principalTable: "Employee",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TeamMember",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmployeeId = table.Column<int>(type: "integer", nullable: false),
                    ProjectTeamId = table.Column<int>(type: "integer", nullable: false),
                    RoleInTeam = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamMember", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeamMember_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeamMember_ProjectTeam_ProjectTeamId",
                        column: x => x.ProjectTeamId,
                        principalTable: "ProjectTeam",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    Password = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    RoleId = table.Column<int>(type: "integer", nullable: false),
                    EmployeeId = table.Column<int>(type: "integer", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastLoginDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserSettings = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Users_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Candidate_InterviewerId",
                table: "Candidate",
                column: "InterviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_Department_ParentDepartmentId",
                table: "Department",
                column: "ParentDepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_CoachId",
                table: "Employee",
                column: "CoachId");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_DepartmentId",
                table: "Employee",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_ManagerId",
                table: "Employee",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMember_EmployeeId",
                table: "TeamMember",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMember_ProjectTeamId",
                table: "TeamMember",
                column: "ProjectTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_EmployeeId",
                table: "Users",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Candidate");

            migrationBuilder.DropTable(
                name: "Permission");

            migrationBuilder.DropTable(
                name: "TeamMember");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "ProjectTeam");

            migrationBuilder.DropTable(
                name: "Employee");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Department");
        }
    }
}
