using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Calendar.Migrations
{
    /// <inheritdoc />
    public partial class DelayDaysForTaskStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BSEDelayDays",
                table: "TaskStatus",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CDSLDelayDays",
                table: "TaskStatus",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MCXDelayDays",
                table: "TaskStatus",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NCDEXDelayDays",
                table: "TaskStatus",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NSDLDelayDays",
                table: "TaskStatus",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NSEDelayDays",
                table: "TaskStatus",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BSEDelayDays",
                table: "TaskStatus");

            migrationBuilder.DropColumn(
                name: "CDSLDelayDays",
                table: "TaskStatus");

            migrationBuilder.DropColumn(
                name: "MCXDelayDays",
                table: "TaskStatus");

            migrationBuilder.DropColumn(
                name: "NCDEXDelayDays",
                table: "TaskStatus");

            migrationBuilder.DropColumn(
                name: "NSDLDelayDays",
                table: "TaskStatus");

            migrationBuilder.DropColumn(
                name: "NSEDelayDays",
                table: "TaskStatus");
        }
    }
}
