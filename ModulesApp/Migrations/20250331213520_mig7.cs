using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ModulesApp.Migrations
{
    /// <inheritdoc />
    public partial class mig7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "ModuleProgramFile",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Path",
                table: "ModuleProgramFile",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "ModuleProgram",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Path",
                table: "ModuleProgram",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "ModuleIDF",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Path",
                table: "ModuleIDF",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Version",
                table: "ModuleIDF",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "ModuleFirmware",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Path",
                table: "ModuleFirmware",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Version",
                table: "ModuleFirmware",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "ModuleProgramFile");

            migrationBuilder.DropColumn(
                name: "Path",
                table: "ModuleProgramFile");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "ModuleProgram");

            migrationBuilder.DropColumn(
                name: "Path",
                table: "ModuleProgram");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "ModuleIDF");

            migrationBuilder.DropColumn(
                name: "Path",
                table: "ModuleIDF");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "ModuleIDF");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "ModuleFirmware");

            migrationBuilder.DropColumn(
                name: "Path",
                table: "ModuleFirmware");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "ModuleFirmware");
        }
    }
}
