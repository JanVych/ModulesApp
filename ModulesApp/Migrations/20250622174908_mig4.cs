using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ModulesApp.Migrations
{
    /// <inheritdoc />
    public partial class mig4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TargetData",
                table: "TaskLink",
                newName: "TargetDataType");

            migrationBuilder.RenameColumn(
                name: "SourceData",
                table: "TaskLink",
                newName: "SourceDataType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TargetDataType",
                table: "TaskLink",
                newName: "TargetData");

            migrationBuilder.RenameColumn(
                name: "SourceDataType",
                table: "TaskLink",
                newName: "SourceData");
        }
    }
}
