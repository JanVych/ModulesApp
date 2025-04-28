using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ModulesApp.Migrations
{
    /// <inheritdoc />
    public partial class mig9 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Action_BackgroundService_BackgroundServiceId",
                table: "Action");

            migrationBuilder.DropForeignKey(
                name: "FK_Action_Module_ModuleId",
                table: "Action");

            migrationBuilder.AlterColumn<long>(
                name: "ModuleId",
                table: "Action",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<long>(
                name: "BackgroundServiceId",
                table: "Action",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "INTEGER");

            migrationBuilder.AddForeignKey(
                name: "FK_Action_BackgroundService_BackgroundServiceId",
                table: "Action",
                column: "BackgroundServiceId",
                principalTable: "BackgroundService",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Action_Module_ModuleId",
                table: "Action",
                column: "ModuleId",
                principalTable: "Module",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Action_BackgroundService_BackgroundServiceId",
                table: "Action");

            migrationBuilder.DropForeignKey(
                name: "FK_Action_Module_ModuleId",
                table: "Action");

            migrationBuilder.AlterColumn<long>(
                name: "ModuleId",
                table: "Action",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "BackgroundServiceId",
                table: "Action",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Action_BackgroundService_BackgroundServiceId",
                table: "Action",
                column: "BackgroundServiceId",
                principalTable: "BackgroundService",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Action_Module_ModuleId",
                table: "Action",
                column: "ModuleId",
                principalTable: "Module",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
