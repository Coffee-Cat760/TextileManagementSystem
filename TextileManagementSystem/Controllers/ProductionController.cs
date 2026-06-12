using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TextileManagementSystem.Data;
using TextileManagementSystem.Models;

namespace TextileManagementSystem.Controllers;

public class ProductionController : Controller
{
    private readonly ApplicationDbContext _context;

    public ProductionController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var model = await BuildDashboardViewModel();
        return View(model);
    }

    public IActionResult CreateOrder()
    {
        return View(new ProductionOrder
        {
            OrderNumber = $"ORD-{DateTime.Now:HHmmss}",
            TargetDate = DateTime.Today.AddDays(7),
            Status = "Active"
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateOrder(ProductionOrder order)
    {
        if (!ModelState.IsValid)
        {
            return View(order);
        }

        order.CreatedAt = DateTime.Now;
        _context.ProductionOrders.Add(order);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(StageTracker));
    }

    public async Task<IActionResult> EditOrder(int id)
    {
        var order = await _context.ProductionOrders.FindAsync(id);
        return order is null ? NotFound() : View(order);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditOrder(int id, ProductionOrder order)
    {
        if (id != order.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(order);
        }

        var existing = await _context.ProductionOrders.FindAsync(id);
        if (existing is null)
        {
            return NotFound();
        }

        existing.OrderNumber = order.OrderNumber;
        existing.ClientName = order.ClientName;
        existing.ItemName = order.ItemName;
        existing.Material = order.Material;
        existing.Quantity = order.Quantity;
        existing.CurrentStage = order.CurrentStage;
        existing.ProgressPercentage = order.ProgressPercentage;
        existing.TargetDate = order.TargetDate;
        existing.Status = order.Status;
        existing.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(StageTracker));
    }

    public async Task<IActionResult> DeleteOrder(int id)
    {
        var order = await _context.ProductionOrders.FindAsync(id);
        return order is null ? NotFound() : View(order);
    }

    [HttpPost, ActionName("DeleteOrder")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteOrderConfirmed(int id)
    {
        var order = await _context.ProductionOrders.FindAsync(id);
        if (order is null)
        {
            return NotFound();
        }

        _context.ProductionOrders.Remove(order);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(StageTracker));
    }

    public async Task<IActionResult> DailyOutput(int? productionOrderId)
    {
        var model = await BuildDailyOutputViewModel(new DailyProductionOutput
        {
            ProductionOrderId = productionOrderId ?? 0,
            OutputDate = DateTime.Today
        });

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DailyOutput(DailyProductionOutput output)
    {
        if (!ModelState.IsValid)
        {
            return View(await BuildDailyOutputViewModel(output));
        }

        output.CreatedAt = DateTime.Now;
        _context.DailyProductionOutputs.Add(output);

        var order = await _context.ProductionOrders.FindAsync(output.ProductionOrderId);
        if (order is not null)
        {
            order.CurrentStage = output.ProductionStage;
            order.ProgressPercentage = Math.Min(100, Math.Max(order.ProgressPercentage, output.QuantityCompleted * 100 / Math.Max(order.Quantity, 1)));
            order.UpdatedAt = DateTime.Now;
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> StageTracker()
    {
        var orders = await _context.ProductionOrders
            .OrderBy(order => order.TargetDate)
            .ToListAsync();

        return View(orders);
    }

    public async Task<IActionResult> Alerts()
    {
        var alerts = await _context.ProductionAlerts
            .Include(alert => alert.ProductionOrder)
            .OrderByDescending(alert => alert.DaysBehind)
            .ToListAsync();

        return View(alerts);
    }

    public async Task<IActionResult> CreateAlert()
    {
        ViewBag.ProductionOrders = await BuildOrderSelectList();
        return View(new ProductionAlert { TargetDate = DateTime.Today, Severity = "Warning" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateAlert(ProductionAlert alert)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.ProductionOrders = await BuildOrderSelectList(alert.ProductionOrderId);
            return View(alert);
        }

        alert.CreatedAt = DateTime.Now;
        _context.ProductionAlerts.Add(alert);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Alerts));
    }

    public async Task<IActionResult> EditAlert(int id)
    {
        var alert = await _context.ProductionAlerts.FindAsync(id);
        if (alert is null)
        {
            return NotFound();
        }

        ViewBag.ProductionOrders = await BuildOrderSelectList(alert.ProductionOrderId);
        return View(alert);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditAlert(int id, ProductionAlert alert)
    {
        if (id != alert.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            ViewBag.ProductionOrders = await BuildOrderSelectList(alert.ProductionOrderId);
            return View(alert);
        }

        var existing = await _context.ProductionAlerts.FindAsync(id);
        if (existing is null)
        {
            return NotFound();
        }

        existing.ProductionOrderId = alert.ProductionOrderId;
        existing.ItemType = alert.ItemType;
        existing.DelayedStage = alert.DelayedStage;
        existing.TargetDate = alert.TargetDate;
        existing.AssignedTeam = alert.AssignedTeam;
        existing.DaysBehind = alert.DaysBehind;
        existing.SuggestedAction = alert.SuggestedAction;
        existing.Severity = alert.Severity;

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Alerts));
    }

    public async Task<IActionResult> DeleteAlert(int id)
    {
        var alert = await _context.ProductionAlerts
            .Include(item => item.ProductionOrder)
            .FirstOrDefaultAsync(item => item.Id == id);

        return alert is null ? NotFound() : View(alert);
    }

    [HttpPost, ActionName("DeleteAlert")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAlertConfirmed(int id)
    {
        var alert = await _context.ProductionAlerts.FindAsync(id);
        if (alert is null)
        {
            return NotFound();
        }

        _context.ProductionAlerts.Remove(alert);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Alerts));
    }

    public async Task<IActionResult> Reports()
    {
        var model = await BuildDashboardViewModel();
        return View(model);
    }

    private async Task<ProductionDashboardViewModel> BuildDashboardViewModel()
    {
        var orders = await _context.ProductionOrders
            .OrderBy(order => order.TargetDate)
            .ToListAsync();
        var outputs = await _context.DailyProductionOutputs
            .Include(output => output.ProductionOrder)
            .OrderByDescending(output => output.OutputDate)
            .ToListAsync();
        var alerts = await _context.ProductionAlerts
            .Include(alert => alert.ProductionOrder)
            .OrderByDescending(alert => alert.DaysBehind)
            .ToListAsync();

        return new ProductionDashboardViewModel
        {
            Orders = orders,
            DailyOutputs = outputs,
            Alerts = alerts,
            TotalOutput = outputs.Sum(output => output.QuantityCompleted),
            ActiveOrders = orders.Count(order => order.Status == "Active"),
            BottleneckAlerts = alerts.Count(alert => alert.Severity == "Critical" || alert.DaysBehind > 0)
        };
    }

    private async Task<DailyOutputViewModel> BuildDailyOutputViewModel(DailyProductionOutput output)
    {
        var orders = await _context.ProductionOrders
            .OrderBy(order => order.OrderNumber)
            .ToListAsync();

        var selectedOrderId = output.ProductionOrderId != 0 ? output.ProductionOrderId : orders.FirstOrDefault()?.Id ?? 0;

        output.ProductionOrderId = selectedOrderId;

        return new DailyOutputViewModel
        {
            Output = output,
            SelectedOrder = orders.FirstOrDefault(order => order.Id == selectedOrderId),
            ProductionOrders = orders
                .Select(order => new SelectListItem($"{order.OrderNumber} ({order.ItemName})", order.Id.ToString(), order.Id == selectedOrderId))
                .ToList()
        };
    }

    private async Task<IReadOnlyList<SelectListItem>> BuildOrderSelectList(int selectedId = 0)
    {
        return await _context.ProductionOrders
            .OrderBy(order => order.OrderNumber)
            .Select(order => new SelectListItem($"{order.OrderNumber} - {order.ItemName}", order.Id.ToString(), order.Id == selectedId))
            .ToListAsync();
    }
}
