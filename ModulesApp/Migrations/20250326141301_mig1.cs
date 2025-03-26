using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ModulesApp.Migrations
{
    /// <inheritdoc />
    public partial class mig1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BackgroundService",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Interval = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    LastRun = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Data = table.Column<string>(type: "TEXT", nullable: false),
                    IsRunning = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BackgroundService", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Dashboard",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    IconString = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dashboard", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Module",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Key = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    ProgramName = table.Column<string>(type: "TEXT", nullable: true),
                    ProgramStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    ProgramVersion = table.Column<string>(type: "TEXT", nullable: true),
                    Chip = table.Column<string>(type: "TEXT", nullable: true),
                    IDFVersion = table.Column<string>(type: "TEXT", nullable: true),
                    FirmwareVersion = table.Column<string>(type: "TEXT", nullable: true),
                    FlashSize = table.Column<int>(type: "INTEGER", nullable: true),
                    FreeHeap = table.Column<int>(type: "INTEGER", nullable: true),
                    WifiCurrent = table.Column<string>(type: "TEXT", nullable: true),
                    LastResponse = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Data = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Module", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DashBoardEntity",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Data = table.Column<string>(type: "TEXT", nullable: false),
                    DashboardId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DashBoardEntity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DashBoardEntity_Dashboard_DashboardId",
                        column: x => x.DashboardId,
                        principalTable: "Dashboard",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Action",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Key = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: false),
                    ModuleId = table.Column<long>(type: "INTEGER", nullable: false),
                    BackgroundServiceId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Action", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Action_BackgroundService_BackgroundServiceId",
                        column: x => x.BackgroundServiceId,
                        principalTable: "BackgroundService",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Action_Module_ModuleId",
                        column: x => x.ModuleId,
                        principalTable: "Module",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Task",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    IntervalSeconds = table.Column<int>(type: "INTEGER", nullable: false),
                    LastRun = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModuleId = table.Column<long>(type: "INTEGER", nullable: false),
                    BackgroundServiceId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Task", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Task_BackgroundService_BackgroundServiceId",
                        column: x => x.BackgroundServiceId,
                        principalTable: "BackgroundService",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Task_Module_ModuleId",
                        column: x => x.ModuleId,
                        principalTable: "Module",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaskNode",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    SubType = table.Column<int>(type: "INTEGER", nullable: false),
                    InputType = table.Column<int>(type: "INTEGER", nullable: false),
                    StringVal1 = table.Column<string>(type: "TEXT", nullable: false),
                    StringVal2 = table.Column<string>(type: "TEXT", nullable: false),
                    StringVal3 = table.Column<string>(type: "TEXT", nullable: false),
                    DoubleVal1 = table.Column<double>(type: "REAL", nullable: false),
                    LongVal1 = table.Column<long>(type: "INTEGER", nullable: false),
                    LongVal2 = table.Column<long>(type: "INTEGER", nullable: false),
                    BoolVal1 = table.Column<bool>(type: "INTEGER", nullable: false),
                    PositionX = table.Column<double>(type: "REAL", nullable: false),
                    PositionY = table.Column<double>(type: "REAL", nullable: false),
                    Order = table.Column<int>(type: "INTEGER", nullable: false),
                    TaskId = table.Column<long>(type: "INTEGER", nullable: false),
                    NodeType = table.Column<string>(type: "TEXT", maxLength: 21, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskNode", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskNode_Task_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Task",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaskLink",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SourceNodeId = table.Column<long>(type: "INTEGER", nullable: false),
                    TargetNodeId = table.Column<long>(type: "INTEGER", nullable: false),
                    SourceOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    SourceInput = table.Column<bool>(type: "INTEGER", nullable: false),
                    SourceData = table.Column<bool>(type: "INTEGER", nullable: false),
                    TargetOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    TargetInput = table.Column<bool>(type: "INTEGER", nullable: false),
                    TargetData = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskLink", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskLink_TaskNode_SourceNodeId",
                        column: x => x.SourceNodeId,
                        principalTable: "TaskNode",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskLink_TaskNode_TargetNodeId",
                        column: x => x.TargetNodeId,
                        principalTable: "TaskNode",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Action_BackgroundServiceId",
                table: "Action",
                column: "BackgroundServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Action_ModuleId",
                table: "Action",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_DashBoardEntity_DashboardId",
                table: "DashBoardEntity",
                column: "DashboardId");

            migrationBuilder.CreateIndex(
                name: "IX_Task_BackgroundServiceId",
                table: "Task",
                column: "BackgroundServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Task_ModuleId",
                table: "Task",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskLink_SourceNodeId",
                table: "TaskLink",
                column: "SourceNodeId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskLink_TargetNodeId",
                table: "TaskLink",
                column: "TargetNodeId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskNode_TaskId",
                table: "TaskNode",
                column: "TaskId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Action");

            migrationBuilder.DropTable(
                name: "DashBoardEntity");

            migrationBuilder.DropTable(
                name: "TaskLink");

            migrationBuilder.DropTable(
                name: "Dashboard");

            migrationBuilder.DropTable(
                name: "TaskNode");

            migrationBuilder.DropTable(
                name: "Task");

            migrationBuilder.DropTable(
                name: "BackgroundService");

            migrationBuilder.DropTable(
                name: "Module");
        }
    }
}
