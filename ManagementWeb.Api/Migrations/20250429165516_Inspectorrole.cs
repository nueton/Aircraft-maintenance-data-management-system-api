using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManagementWeb.Api.Migrations
{
    /// <inheritdoc />
    public partial class Inspectorrole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AdminChangeTime",
                table: "Tasks",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "AdminChangeUserId",
                table: "Tasks",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminChangeTime",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "AdminChangeUserId",
                table: "Tasks");
        }
    }
}
