using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Calendar.Migrations
{
    /// <inheritdoc />
    public partial class TasksForExchanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DelaySubmissionBSE",
                table: "Tasks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DelaySubmissionCDSL",
                table: "Tasks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DelaySubmissionMCX",
                table: "Tasks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DelaySubmissionNSDEX",
                table: "Tasks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DelaySubmissionNSDL",
                table: "Tasks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DelaySubmissionNSE",
                table: "Tasks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsBSE",
                table: "Tasks",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCDSL",
                table: "Tasks",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsMCX",
                table: "Tasks",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsNSDEX",
                table: "Tasks",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsNSDL",
                table: "Tasks",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsNSE",
                table: "Tasks",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "NonSubmissionBSE",
                table: "Tasks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NonSubmissionCDSL",
                table: "Tasks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NonSubmissionMCX",
                table: "Tasks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NonSubmissionNSDEX",
                table: "Tasks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NonSubmissionNSDL",
                table: "Tasks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NonSubmissionNSE",
                table: "Tasks",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DelaySubmissionBSE",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "DelaySubmissionCDSL",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "DelaySubmissionMCX",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "DelaySubmissionNSDEX",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "DelaySubmissionNSDL",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "DelaySubmissionNSE",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "IsBSE",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "IsCDSL",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "IsMCX",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "IsNSDEX",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "IsNSDL",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "IsNSE",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "NonSubmissionBSE",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "NonSubmissionCDSL",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "NonSubmissionMCX",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "NonSubmissionNSDEX",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "NonSubmissionNSDL",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "NonSubmissionNSE",
                table: "Tasks");
        }
    }
}
