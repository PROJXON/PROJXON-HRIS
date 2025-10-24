using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CloudSync.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedEmployeeBasicFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "basic_info_birth_city",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "basic_info_birth_country",
                table: "employees");

            migrationBuilder.RenameColumn(
                name: "basic_info_nick_name",
                table: "employees",
                newName: "basic_info_race");

            migrationBuilder.RenameColumn(
                name: "basic_info_birth_state",
                table: "employees",
                newName: "basic_info_name_pronunciation");

            migrationBuilder.RenameColumn(
                name: "basic_info_birth_date",
                table: "employees",
                newName: "basic_info_date_of_birth");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "basic_info_race",
                table: "employees",
                newName: "basic_info_nick_name");

            migrationBuilder.RenameColumn(
                name: "basic_info_name_pronunciation",
                table: "employees",
                newName: "basic_info_birth_state");

            migrationBuilder.RenameColumn(
                name: "basic_info_date_of_birth",
                table: "employees",
                newName: "basic_info_birth_date");

            migrationBuilder.AddColumn<string>(
                name: "basic_info_birth_city",
                table: "employees",
                type: "character varying(40)",
                maxLength: 40,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "basic_info_birth_country",
                table: "employees",
                type: "character varying(40)",
                maxLength: 40,
                nullable: true);
        }
    }
}
