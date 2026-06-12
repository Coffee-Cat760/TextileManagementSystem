using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using TextileManagementSystem.Data;

#nullable disable

namespace TextileManagementSystem.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260611002000_AddProductionModule")]
    public partial class AddProductionModule : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductionOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderNumber = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    ClientName = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    ItemName = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Material = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    CurrentStage = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ProgressPercentage = table.Column<int>(type: "int", nullable: false),
                    TargetDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionOrders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DailyProductionOutputs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductionOrderId = table.Column<int>(type: "int", nullable: false),
                    ProductionStage = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    AssignedWorkers = table.Column<string>(type: "nvarchar(180)", maxLength: 180, nullable: false),
                    QuantityCompleted = table.Column<int>(type: "int", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    OutputDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyProductionOutputs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DailyProductionOutputs_ProductionOrders_ProductionOrderId",
                        column: x => x.ProductionOrderId,
                        principalTable: "ProductionOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductionAlerts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductionOrderId = table.Column<int>(type: "int", nullable: false),
                    ItemType = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    DelayedStage = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    TargetDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AssignedTeam = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    DaysBehind = table.Column<int>(type: "int", nullable: false),
                    SuggestedAction = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    Severity = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionAlerts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductionAlerts_ProductionOrders_ProductionOrderId",
                        column: x => x.ProductionOrderId,
                        principalTable: "ProductionOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DailyProductionOutputs_ProductionOrderId",
                table: "DailyProductionOutputs",
                column: "ProductionOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionAlerts_ProductionOrderId",
                table: "ProductionAlerts",
                column: "ProductionOrderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailyProductionOutputs");

            migrationBuilder.DropTable(
                name: "ProductionAlerts");

            migrationBuilder.DropTable(
                name: "ProductionOrders");
        }
    }
}
