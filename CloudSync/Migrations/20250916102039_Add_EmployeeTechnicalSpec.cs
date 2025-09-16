using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CloudSync.Migrations
{
    /// <inheritdoc />
    public partial class Add_EmployeeTechnicalSpec : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateTable(
                name: "employee_technical_specs",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    laptop_model = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    operating_system = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    cellphone_model = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_employee_technical_specs", x => x.id);
                    table.ForeignKey(
                        name: "fk_employee_technical_specs_employees_id",
                        column: x => x.id,
                        principalTable: "employees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "employee_technical_specs");

            migrationBuilder.DropColumn(
                name: "exit_date",
                table: "employee_positions");

            migrationBuilder.DropColumn(
                name: "hire_date",
                table: "employee_positions");
        }
    }
}
