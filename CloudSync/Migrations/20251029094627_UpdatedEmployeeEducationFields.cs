using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CloudSync.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedEmployeeEducationFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "graduate_degree",
                table: "employee_educations",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "graduate_school",
                table: "employee_educations",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "undergraduate_degree",
                table: "employee_educations",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "undergraduate_school",
                table: "employee_educations",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "graduate_degree",
                table: "employee_educations");

            migrationBuilder.DropColumn(
                name: "graduate_school",
                table: "employee_educations");

            migrationBuilder.DropColumn(
                name: "undergraduate_degree",
                table: "employee_educations");

            migrationBuilder.DropColumn(
                name: "undergraduate_school",
                table: "employee_educations");
        }
    }
}
