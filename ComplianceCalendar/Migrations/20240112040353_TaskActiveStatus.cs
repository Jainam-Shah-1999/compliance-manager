using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Calendar.Migrations
{
    /// <inheritdoc />
    public partial class TaskActiveStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "InactiveDate",
                table: "Tasks",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Tasks",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InactiveDate",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Tasks");
        }
    }
}
