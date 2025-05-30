﻿// <auto-generated />
using System;
using System.Collections.Generic;
using CloudSync.Infrastructure;
using CloudSync.Modules.EmployeeManagement.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CloudSync.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    partial class DatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("CloudSync.Modules.CandidateManagement.Models.Candidate", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime?>("AvailabilityDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("availability_date");

                    b.Property<string>("City")
                        .HasMaxLength(30)
                        .HasColumnType("character varying(30)")
                        .HasColumnName("city");

                    b.Property<string>("CoverLetterUrl")
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)")
                        .HasColumnName("cover_letter_url");

                    b.Property<DateTime>("CreateDateTime")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("create_date_time");

                    b.Property<string>("EducationLevel")
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)")
                        .HasColumnName("education_level");

                    b.Property<string>("Email")
                        .HasMaxLength(40)
                        .HasColumnType("character varying(40)")
                        .HasColumnName("email");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("character varying(40)")
                        .HasColumnName("first_name");

                    b.Property<string>("GitHubUrl")
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)")
                        .HasColumnName("git_hub_url");

                    b.Property<DateTime?>("InterviewDateTime")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("interview_date_time");

                    b.Property<int?>("InterviewerId")
                        .HasColumnType("integer")
                        .HasColumnName("interviewer_id");

                    b.Property<List<Employee>>("Interviewers")
                        .HasColumnType("jsonb")
                        .HasColumnName("interviewers");

                    b.Property<string>("JobAppliedFor")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("character varying(40)")
                        .HasColumnName("job_applied_for");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("character varying(40)")
                        .HasColumnName("last_name");

                    b.Property<string>("LinkedInUrl")
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)")
                        .HasColumnName("linked_in_url");

                    b.Property<string>("Notes")
                        .HasMaxLength(400)
                        .HasColumnType("character varying(400)")
                        .HasColumnName("notes");

                    b.Property<DateTime?>("OnboardingDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("onboarding_date");

                    b.Property<string>("PersonalWebsiteUrl")
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)")
                        .HasColumnName("personal_website_url");

                    b.Property<string>("Phone")
                        .HasMaxLength(15)
                        .HasColumnType("character varying(15)")
                        .HasColumnName("phone");

                    b.Property<string>("RecruitingSource")
                        .HasMaxLength(30)
                        .HasColumnType("character varying(30)")
                        .HasColumnName("recruiting_source");

                    b.Property<string>("ResumeUrl")
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)")
                        .HasColumnName("resume_url");

                    b.Property<string>("State")
                        .HasMaxLength(30)
                        .HasColumnType("character varying(30)")
                        .HasColumnName("state");

                    b.Property<string>("Status")
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)")
                        .HasColumnName("status");

                    b.Property<string>("TimeZone")
                        .HasMaxLength(30)
                        .HasColumnType("character varying(30)")
                        .HasColumnName("time_zone");

                    b.Property<List<string>>("UniversitiesAttended")
                        .HasColumnType("jsonb")
                        .HasColumnName("universities_attended");

                    b.Property<DateTime>("UpdateDateTime")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("update_date_time");

                    b.Property<string>("WorkAuthorizationType")
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)")
                        .HasColumnName("work_authorization_type");

                    b.HasKey("Id")
                        .HasName("pk_candidate");

                    b.HasIndex("InterviewerId")
                        .HasDatabaseName("ix_candidate_interviewer_id");

                    b.ToTable("candidate", (string)null);
                });

            modelBuilder.Entity("CloudSync.Modules.EmployeeManagement.Models.Department", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)")
                        .HasColumnName("name");

                    b.Property<int>("ParentDepartmentId")
                        .HasColumnType("integer")
                        .HasColumnName("parent_department_id");

                    b.HasKey("Id")
                        .HasName("pk_department");

                    b.HasIndex("ParentDepartmentId")
                        .HasDatabaseName("ix_department_parent_department_id");

                    b.ToTable("department", (string)null);
                });

            modelBuilder.Entity("CloudSync.Modules.EmployeeManagement.Models.Employee", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Address")
                        .HasColumnType("json")
                        .HasColumnName("address");

                    b.Property<string>("BirthCity")
                        .HasMaxLength(40)
                        .HasColumnType("character varying(40)")
                        .HasColumnName("birth_city");

                    b.Property<string>("BirthCountry")
                        .HasMaxLength(40)
                        .HasColumnType("character varying(40)")
                        .HasColumnName("birth_country");

                    b.Property<DateTime?>("BirthDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("birth_date");

                    b.Property<string>("BirthState")
                        .HasMaxLength(40)
                        .HasColumnType("character varying(40)")
                        .HasColumnName("birth_state");

                    b.Property<List<string>>("CanvasCertificates")
                        .HasColumnType("jsonb")
                        .HasColumnName("canvas_certificates");

                    b.Property<List<string>>("CanvasCoursesCompleted")
                        .HasColumnType("jsonb")
                        .HasColumnName("canvas_courses_completed");

                    b.Property<string>("City")
                        .HasMaxLength(30)
                        .HasColumnType("character varying(30)")
                        .HasColumnName("city");

                    b.Property<int>("CoachId")
                        .HasColumnType("integer")
                        .HasColumnName("coach_id");

                    b.Property<string>("CoverLetterUrl")
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)")
                        .HasColumnName("cover_letter_url");

                    b.Property<DateTime>("CreateDateTime")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("create_date_time");

                    b.Property<List<string>>("DegreesEarned")
                        .HasColumnType("jsonb")
                        .HasColumnName("degrees_earned");

                    b.Property<int>("DepartmentId")
                        .HasColumnType("integer")
                        .HasColumnName("department_id");

                    b.Property<string>("EducationLevel")
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)")
                        .HasColumnName("education_level");

                    b.Property<string>("EmergencyContactName")
                        .HasMaxLength(40)
                        .HasColumnType("character varying(40)")
                        .HasColumnName("emergency_contact_name");

                    b.Property<string>("EmergencyContactPhone")
                        .HasColumnType("text")
                        .HasColumnName("emergency_contact_phone");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("character varying(40)")
                        .HasColumnName("first_name");

                    b.Property<string>("Gender")
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)")
                        .HasColumnName("gender");

                    b.Property<string>("GitHubUrl")
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)")
                        .HasColumnName("git_hub_url");

                    b.Property<string>("HierarchyLevel")
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)")
                        .HasColumnName("hierarchy_level");

                    b.Property<string>("IdCountry")
                        .HasMaxLength(40)
                        .HasColumnType("character varying(40)")
                        .HasColumnName("id_country");

                    b.Property<string>("IdNumber")
                        .HasMaxLength(40)
                        .HasColumnType("character varying(40)")
                        .HasColumnName("id_number");

                    b.Property<string>("IdState")
                        .HasMaxLength(40)
                        .HasColumnType("character varying(40)")
                        .HasColumnName("id_state");

                    b.Property<string>("IdType")
                        .HasMaxLength(40)
                        .HasColumnType("character varying(40)")
                        .HasColumnName("id_type");

                    b.Property<string>("JobTitle")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("character varying(40)")
                        .HasColumnName("job_title");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("character varying(40)")
                        .HasColumnName("last_name");

                    b.Property<string>("LinkedInUrl")
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)")
                        .HasColumnName("linked_in_url");

                    b.Property<int>("ManagerId")
                        .HasColumnType("integer")
                        .HasColumnName("manager_id");

                    b.Property<string>("NewCompany")
                        .HasMaxLength(40)
                        .HasColumnType("character varying(40)")
                        .HasColumnName("new_company");

                    b.Property<DateTime?>("OffboardingDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("offboarding_date");

                    b.Property<List<string>>("OnboardingChecklist")
                        .HasColumnType("jsonb")
                        .HasColumnName("onboarding_checklist");

                    b.Property<DateTime?>("OnboardingDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("onboarding_date");

                    b.Property<string>("PersonalEmail")
                        .HasMaxLength(60)
                        .HasColumnType("character varying(60)")
                        .HasColumnName("personal_email");

                    b.Property<string>("PersonalWebsiteUrl")
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)")
                        .HasColumnName("personal_website_url");

                    b.Property<string>("Phone")
                        .HasMaxLength(15)
                        .HasColumnType("character varying(15)")
                        .HasColumnName("phone");

                    b.Property<string>("PreferredName")
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)")
                        .HasColumnName("preferred_name");

                    b.Property<string>("PreferredPronouns")
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)")
                        .HasColumnName("preferred_pronouns");

                    b.Property<string>("ProfilePictureUrl")
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)")
                        .HasColumnName("profile_picture_url");

                    b.Property<string>("ProjxonEmail")
                        .HasMaxLength(60)
                        .HasColumnType("character varying(60)")
                        .HasColumnName("projxon_email");

                    b.Property<string>("RecruitingSource")
                        .HasMaxLength(30)
                        .HasColumnType("character varying(30)")
                        .HasColumnName("recruiting_source");

                    b.Property<string>("ResumeUrl")
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)")
                        .HasColumnName("resume_url");

                    b.Property<string>("SocialSecurityNumber")
                        .HasMaxLength(9)
                        .HasColumnType("character varying(9)")
                        .HasColumnName("social_security_number");

                    b.Property<string>("State")
                        .HasMaxLength(30)
                        .HasColumnType("character varying(30)")
                        .HasColumnName("state");

                    b.Property<string>("Status")
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)")
                        .HasColumnName("status");

                    b.Property<string>("TimeZone")
                        .HasMaxLength(30)
                        .HasColumnType("character varying(30)")
                        .HasColumnName("time_zone");

                    b.Property<List<string>>("UniversitiesAttended")
                        .HasColumnType("jsonb")
                        .HasColumnName("universities_attended");

                    b.Property<DateTime>("UpdateDateTime")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("update_date_time");

                    b.Property<DateTime>("VisaExpirationDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("visa_expiration_date");

                    b.Property<string>("VisaNumber")
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)")
                        .HasColumnName("visa_number");

                    b.Property<string>("WorkAuthorizationDocumentUrl")
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)")
                        .HasColumnName("work_authorization_document_url");

                    b.Property<string>("WorkAuthorizationType")
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)")
                        .HasColumnName("work_authorization_type");

                    b.Property<DateTime>("WorkExpirationDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("work_expiration_date");

                    b.Property<string>("WorkPermitNumber")
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)")
                        .HasColumnName("work_permit_number");

                    b.HasKey("Id")
                        .HasName("pk_employee");

                    b.HasIndex("CoachId")
                        .HasDatabaseName("ix_employee_coach_id");

                    b.HasIndex("DepartmentId")
                        .HasDatabaseName("ix_employee_department_id");

                    b.HasIndex("ManagerId")
                        .HasDatabaseName("ix_employee_manager_id");

                    b.ToTable("employee", (string)null);
                });

            modelBuilder.Entity("CloudSync.Modules.EmployeeManagement.Models.ProjectTeam", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)")
                        .HasColumnName("description");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("character varying(30)")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("pk_project_team");

                    b.ToTable("project_team", (string)null);
                });

            modelBuilder.Entity("CloudSync.Modules.EmployeeManagement.Models.TeamMember", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("EmployeeId")
                        .HasColumnType("integer")
                        .HasColumnName("employee_id");

                    b.Property<int>("ProjectTeamId")
                        .HasColumnType("integer")
                        .HasColumnName("project_team_id");

                    b.Property<string>("RoleInTeam")
                        .HasMaxLength(30)
                        .HasColumnType("character varying(30)")
                        .HasColumnName("role_in_team");

                    b.HasKey("Id")
                        .HasName("pk_team_member");

                    b.HasIndex("EmployeeId")
                        .HasDatabaseName("ix_team_member_employee_id");

                    b.HasIndex("ProjectTeamId")
                        .HasDatabaseName("ix_team_member_project_team_id");

                    b.ToTable("team_member", (string)null);
                });

            modelBuilder.Entity("CloudSync.Modules.UserManagement.Models.InvitedUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreateDateTime")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("create_date_time");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("email");

                    b.Property<int>("InvitedByEmployeeId")
                        .HasMaxLength(255)
                        .HasColumnType("integer")
                        .HasColumnName("invited_by_employee_id");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)")
                        .HasColumnName("status");

                    b.HasKey("Id")
                        .HasName("pk_invited_users");

                    b.HasIndex("InvitedByEmployeeId")
                        .HasDatabaseName("ix_invited_users_invited_by_employee_id");

                    b.ToTable("invited_users", (string)null);
                });

            modelBuilder.Entity("CloudSync.Modules.UserManagement.Models.Permission", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)")
                        .HasColumnName("description");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("character varying(40)")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("pk_permission");

                    b.ToTable("permission", (string)null);
                });

            modelBuilder.Entity("CloudSync.Modules.UserManagement.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreateDateTime")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("create_date_time");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("character varying(40)")
                        .HasColumnName("email");

                    b.Property<string>("GoogleUserId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("google_user_id");

                    b.Property<DateTime>("LastLoginDateTime")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("last_login_date_time");

                    b.Property<string>("UserSettings")
                        .HasColumnType("jsonb")
                        .HasColumnName("user_settings");

                    b.HasKey("Id")
                        .HasName("pk_users");

                    b.ToTable("users", (string)null);
                });

            modelBuilder.Entity("CloudSync.Modules.UserManagement.Models.UserRole", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)")
                        .HasColumnName("description");

                    b.Property<string>("Name")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("pk_roles");

                    b.ToTable("roles", (string)null);
                });

            modelBuilder.Entity("CloudSync.Modules.CandidateManagement.Models.Candidate", b =>
                {
                    b.HasOne("CloudSync.Modules.EmployeeManagement.Models.Employee", "Interviewer")
                        .WithMany()
                        .HasForeignKey("InterviewerId")
                        .HasConstraintName("fk_candidate_employee_interviewer_id");

                    b.Navigation("Interviewer");
                });

            modelBuilder.Entity("CloudSync.Modules.EmployeeManagement.Models.Department", b =>
                {
                    b.HasOne("CloudSync.Modules.EmployeeManagement.Models.Department", "ParentDepartment")
                        .WithMany()
                        .HasForeignKey("ParentDepartmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_department_department_parent_department_id");

                    b.Navigation("ParentDepartment");
                });

            modelBuilder.Entity("CloudSync.Modules.EmployeeManagement.Models.Employee", b =>
                {
                    b.HasOne("CloudSync.Modules.EmployeeManagement.Models.Employee", "Coach")
                        .WithMany()
                        .HasForeignKey("CoachId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_employee_employee_coach_id");

                    b.HasOne("CloudSync.Modules.EmployeeManagement.Models.Department", "Department")
                        .WithMany()
                        .HasForeignKey("DepartmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_employee_department_department_id");

                    b.HasOne("CloudSync.Modules.EmployeeManagement.Models.Employee", "Manager")
                        .WithMany()
                        .HasForeignKey("ManagerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_employee_employee_manager_id");

                    b.Navigation("Coach");

                    b.Navigation("Department");

                    b.Navigation("Manager");
                });

            modelBuilder.Entity("CloudSync.Modules.EmployeeManagement.Models.TeamMember", b =>
                {
                    b.HasOne("CloudSync.Modules.EmployeeManagement.Models.Employee", "Employee")
                        .WithMany()
                        .HasForeignKey("EmployeeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_team_member_employee_employee_id");

                    b.HasOne("CloudSync.Modules.EmployeeManagement.Models.ProjectTeam", "ProjectTeam")
                        .WithMany()
                        .HasForeignKey("ProjectTeamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_team_member_project_team_project_team_id");

                    b.Navigation("Employee");

                    b.Navigation("ProjectTeam");
                });

            modelBuilder.Entity("CloudSync.Modules.UserManagement.Models.InvitedUser", b =>
                {
                    b.HasOne("CloudSync.Modules.UserManagement.Models.User", "InvitedByEmployee")
                        .WithMany()
                        .HasForeignKey("InvitedByEmployeeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_invited_users_users_invited_by_employee_id");

                    b.Navigation("InvitedByEmployee");
                });
#pragma warning restore 612, 618
        }
    }
}
