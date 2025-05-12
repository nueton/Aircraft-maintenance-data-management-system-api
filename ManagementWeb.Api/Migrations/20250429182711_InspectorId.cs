using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManagementWeb.Api.Migrations
{
    /// <inheritdoc />
    public partial class InspectorId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Inspector",
                table: "Tasks");

            migrationBuilder.AddColumn<Guid>(
                name: "InspectorId",
                table: "Tasks",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InspectorId",
                table: "Tasks");

            migrationBuilder.AddColumn<string>(
                name: "Inspector",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
