using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CloudSync.Migrations
{
    /// <inheritdoc />
    public partial class UsePositionClassInEmployeeModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_employees_departments_department_id",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "job_title",
                table: "employees");

            migrationBuilder.RenameColumn(
                name: "department_id",
                table: "employees",
                newName: "position_id");

            migrationBuilder.RenameIndex(
                name: "ix_employees_department_id",
                table: "employees",
                newName: "ix_employees_position_id");

            migrationBuilder.CreateTable(
                name: "position",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    position_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    department_id = table.Column<int>(type: "integer", nullable: false),
                    sub_department_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_position", x => x.id);
                    table.ForeignKey(
                        name: "fk_position_departments_department_id",
                        column: x => x.department_id,
                        principalTable: "departments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_position_departments_sub_department_id",
                        column: x => x.sub_department_id,
                        principalTable: "departments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_position_department_id",
                table: "position",
                column: "department_id");

            migrationBuilder.CreateIndex(
                name: "ix_position_sub_department_id",
                table: "position",
                column: "sub_department_id");

            migrationBuilder.AddForeignKey(
                name: "fk_employees_position_position_id",
                table: "employees",
                column: "position_id",
                principalTable: "position",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_employees_position_position_id",
                table: "employees");

            migrationBuilder.DropTable(
                name: "position");

            migrationBuilder.RenameColumn(
                name: "position_id",
                table: "employees",
                newName: "department_id");

            migrationBuilder.RenameIndex(
                name: "ix_employees_position_id",
                table: "employees",
                newName: "ix_employees_department_id");

            migrationBuilder.AddColumn<string>(
                name: "job_title",
                table: "employees",
                type: "character varying(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "fk_employees_departments_department_id",
                table: "employees",
                column: "department_id",
                principalTable: "departments",
                principalColumn: "id");
        }
    }
}
