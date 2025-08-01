using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ModulesApp.Migrations
{
    /// <inheritdoc />
    public partial class mig10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ModuleFirmware_ModuleIDF_IDFId",
                table: "ModuleFirmware");

            migrationBuilder.DropTable(
                name: "ModuleIDF");

            migrationBuilder.DropIndex(
                name: "IX_ModuleFirmware_IDFId",
                table: "ModuleFirmware");

            migrationBuilder.DropColumn(
                name: "IDFId",
                table: "ModuleFirmware");

            migrationBuilder.AddColumn<string>(
                name: "IdfVersion",
                table: "ModuleFirmware",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdfVersion",
                table: "ModuleFirmware");

            migrationBuilder.AddColumn<long>(
                name: "IDFId",
                table: "ModuleFirmware",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "ModuleIDF",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Path = table.Column<string>(type: "TEXT", nullable: false),
                    Version = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModuleIDF", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ModuleFirmware_IDFId",
                table: "ModuleFirmware",
                column: "IDFId");

            migrationBuilder.AddForeignKey(
                name: "FK_ModuleFirmware_ModuleIDF_IDFId",
                table: "ModuleFirmware",
                column: "IDFId",
                principalTable: "ModuleIDF",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
