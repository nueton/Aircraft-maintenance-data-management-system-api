using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManagementWeb.Api.Migrations
{
    /// <inheritdoc />
    public partial class ChangeJCN : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "JCH",
                table: "Tasks",
                newName: "JCN");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "JCN",
                table: "Tasks",
                newName: "JCH");
        }
    }
}
