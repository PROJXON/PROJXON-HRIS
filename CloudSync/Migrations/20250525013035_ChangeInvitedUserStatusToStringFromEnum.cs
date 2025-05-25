using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CloudSync.Migrations
{
    /// <inheritdoc />
    public partial class ChangeInvitedUserStatusToStringFromEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvitedUsers_Users_InvitedById",
                table: "InvitedUsers");

            migrationBuilder.RenameColumn(
                name: "InvitedById",
                table: "InvitedUsers",
                newName: "InvitedByEmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_InvitedUsers_InvitedById",
                table: "InvitedUsers",
                newName: "IX_InvitedUsers_InvitedByEmployeeId");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "InvitedUsers",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldMaxLength: 20);

            migrationBuilder.AddForeignKey(
                name: "FK_InvitedUsers_Users_InvitedByEmployeeId",
                table: "InvitedUsers",
                column: "InvitedByEmployeeId",
                principalTable: "Users",
                principalColumn: "GoogleUserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvitedUsers_Users_InvitedByEmployeeId",
                table: "InvitedUsers");

            migrationBuilder.RenameColumn(
                name: "InvitedByEmployeeId",
                table: "InvitedUsers",
                newName: "InvitedById");

            migrationBuilder.RenameIndex(
                name: "IX_InvitedUsers_InvitedByEmployeeId",
                table: "InvitedUsers",
                newName: "IX_InvitedUsers_InvitedById");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "InvitedUsers",
                type: "integer",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AddForeignKey(
                name: "FK_InvitedUsers_Users_InvitedById",
                table: "InvitedUsers",
                column: "InvitedById",
                principalTable: "Users",
                principalColumn: "GoogleUserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
