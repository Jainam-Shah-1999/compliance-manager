using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Calendar.Migrations
{
    /// <inheritdoc />
    public partial class updatedTaskProperties1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OriginalTaskId",
                table: "TaskStatus");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OriginalTaskId",
                table: "TaskStatus",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
