using Microsoft.EntityFrameworkCore;
using TextileManagementSystem.Models;

namespace TextileManagementSystem.Data;

public static class ProductionDataSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (await context.ProductionOrders.AnyAsync())
        {
            return;
        }

        var orders = new List<ProductionOrder>
        {
            new() { OrderNumber = "ORD-90231", ClientName = "VelvetVogue Co.", ItemName = "Cotton Twill Shirts", Material = "Silk Crimson", Quantity = 5000, CurrentStage = "Stitching", ProgressPercentage = 38, TargetDate = DateTime.Today.AddDays(6), Status = "Active" },
            new() { OrderNumber = "ORD-90234", ClientName = "Modern Textiles", ItemName = "Linen Jackets", Material = "Linen Ash", Quantity = 2800, CurrentStage = "Cutting", ProgressPercentage = 25, TargetDate = DateTime.Today.AddDays(2), Status = "Delayed" },
            new() { OrderNumber = "ORD-89912", ClientName = "Blueberry Kids", ItemName = "Silk Blouses", Material = "Cotton Sky", Quantity = 1200, CurrentStage = "Finishing", ProgressPercentage = 70, TargetDate = DateTime.Today.AddDays(8), Status = "Active" },
            new() { OrderNumber = "ORD-90110", ClientName = "Urban Wear Ltd.", ItemName = "Linen Trousers", Material = "Denim Midnight", Quantity = 3500, CurrentStage = "Quality Check", ProgressPercentage = 90, TargetDate = DateTime.Today.AddDays(-1), Status = "Delayed" }
        };

        context.ProductionOrders.AddRange(orders);
        await context.SaveChangesAsync();

        context.DailyProductionOutputs.AddRange(
            new DailyProductionOutput { ProductionOrderId = orders[0].Id, ProductionStage = "Stitching", AssignedWorkers = "Julian Chan, Sarah Miller", QuantityCompleted = 3250, Unit = "Meters", OutputDate = DateTime.Today, Notes = "Shift A completed without defects." },
            new DailyProductionOutput { ProductionOrderId = orders[2].Id, ProductionStage = "Finishing", AssignedWorkers = "Marcus Chen", QuantityCompleted = 1270, Unit = "Units", OutputDate = DateTime.Today.AddDays(-1), Notes = "Finishing line operating normally." });

        context.ProductionAlerts.AddRange(
            new ProductionAlert { ProductionOrderId = orders[1].Id, ItemType = "Denim Jackets", DelayedStage = "Cutting", TargetDate = DateTime.Today.AddDays(2), AssignedTeam = "Cutting Team A", DaysBehind = 3, SuggestedAction = "Reassign Worker", Severity = "Critical" },
            new ProductionAlert { ProductionOrderId = orders[0].Id, ItemType = "Cotton Shirts", DelayedStage = "Stitching", TargetDate = DateTime.Today.AddDays(6), AssignedTeam = "Stitching Team B", DaysBehind = 1, SuggestedAction = "Check QC Queue", Severity = "Warning" },
            new ProductionAlert { ProductionOrderId = orders[3].Id, ItemType = "Silk Ties", DelayedStage = "Packaging", TargetDate = DateTime.Today.AddDays(1), AssignedTeam = "Packaging Team", DaysBehind = 0, SuggestedAction = "Awaiting Labels", Severity = "Info" });

        await context.SaveChangesAsync();
    }
}
