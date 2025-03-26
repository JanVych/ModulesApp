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
            migrationBuilder.DropColumn(
                name: "IsRunning",
                table: "BackgroundService");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRunning",
                table: "BackgroundService",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }
    }
}
