using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CloudSync.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedEmployeeContactInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "contact_info_address_time_zone",
                table: "employees",
                newName: "contact_info_permanent_address_time_zone");

            migrationBuilder.RenameColumn(
                name: "contact_info_address_state_or_province",
                table: "employees",
                newName: "contact_info_permanent_address_state_or_province");

            migrationBuilder.RenameColumn(
                name: "contact_info_address_postal_code",
                table: "employees",
                newName: "contact_info_permanent_address_postal_code");

            migrationBuilder.RenameColumn(
                name: "contact_info_address_country",
                table: "employees",
                newName: "contact_info_permanent_address_country");

            migrationBuilder.RenameColumn(
                name: "contact_info_address_city",
                table: "employees",
                newName: "contact_info_permanent_address_city");

            migrationBuilder.RenameColumn(
                name: "contact_info_address_address_line2",
                table: "employees",
                newName: "contact_info_permanent_address_address_line2");

            migrationBuilder.RenameColumn(
                name: "contact_info_address_address_line1",
                table: "employees",
                newName: "contact_info_permanent_address_address_line1");

            migrationBuilder.RenameColumn(
                name: "contact_info_phone",
                table: "employees",
                newName: "contact_info_phone_number");

            migrationBuilder.RenameColumn(
                name: "contact_info_international_phone",
                table: "employees",
                newName: "contact_info_mailing_address_time_zone");

            migrationBuilder.RenameColumn(
                name: "contact_info_emergency_contact_phone",
                table: "employees",
                newName: "contact_info_mailing_address_postal_code");

            migrationBuilder.AddColumn<string>(
                name: "contact_info_emergency_contact_phone_number",
                table: "employees",
                type: "character varying(15)",
                maxLength: 15,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "contact_info_international_phone_number",
                table: "employees",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "contact_info_mailing_address_address_line1",
                table: "employees",
                type: "character varying(80)",
                maxLength: 80,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "contact_info_mailing_address_address_line2",
                table: "employees",
                type: "character varying(80)",
                maxLength: 80,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "contact_info_mailing_address_city",
                table: "employees",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "contact_info_mailing_address_country",
                table: "employees",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "contact_info_mailing_address_state_or_province",
                table: "employees",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "contact_info_emergency_contact_phone_number",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "contact_info_international_phone_number",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "contact_info_mailing_address_address_line1",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "contact_info_mailing_address_address_line2",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "contact_info_mailing_address_city",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "contact_info_mailing_address_country",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "contact_info_mailing_address_state_or_province",
                table: "employees");

            migrationBuilder.RenameColumn(
                name: "contact_info_permanent_address_time_zone",
                table: "employees",
                newName: "contact_info_address_time_zone");

            migrationBuilder.RenameColumn(
                name: "contact_info_permanent_address_state_or_province",
                table: "employees",
                newName: "contact_info_address_state_or_province");

            migrationBuilder.RenameColumn(
                name: "contact_info_permanent_address_postal_code",
                table: "employees",
                newName: "contact_info_address_postal_code");

            migrationBuilder.RenameColumn(
                name: "contact_info_permanent_address_country",
                table: "employees",
                newName: "contact_info_address_country");

            migrationBuilder.RenameColumn(
                name: "contact_info_permanent_address_city",
                table: "employees",
                newName: "contact_info_address_city");

            migrationBuilder.RenameColumn(
                name: "contact_info_permanent_address_address_line2",
                table: "employees",
                newName: "contact_info_address_address_line2");

            migrationBuilder.RenameColumn(
                name: "contact_info_permanent_address_address_line1",
                table: "employees",
                newName: "contact_info_address_address_line1");

            migrationBuilder.RenameColumn(
                name: "contact_info_phone_number",
                table: "employees",
                newName: "contact_info_phone");

            migrationBuilder.RenameColumn(
                name: "contact_info_mailing_address_time_zone",
                table: "employees",
                newName: "contact_info_international_phone");

            migrationBuilder.RenameColumn(
                name: "contact_info_mailing_address_postal_code",
                table: "employees",
                newName: "contact_info_emergency_contact_phone");
        }
    }
}
