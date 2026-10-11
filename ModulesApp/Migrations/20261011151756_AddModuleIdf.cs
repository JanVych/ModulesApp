using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ModulesApp.Migrations
{
    /// <inheritdoc />
    public partial class AddModuleIdf : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "IdfId",
                table: "ModuleFirmware",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ModuleIDF",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Version = table.Column<string>(type: "text", nullable: false),
                    Path = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModuleIDF", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ModuleFirmware_IdfId",
                table: "ModuleFirmware",
                column: "IdfId");

            migrationBuilder.CreateIndex(
                name: "IX_ModuleIDF_Version",
                table: "ModuleIDF",
                column: "Version",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ModuleFirmware_ModuleIDF_IdfId",
                table: "ModuleFirmware",
                column: "IdfId",
                principalTable: "ModuleIDF",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            // keep existing IDF versions (paths are filled in later)
            migrationBuilder.Sql(@"
                INSERT INTO ""ModuleIDF"" (""Version"", ""Path"")
                SELECT DISTINCT ""IdfVersion"", '' FROM ""ModuleFirmware"" WHERE ""IdfVersion"" <> '';
                UPDATE ""ModuleFirmware"" f SET ""IdfId"" = i.""Id""
                FROM ""ModuleIDF"" i WHERE i.""Version"" = f.""IdfVersion"";");

            migrationBuilder.DropColumn(
                name: "IdfVersion",
                table: "ModuleFirmware");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IdfVersion",
                table: "ModuleFirmware",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql(@"
                UPDATE ""ModuleFirmware"" f SET ""IdfVersion"" = i.""Version""
                FROM ""ModuleIDF"" i WHERE i.""Id"" = f.""IdfId"";");

            migrationBuilder.DropForeignKey(
                name: "FK_ModuleFirmware_ModuleIDF_IdfId",
                table: "ModuleFirmware");

            migrationBuilder.DropTable(
                name: "ModuleIDF");

            migrationBuilder.DropIndex(
                name: "IX_ModuleFirmware_IdfId",
                table: "ModuleFirmware");

            migrationBuilder.DropColumn(
                name: "IdfId",
                table: "ModuleFirmware");
        }
    }
}
