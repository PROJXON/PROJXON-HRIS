using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CloudSync.Migrations
{
    /// <inheritdoc />
    public partial class AddPositionModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "hierarchy_level",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "social_security_number",
                table: "employees");

            migrationBuilder.AddColumn<string>(
                name: "hierarchy_level",
                table: "position",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "emergency_contact_phone",
                table: "employees",
                type: "character varying(15)",
                maxLength: 15,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "country",
                table: "employees",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "hierarchy_level",
                table: "position");

            migrationBuilder.DropColumn(
                name: "country",
                table: "employees");

            migrationBuilder.AlterColumn<string>(
                name: "emergency_contact_phone",
                table: "employees",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(15)",
                oldMaxLength: 15,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "hierarchy_level",
                table: "employees",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "social_security_number",
                table: "employees",
                type: "character varying(9)",
                maxLength: 9,
                nullable: true);
        }
    }
}
