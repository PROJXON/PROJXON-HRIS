using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CloudSync.Migrations
{
    /// <inheritdoc />
    public partial class AddDefaultValueToCreateDateTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDateTime",
                table: "Users",
                nullable: false,
                defaultValueSql: "NOW()");
            
            migrationBuilder.AlterColumn<DateTime>(
                name: "LastLoginDateTime",
                table: "Users",
                nullable: false,
                defaultValueSql: "NOW()");
            
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDateTime",
                table: "Employee",
                nullable: false,
                defaultValueSql: "NOW()");
            
            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdateDateTime",
                table: "Employee",
                nullable: false,
                defaultValueSql: "NOW()");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
