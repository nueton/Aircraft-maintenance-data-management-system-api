using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManagementWeb.Api.Migrations
{
    /// <inheritdoc />
    public partial class DetailUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CreateUserId",
                table: "Users",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTime",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ResetPasswordId",
                table: "Users",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "ResetTime",
                table: "Users",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreateUserId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CreatedTime",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ResetPasswordId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ResetTime",
                table: "Users");
        }
    }
}
