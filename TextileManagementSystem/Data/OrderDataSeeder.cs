using Microsoft.EntityFrameworkCore;
using TextileManagementSystem.Models;

namespace TextileManagementSystem.Data;

public static class OrderDataSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (await context.CustomerOrders.AnyAsync())
        {
            return;
        }

        var orders = new List<CustomerOrder>
        {
            new()
            {
                OrderNumber = "HFF-9001",
                CustomerName = "Nordic Linen Co.",
                ItemType = "Premium Cotton",
                ProductName = "Egyptian Cotton",
                Quantity = 2200,
                Unit = "Meters",
                UnitPrice = 38,
                DeliveryDate = DateTime.Today.AddDays(11),
                ProductionStage = "Cutting",
                ProductionProgress = 72,
                PaymentStatus = "Paid",
                OrderStatus = "In Production",
                DeliveryStatus = "On Track",
                ProductionOrderNumber = "PO-9824",
                Notes = "Priority export order with standard finishing.",
                CarrierName = "DHL Freight",
                TrackingNumber = "DHL-99301",
                CreatedAt = DateTime.Now.AddDays(-9)
            },
            new()
            {
                OrderNumber = "HFF-9002",
                CustomerName = "Atelier Silks",
                ItemType = "Brushed Twill",
                ProductName = "Silky Blend",
                Quantity = 500,
                Unit = "Sqm",
                UnitPrice = 64,
                DeliveryDate = DateTime.Today.AddDays(5),
                ProductionStage = "Dyeing",
                ProductionProgress = 48,
                PaymentStatus = "Pending",
                OrderStatus = "In Production",
                DeliveryStatus = "Delayed",
                ProductionOrderNumber = "PO-9826",
                InternalNotes = "Dyeing slot needs supervisor approval.",
                CreatedAt = DateTime.Now.AddDays(-12)
            },
            new()
            {
                OrderNumber = "HFF-8982",
                CustomerName = "Global Threads Ltd.",
                ItemType = "Linen Yarn",
                ProductName = "Organic Linen",
                Quantity = 12000,
                Unit = "Units",
                UnitPrice = 16,
                DeliveryDate = DateTime.Today.AddDays(8),
                ProductionStage = "Stitching",
                ProductionProgress = 88,
                PaymentStatus = "Partial",
                OrderStatus = "Quality Check",
                DeliveryStatus = "On Track",
                ProductionOrderNumber = "PO-9819",
                CreatedAt = DateTime.Now.AddDays(-16)
            },
            new()
            {
                OrderNumber = "HFF-8985",
                CustomerName = "Nicofic Home Designs",
                ItemType = "Synthetic Blend",
                ProductName = "Industrial Yarn",
                Quantity = 450,
                Unit = "Mtrs",
                UnitPrice = 42,
                DeliveryDate = DateTime.Today.AddDays(2),
                ProductionStage = "Cutting",
                ProductionProgress = 35,
                PaymentStatus = "Paid",
                OrderStatus = "Pending",
                DeliveryStatus = "On Track",
                CreatedAt = DateTime.Now.AddDays(-5)
            }
        };

        foreach (var order in orders)
        {
            order.TotalAmount = Math.Round(order.Quantity * order.UnitPrice, 2);
        }

        context.CustomerOrders.AddRange(orders);
        await context.SaveChangesAsync();
    }
}
