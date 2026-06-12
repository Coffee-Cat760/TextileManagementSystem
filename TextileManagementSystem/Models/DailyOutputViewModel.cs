using Microsoft.AspNetCore.Mvc.Rendering;

namespace TextileManagementSystem.Models;

public class DailyOutputViewModel
{
    public DailyProductionOutput Output { get; set; } = new();

    public IReadOnlyList<SelectListItem> ProductionOrders { get; set; } = [];

    public ProductionOrder? SelectedOrder { get; set; }
}
