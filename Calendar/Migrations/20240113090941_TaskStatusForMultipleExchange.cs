using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Calendar.Migrations
{
    /// <inheritdoc />
    public partial class TaskStatusForMultipleExchange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Status",
                table: "TaskStatus",
                newName: "NSEStatus");

            migrationBuilder.AddColumn<int>(
                name: "BSEStatus",
                table: "TaskStatus",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CDSLStatus",
                table: "TaskStatus",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MCXStatus",
                table: "TaskStatus",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NCDEXStatus",
                table: "TaskStatus",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NSDLStatus",
                table: "TaskStatus",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BSEStatus",
                table: "TaskStatus");

            migrationBuilder.DropColumn(
                name: "CDSLStatus",
                table: "TaskStatus");

            migrationBuilder.DropColumn(
                name: "MCXStatus",
                table: "TaskStatus");

            migrationBuilder.DropColumn(
                name: "NCDEXStatus",
                table: "TaskStatus");

            migrationBuilder.DropColumn(
                name: "NSDLStatus",
                table: "TaskStatus");

            migrationBuilder.RenameColumn(
                name: "NSEStatus",
                table: "TaskStatus",
                newName: "Status");
        }
    }
}
