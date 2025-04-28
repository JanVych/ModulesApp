using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ModulesApp.Migrations
{
    /// <inheritdoc />
    public partial class mig8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "DashboardEntityId",
                table: "Task",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Task_DashboardEntityId",
                table: "Task",
                column: "DashboardEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Task_DashBoardEntity_DashboardEntityId",
                table: "Task",
                column: "DashboardEntityId",
                principalTable: "DashBoardEntity",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Task_DashBoardEntity_DashboardEntityId",
                table: "Task");

            migrationBuilder.DropIndex(
                name: "IX_Task_DashboardEntityId",
                table: "Task");

            migrationBuilder.DropColumn(
                name: "DashboardEntityId",
                table: "Task");
        }
    }
}
