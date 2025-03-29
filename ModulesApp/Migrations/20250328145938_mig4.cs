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
            migrationBuilder.DropForeignKey(
                name: "FK_Task_BackgroundService_BackgroundServiceId",
                table: "Task");

            migrationBuilder.DropForeignKey(
                name: "FK_Task_Module_ModuleId",
                table: "Task");

            migrationBuilder.AlterColumn<long>(
                name: "ModuleId",
                table: "Task",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<long>(
                name: "BackgroundServiceId",
                table: "Task",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "INTEGER");

            migrationBuilder.AddForeignKey(
                name: "FK_Task_BackgroundService_BackgroundServiceId",
                table: "Task",
                column: "BackgroundServiceId",
                principalTable: "BackgroundService",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Task_Module_ModuleId",
                table: "Task",
                column: "ModuleId",
                principalTable: "Module",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Task_BackgroundService_BackgroundServiceId",
                table: "Task");

            migrationBuilder.DropForeignKey(
                name: "FK_Task_Module_ModuleId",
                table: "Task");

            migrationBuilder.AlterColumn<long>(
                name: "ModuleId",
                table: "Task",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "BackgroundServiceId",
                table: "Task",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Task_BackgroundService_BackgroundServiceId",
                table: "Task",
                column: "BackgroundServiceId",
                principalTable: "BackgroundService",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Task_Module_ModuleId",
                table: "Task",
                column: "ModuleId",
                principalTable: "Module",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
