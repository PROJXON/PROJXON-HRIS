using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CloudSync.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedEmployeePositionFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "employee_life_cycle_stage",
                table: "employee_positions",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "previous_employers",
                table: "employee_positions",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "projxon_email",
                table: "employee_positions",
                type: "character varying(60)",
                maxLength: 60,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "employee_life_cycle_stage",
                table: "employee_positions");

            migrationBuilder.DropColumn(
                name: "previous_employers",
                table: "employee_positions");

            migrationBuilder.DropColumn(
                name: "projxon_email",
                table: "employee_positions");
        }
    }
}
