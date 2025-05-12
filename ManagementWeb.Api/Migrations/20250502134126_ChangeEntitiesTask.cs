using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManagementWeb.Api.Migrations
{
    /// <inheritdoc />
    public partial class ChangeEntitiesTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminChangeUserId",
                table: "Tasks");

            migrationBuilder.RenameColumn(
                name: "ChangeStatusUserId",
                table: "Tasks",
                newName: "AdminId");

            migrationBuilder.RenameColumn(
                name: "ChangeStatusTime",
                table: "Tasks",
                newName: "InspectorChangeTime");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "InspectorChangeTime",
                table: "Tasks",
                newName: "ChangeStatusTime");

            migrationBuilder.RenameColumn(
                name: "AdminId",
                table: "Tasks",
                newName: "ChangeStatusUserId");

            migrationBuilder.AddColumn<Guid>(
                name: "AdminChangeUserId",
                table: "Tasks",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
