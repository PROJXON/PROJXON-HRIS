using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CloudSync.Migrations
{
    /// <inheritdoc />
    public partial class PendingModelChangesFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "google_user_id",
                table: "user_response");

            migrationBuilder.DropColumn(
                name: "role_id",
                table: "user_response");

            migrationBuilder.DropColumn(
                name: "role_name",
                table: "user_response");

            migrationBuilder.DropColumn(
                name: "contact_info_discord_username",
                table: "employees");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "google_user_id",
                table: "user_response",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "role_id",
                table: "user_response",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "role_name",
                table: "user_response",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "contact_info_discord_username",
                table: "employees",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);
        }
    }
}
