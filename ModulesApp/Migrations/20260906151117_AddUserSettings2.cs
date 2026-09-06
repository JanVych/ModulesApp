using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ModulesApp.Migrations
{
    /// <inheritdoc />
    public partial class AddUserSettings2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UiSettings",
                table: "UserSettings",
                newName: "Settings");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Settings",
                table: "UserSettings",
                newName: "UiSettings");
        }
    }
}
