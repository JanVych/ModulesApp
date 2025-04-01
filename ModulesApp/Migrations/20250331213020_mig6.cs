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
            migrationBuilder.CreateTable(
                name: "ModuleIDF",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModuleIDF", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ModuleFirmware",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IDFId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModuleFirmware", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModuleFirmware_ModuleIDF_IDFId",
                        column: x => x.IDFId,
                        principalTable: "ModuleIDF",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ModuleProgram",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FirmwareId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModuleProgram", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModuleProgram_ModuleFirmware_FirmwareId",
                        column: x => x.FirmwareId,
                        principalTable: "ModuleFirmware",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ModuleProgramFile",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Content = table.Column<string>(type: "TEXT", nullable: true),
                    ProgramId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModuleProgramFile", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModuleProgramFile_ModuleProgram_ProgramId",
                        column: x => x.ProgramId,
                        principalTable: "ModuleProgram",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ModuleFirmware_IDFId",
                table: "ModuleFirmware",
                column: "IDFId");

            migrationBuilder.CreateIndex(
                name: "IX_ModuleProgram_FirmwareId",
                table: "ModuleProgram",
                column: "FirmwareId");

            migrationBuilder.CreateIndex(
                name: "IX_ModuleProgramFile_ProgramId",
                table: "ModuleProgramFile",
                column: "ProgramId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ModuleProgramFile");

            migrationBuilder.DropTable(
                name: "ModuleProgram");

            migrationBuilder.DropTable(
                name: "ModuleFirmware");

            migrationBuilder.DropTable(
                name: "ModuleIDF");
        }
    }
}
