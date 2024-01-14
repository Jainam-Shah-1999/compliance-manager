using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Calendar.Migrations
{
    /// <inheritdoc />
    public partial class renameNCDEX : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NonSubmissionNSDEX",
                table: "Tasks",
                newName: "NonSubmissionNCDEX");

            migrationBuilder.RenameColumn(
                name: "IsNSDEX",
                table: "Tasks",
                newName: "IsNCDEX");

            migrationBuilder.RenameColumn(
                name: "DelaySubmissionNSDEX",
                table: "Tasks",
                newName: "DelaySubmissionNCDEX");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NonSubmissionNCDEX",
                table: "Tasks",
                newName: "NonSubmissionNSDEX");

            migrationBuilder.RenameColumn(
                name: "IsNCDEX",
                table: "Tasks",
                newName: "IsNSDEX");

            migrationBuilder.RenameColumn(
                name: "DelaySubmissionNCDEX",
                table: "Tasks",
                newName: "DelaySubmissionNSDEX");
        }
    }
}
