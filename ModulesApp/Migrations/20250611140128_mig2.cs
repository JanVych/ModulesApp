using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ModulesApp.Migrations
{
    /// <inheritdoc />
    public partial class mig2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Data",
                table: "BackgroundService",
                newName: "MessageData");

            migrationBuilder.AddColumn<string>(
                name: "ConfigurationData",
                table: "BackgroundService",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConfigurationData",
                table: "BackgroundService");

            migrationBuilder.RenameColumn(
                name: "MessageData",
                table: "BackgroundService",
                newName: "Data");
        }
    }
}
