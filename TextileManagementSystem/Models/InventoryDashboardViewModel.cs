namespace TextileManagementSystem.Models;

public class InventoryDashboardViewModel
{
    public IReadOnlyList<InventoryItem> Items { get; set; } = [];

    public decimal StockValuation { get; set; }

    public int LowStockAlerts { get; set; }

    public int ActiveSuppliers { get; set; }
}
