using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CloudSync.Migrations
{
    /// <inheritdoc />
    public partial class AddToEmployeeTraining : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "development_plan",
                table: "employee_trainings",
                type: "character varying(40)",
                maxLength: 40,
                nullable: true);

            migrationBuilder.AddColumn<List<string>>(
                name: "evaluation",
                table: "employee_trainings",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "leadership_participation",
                table: "employee_trainings",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "participant_dashboard",
                table: "employee_trainings",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "participant_meeting_link",
                table: "employee_trainings",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "training_hours_logged",
                table: "employee_trainings",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "exit_date",
                table: "employee_positions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "hire_date",
                table: "employee_positions",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "development_plan",
                table: "employee_trainings");

            migrationBuilder.DropColumn(
                name: "evaluation",
                table: "employee_trainings");

            migrationBuilder.DropColumn(
                name: "leadership_participation",
                table: "employee_trainings");

            migrationBuilder.DropColumn(
                name: "participant_dashboard",
                table: "employee_trainings");

            migrationBuilder.DropColumn(
                name: "participant_meeting_link",
                table: "employee_trainings");

            migrationBuilder.DropColumn(
                name: "training_hours_logged",
                table: "employee_trainings");

            migrationBuilder.DropColumn(
                name: "exit_date",
                table: "employee_positions");

            migrationBuilder.DropColumn(
                name: "hire_date",
                table: "employee_positions");
        }
    }
}
