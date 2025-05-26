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
                name: "department",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    parent_department_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_department", x => x.id);
                    table.ForeignKey(
                        name: "fk_department_department_parent_department_id",
                        column: x => x.parent_department_id,
                        principalTable: "department",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "permission",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_permission", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "project_team",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_project_team", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    google_user_id = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    email = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    create_date_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_login_date_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    user_settings = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "employee",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    first_name = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    last_name = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    projxon_email = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    phone = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true),
                    address = table.Column<string>(type: "json", nullable: true),
                    personal_email = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    emergency_contact_name = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    emergency_contact_phone = table.Column<string>(type: "text", nullable: true),
                    social_security_number = table.Column<string>(type: "character varying(9)", maxLength: 9, nullable: true),
                    id_number = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    id_type = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    id_country = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    id_state = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    birth_country = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    birth_state = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    birth_city = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    birth_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    gender = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    preferred_pronouns = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    preferred_name = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    job_title = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    department_id = table.Column<int>(type: "integer", nullable: false),
                    manager_id = table.Column<int>(type: "integer", nullable: false),
                    coach_id = table.Column<int>(type: "integer", nullable: false),
                    hierarchy_level = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    onboarding_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    offboarding_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    create_date_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    update_date_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    profile_picture_url = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    resume_url = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    cover_letter_url = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    linked_in_url = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    git_hub_url = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    personal_website_url = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    work_authorization_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    work_authorization_document_url = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    visa_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    visa_expiration_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    work_permit_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    work_expiration_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    education_level = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    universities_attended = table.Column<List<string>>(type: "jsonb", nullable: true),
                    degrees_earned = table.Column<List<string>>(type: "jsonb", nullable: true),
                    time_zone = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    city = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    state = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    recruiting_source = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    canvas_courses_completed = table.Column<List<string>>(type: "jsonb", nullable: true),
                    canvas_certificates = table.Column<List<string>>(type: "jsonb", nullable: true),
                    new_company = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    onboarding_checklist = table.Column<List<string>>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_employee", x => x.id);
                    table.ForeignKey(
                        name: "fk_employee_department_department_id",
                        column: x => x.department_id,
                        principalTable: "department",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_employee_employee_coach_id",
                        column: x => x.coach_id,
                        principalTable: "employee",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_employee_employee_manager_id",
                        column: x => x.manager_id,
                        principalTable: "employee",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "invited_users",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    invited_by_employee_id = table.Column<int>(type: "integer", maxLength: 255, nullable: false),
                    create_date_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_invited_users", x => x.id);
                    table.ForeignKey(
                        name: "fk_invited_users_users_invited_by_employee_id",
                        column: x => x.invited_by_employee_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "candidate",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    first_name = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    last_name = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    email = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    phone = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true),
                    job_applied_for = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    availability_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    onboarding_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    create_date_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    update_date_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    resume_url = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    cover_letter_url = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    linked_in_url = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    git_hub_url = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    personal_website_url = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    work_authorization_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    education_level = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    universities_attended = table.Column<List<string>>(type: "jsonb", nullable: true),
                    time_zone = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    city = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    state = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    recruiting_source = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    interviewer_id = table.Column<int>(type: "integer", nullable: true),
                    interviewers = table.Column<List<Employee>>(type: "jsonb", nullable: true),
                    interview_date_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    notes = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_candidate", x => x.id);
                    table.ForeignKey(
                        name: "fk_candidate_employee_interviewer_id",
                        column: x => x.interviewer_id,
                        principalTable: "employee",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "team_member",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    employee_id = table.Column<int>(type: "integer", nullable: false),
                    project_team_id = table.Column<int>(type: "integer", nullable: false),
                    role_in_team = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_team_member", x => x.id);
                    table.ForeignKey(
                        name: "fk_team_member_employee_employee_id",
                        column: x => x.employee_id,
                        principalTable: "employee",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_team_member_project_team_project_team_id",
                        column: x => x.project_team_id,
                        principalTable: "project_team",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_candidate_interviewer_id",
                table: "candidate",
                column: "interviewer_id");

            migrationBuilder.CreateIndex(
                name: "ix_department_parent_department_id",
                table: "department",
                column: "parent_department_id");

            migrationBuilder.CreateIndex(
                name: "ix_employee_coach_id",
                table: "employee",
                column: "coach_id");

            migrationBuilder.CreateIndex(
                name: "ix_employee_department_id",
                table: "employee",
                column: "department_id");

            migrationBuilder.CreateIndex(
                name: "ix_employee_manager_id",
                table: "employee",
                column: "manager_id");

            migrationBuilder.CreateIndex(
                name: "ix_invited_users_invited_by_employee_id",
                table: "invited_users",
                column: "invited_by_employee_id");

            migrationBuilder.CreateIndex(
                name: "ix_team_member_employee_id",
                table: "team_member",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "ix_team_member_project_team_id",
                table: "team_member",
                column: "project_team_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "candidate");

            migrationBuilder.DropTable(
                name: "invited_users");

            migrationBuilder.DropTable(
                name: "permission");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "team_member");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "employee");

            migrationBuilder.DropTable(
                name: "project_team");

            migrationBuilder.DropTable(
                name: "department");
        }
    }
}
