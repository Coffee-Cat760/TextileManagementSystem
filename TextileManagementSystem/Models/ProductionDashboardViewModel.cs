namespace TextileManagementSystem.Models;

public class ProductionDashboardViewModel
{
    public IReadOnlyList<ProductionOrder> Orders { get; set; } = [];

    public IReadOnlyList<DailyProductionOutput> DailyOutputs { get; set; } = [];

    public IReadOnlyList<ProductionAlert> Alerts { get; set; } = [];

    public int TotalOutput { get; set; }

    public int ActiveOrders { get; set; }

    public double QualityPassRate { get; set; } = 99.4;

    public int BottleneckAlerts { get; set; }
}
