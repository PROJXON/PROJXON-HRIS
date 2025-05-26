using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CloudSync.Migrations
{
    /// <inheritdoc />
    public partial class SetUserPkBackToId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvitedUsers_Users_InvitedByEmployeeId",
                table: "InvitedUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.Sql(
                "ALTER TABLE \"InvitedUsers\" ALTER COLUMN \"InvitedByEmployeeId\" TYPE integer USING \"InvitedByEmployeeId\"::integer;"
            );

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InvitedUsers_Users_InvitedByEmployeeId",
                table: "InvitedUsers",
                column: "InvitedByEmployeeId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvitedUsers_Users_InvitedByEmployeeId",
                table: "InvitedUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.AlterColumn<string>(
                name: "InvitedByEmployeeId",
                table: "InvitedUsers",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldMaxLength: 255);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "GoogleUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_InvitedUsers_Users_InvitedByEmployeeId",
                table: "InvitedUsers",
                column: "InvitedByEmployeeId",
                principalTable: "Users",
                principalColumn: "GoogleUserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
