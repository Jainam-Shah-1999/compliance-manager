using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Calendar.Migrations
{
    /// <inheritdoc />
    public partial class ActiveStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MarkInactive",
                table: "Tasks",
                newName: "Inactive");

            migrationBuilder.AddColumn<bool>(
                name: "Inactive",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Inactive",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "Inactive",
                table: "Tasks",
                newName: "MarkInactive");
        }
    }
}
