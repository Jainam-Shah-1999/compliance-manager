using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Calendar.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "Users",
                newName: "RepresentativeName");

            migrationBuilder.RenameColumn(
                name: "FirstName",
                table: "Users",
                newName: "CompanyName");

            migrationBuilder.AddColumn<int>(
                name: "ContactNumber",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContactNumber",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "RepresentativeName",
                table: "Users",
                newName: "LastName");

            migrationBuilder.RenameColumn(
                name: "CompanyName",
                table: "Users",
                newName: "FirstName");
        }
    }
}
