using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CloudSync.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeeLegal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "id_type",
                table: "employee_legals",
                newName: "personal_id_type");

            migrationBuilder.RenameColumn(
                name: "id_state",
                table: "employee_legals",
                newName: "personal_id_state");

            migrationBuilder.RenameColumn(
                name: "id_number",
                table: "employee_legals",
                newName: "personal_id_number");

            migrationBuilder.RenameColumn(
                name: "id_country",
                table: "employee_legals",
                newName: "personal_id_country");

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

            migrationBuilder.AddColumn<string>(
                name: "agreements",
                table: "employee_legals",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "data_privacy_consents",
                table: "employee_legals",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "disciplinary_records",
                table: "employee_legals",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "eeo_reporting",
                table: "employee_legals",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "employee_contracts",
                table: "employee_legals",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "labor_law_compliance",
                table: "employee_legals",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "policy_acknowledgements",
                table: "employee_legals",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "exit_date",
                table: "employee_positions");

            migrationBuilder.DropColumn(
                name: "hire_date",
                table: "employee_positions");

            migrationBuilder.DropColumn(
                name: "agreements",
                table: "employee_legals");

            migrationBuilder.DropColumn(
                name: "data_privacy_consents",
                table: "employee_legals");

            migrationBuilder.DropColumn(
                name: "disciplinary_records",
                table: "employee_legals");

            migrationBuilder.DropColumn(
                name: "eeo_reporting",
                table: "employee_legals");

            migrationBuilder.DropColumn(
                name: "employee_contracts",
                table: "employee_legals");

            migrationBuilder.DropColumn(
                name: "labor_law_compliance",
                table: "employee_legals");

            migrationBuilder.DropColumn(
                name: "policy_acknowledgements",
                table: "employee_legals");

            migrationBuilder.RenameColumn(
                name: "personal_id_type",
                table: "employee_legals",
                newName: "id_type");

            migrationBuilder.RenameColumn(
                name: "personal_id_state",
                table: "employee_legals",
                newName: "id_state");

            migrationBuilder.RenameColumn(
                name: "personal_id_number",
                table: "employee_legals",
                newName: "id_number");

            migrationBuilder.RenameColumn(
                name: "personal_id_country",
                table: "employee_legals",
                newName: "id_country");
        }
    }
}
