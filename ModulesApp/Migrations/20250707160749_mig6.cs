using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ModulesApp.Migrations
{
    /// <inheritdoc />
    public partial class mig6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WifiPasswordCurrent",
                table: "Module",
                newName: "WifiSsid");

            migrationBuilder.RenameColumn(
                name: "WifiCurrent",
                table: "Module",
                newName: "WifiPassword");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WifiSsid",
                table: "Module",
                newName: "WifiPasswordCurrent");

            migrationBuilder.RenameColumn(
                name: "WifiPassword",
                table: "Module",
                newName: "WifiCurrent");
        }
    }
}
