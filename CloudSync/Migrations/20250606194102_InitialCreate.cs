using System;
using System.Collections.Generic;
using CloudSync.Modules.EmployeeManagement.Models;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CloudSync.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "addresses",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    address_line1 = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    address_line2 = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    country = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    state_or_province = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    city = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    postal_code = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    time_zone = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_addresses", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "departments",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    parent_department_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_departments", x => x.id);
                    table.ForeignKey(
                        name: "fk_departments_departments_parent_department_id",
                        column: x => x.parent_department_id,
                        principalTable: "departments",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "permissions",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_permissions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "project_teams",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_project_teams", x => x.id);
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
                name: "employees",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    basic_info_id = table.Column<int>(type: "integer", nullable: false),
                    basic_info_first_name = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    basic_info_last_name = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    basic_info_nationality = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    basic_info_ethnicity = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    basic_info_birth_country = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    basic_info_birth_state = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    basic_info_birth_city = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    basic_info_birth_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    basic_info_gender = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    basic_info_marital_status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    basic_info_preferred_pronouns = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    basic_info_preferred_name = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    basic_info_nick_name = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    contact_info_id = table.Column<int>(type: "integer", nullable: false),
                    contact_info_phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    contact_info_international_phone = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    contact_info_international_phone_type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    contact_info_address_id = table.Column<int>(type: "integer", nullable: true),
                    contact_info_projxon_email = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    contact_info_personal_email = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    contact_info_emergency_contact_name = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    contact_info_emergency_contact_phone = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true),
                    employee_details_id = table.Column<int>(type: "integer", nullable: true),
                    employee_documents_id = table.Column<int>(type: "integer", nullable: true),
                    employee_legal_id = table.Column<int>(type: "integer", nullable: true),
                    employee_education_id = table.Column<int>(type: "integer", nullable: true),
                    employee_training_id = table.Column<int>(type: "integer", nullable: true),
                    create_date_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    update_date_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_employees", x => x.id);
                    table.ForeignKey(
                        name: "fk_employees_addresses_contact_info_address_id",
                        column: x => x.contact_info_address_id,
                        principalTable: "addresses",
                        principalColumn: "id");
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
                        name: "fk_candidate_employees_interviewer_id",
                        column: x => x.interviewer_id,
                        principalTable: "employees",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "employee_documents",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    profile_picture_url = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    resume_url = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    cover_letter_url = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    linked_in_url = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    git_hub_url = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    personal_website_url = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_employee_documents", x => x.id);
                    table.ForeignKey(
                        name: "fk_employee_documents_employees_id",
                        column: x => x.id,
                        principalTable: "employees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "employee_educations",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    education_level = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    universities_attended = table.Column<List<string>>(type: "jsonb", nullable: true),
                    degrees_earned = table.Column<List<string>>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_employee_educations", x => x.id);
                    table.ForeignKey(
                        name: "fk_employee_educations_employees_id",
                        column: x => x.id,
                        principalTable: "employees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "employee_legals",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    id_number = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    id_type = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    id_country = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    id_state = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    work_authorization_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    work_authorization_document_url = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    visa_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    visa_expiration_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    work_permit_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    work_expiration_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_employee_legals", x => x.id);
                    table.ForeignKey(
                        name: "fk_employee_legals_employees_id",
                        column: x => x.id,
                        principalTable: "employees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "employee_positions",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    position_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    department_id = table.Column<int>(type: "integer", nullable: true),
                    sub_department_id = table.Column<int>(type: "integer", nullable: true),
                    hierarchy_level = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    position_id = table.Column<int>(type: "integer", nullable: true),
                    manager_id = table.Column<int>(type: "integer", nullable: true),
                    coach_id = table.Column<int>(type: "integer", nullable: true),
                    onboarding_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    offboarding_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    employment_status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    employment_type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    recruiting_source = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_employee_positions", x => x.id);
                    table.ForeignKey(
                        name: "fk_employee_positions_departments_department_id",
                        column: x => x.department_id,
                        principalTable: "departments",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_employee_positions_departments_sub_department_id",
                        column: x => x.sub_department_id,
                        principalTable: "departments",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_employee_positions_employee_positions_position_id",
                        column: x => x.position_id,
                        principalTable: "employee_positions",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_employee_positions_employees_coach_id",
                        column: x => x.coach_id,
                        principalTable: "employees",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_employee_positions_employees_id",
                        column: x => x.id,
                        principalTable: "employees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_employee_positions_employees_manager_id",
                        column: x => x.manager_id,
                        principalTable: "employees",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "employee_trainings",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    canvas_courses_completed = table.Column<List<string>>(type: "jsonb", nullable: true),
                    canvas_certificates = table.Column<List<string>>(type: "jsonb", nullable: true),
                    new_company = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    onboarding_checklist = table.Column<List<string>>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_employee_trainings", x => x.id);
                    table.ForeignKey(
                        name: "fk_employee_trainings_employees_id",
                        column: x => x.id,
                        principalTable: "employees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "team_members",
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
                    table.PrimaryKey("pk_team_members", x => x.id);
                    table.ForeignKey(
                        name: "fk_team_members_employees_employee_id",
                        column: x => x.employee_id,
                        principalTable: "employees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_team_members_project_teams_project_team_id",
                        column: x => x.project_team_id,
                        principalTable: "project_teams",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    google_user_id = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    email = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    role_id = table.Column<int>(type: "integer", nullable: false),
                    employee_id = table.Column<int>(type: "integer", nullable: false),
                    create_date_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_login_date_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    user_settings = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                    table.ForeignKey(
                        name: "fk_users_employees_employee_id",
                        column: x => x.employee_id,
                        principalTable: "employees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_users_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
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

            migrationBuilder.InsertData(
                table: "departments",
                columns: new[] { "id", "name", "parent_department_id" },
                values: new object[,]
                {
                    { 1, "Business", null },
                    { 2, "Human Resources", null },
                    { 3, "Information Technology", null },
                    { 4, "Marketing", null },
                    { 5, "Operations", null }
                });

            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "id", "description", "name" },
                values: new object[,]
                {
                    { 1, null, "Administrator" },
                    { 2, null, "Human Resources" },
                    { 3, null, "Employee" },
                    { 4, null, "Guest" }
                });

            migrationBuilder.InsertData(
                table: "departments",
                columns: new[] { "id", "name", "parent_department_id" },
                values: new object[,]
                {
                    { 6, "Executive", 1 },
                    { 7, "Consulting", 1 },
                    { 8, "Onboarding", 2 },
                    { 9, "Offboarding", 2 },
                    { 10, "Recruiting", 2 },
                    { 11, "AI", 3 },
                    { 12, "Applications Development", 3 },
                    { 13, "Cybersecurity", 3 },
                    { 14, "SEO", 3 },
                    { 15, "Web Development", 3 },
                    { 16, "Market Research", 4 },
                    { 17, "Social Media", 4 },
                    { 18, "Copywriting", 4 },
                    { 19, "Graphic Design", 4 },
                    { 20, "Finance", 5 },
                    { 21, "Legal", 5 }
                });

            migrationBuilder.CreateIndex(
                name: "ix_candidate_interviewer_id",
                table: "candidate",
                column: "interviewer_id");

            migrationBuilder.CreateIndex(
                name: "ix_departments_parent_department_id",
                table: "departments",
                column: "parent_department_id");

            migrationBuilder.CreateIndex(
                name: "ix_employee_positions_coach_id",
                table: "employee_positions",
                column: "coach_id");

            migrationBuilder.CreateIndex(
                name: "ix_employee_positions_department_id",
                table: "employee_positions",
                column: "department_id");

            migrationBuilder.CreateIndex(
                name: "ix_employee_positions_manager_id",
                table: "employee_positions",
                column: "manager_id");

            migrationBuilder.CreateIndex(
                name: "ix_employee_positions_position_id",
                table: "employee_positions",
                column: "position_id");

            migrationBuilder.CreateIndex(
                name: "ix_employee_positions_sub_department_id",
                table: "employee_positions",
                column: "sub_department_id");

            migrationBuilder.CreateIndex(
                name: "ix_employees_contact_info_address_id",
                table: "employees",
                column: "contact_info_address_id");

            migrationBuilder.CreateIndex(
                name: "ix_invited_users_invited_by_employee_id",
                table: "invited_users",
                column: "invited_by_employee_id");

            migrationBuilder.CreateIndex(
                name: "ix_team_members_employee_id",
                table: "team_members",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "ix_team_members_project_team_id",
                table: "team_members",
                column: "project_team_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_employee_id",
                table: "users",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_role_id",
                table: "users",
                column: "role_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "candidate");

            migrationBuilder.DropTable(
                name: "employee_documents");

            migrationBuilder.DropTable(
                name: "employee_educations");

            migrationBuilder.DropTable(
                name: "employee_legals");

            migrationBuilder.DropTable(
                name: "employee_positions");

            migrationBuilder.DropTable(
                name: "employee_trainings");

            migrationBuilder.DropTable(
                name: "invited_users");

            migrationBuilder.DropTable(
                name: "permissions");

            migrationBuilder.DropTable(
                name: "team_members");

            migrationBuilder.DropTable(
                name: "departments");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "project_teams");

            migrationBuilder.DropTable(
                name: "employees");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "addresses");
        }
    }
}
