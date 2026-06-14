using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using TextileManagementSystem.Data;

#nullable disable

namespace TextileManagementSystem.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260613001000_AddOrderManagementModule")]
    public partial class AddOrderManagementModule : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustomerOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderNumber = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    ItemType = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(140)", maxLength: 140, nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProductionStage = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    ProductionProgress = table.Column<int>(type: "int", nullable: false),
                    PaymentStatus = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    OrderStatus = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    DeliveryStatus = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ProductionOrderNumber = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    LinkedProductionOrderId = table.Column<int>(type: "int", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(600)", maxLength: 600, nullable: true),
                    InternalNotes = table.Column<string>(type: "nvarchar(600)", maxLength: 600, nullable: true),
                    DocumentName = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: true),
                    CarrierName = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    TrackingNumber = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerOrders_ProductionOrders_LinkedProductionOrderId",
                        column: x => x.LinkedProductionOrderId,
                        principalTable: "ProductionOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerOrders_LinkedProductionOrderId",
                table: "CustomerOrders",
                column: "LinkedProductionOrderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "CustomerOrders");
        }
    }
}
