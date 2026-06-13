namespace TextileManagementSystem.Models;

public class OrderDashboardViewModel
{
    public IReadOnlyList<CustomerOrder> Orders { get; set; } = [];

    public int TotalActiveOrders { get; set; }

    public int PendingShipments { get; set; }

    public int SKUsInProduction { get; set; }

    public decimal RevenueInMotion { get; set; }

    public decimal AverageLeadTime { get; set; }

    public int CriticalBottlenecks { get; set; }

    public int DailyTargetCompletion { get; set; }
}
