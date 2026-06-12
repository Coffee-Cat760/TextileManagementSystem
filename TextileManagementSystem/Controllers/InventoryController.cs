using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TextileManagementSystem.Data;
using TextileManagementSystem.Models;

namespace TextileManagementSystem.Controllers;

public class InventoryController : Controller
{
    private readonly ApplicationDbContext _context;

    public InventoryController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var items = await _context.InventoryItems
            .OrderBy(item => item.MaterialName)
            .ToListAsync();

        var model = new InventoryDashboardViewModel
        {
            Items = items,
            StockValuation = items.Sum(item => item.Quantity * item.UnitPrice),
            LowStockAlerts = items.Count(item => item.Status == "Low Stock" || item.Status == "Out of Stock"),
            ActiveSuppliers = items.Select(item => item.Supplier).Distinct().Count()
        };

        return View(model);
    }

    public async Task<IActionResult> Details(int id)
    {
        var item = await _context.InventoryItems.FindAsync(id);
        if (item is null)
        {
            return NotFound();
        }

        ViewBag.StockIssues = await _context.StockIssues
            .Where(issue => issue.InventoryItemId == id)
            .OrderByDescending(issue => issue.IssueDate)
            .ToListAsync();

        return View(item);
    }

    public IActionResult AddStock()
    {
        return View(new InventoryItem { ReceiveDate = DateTime.Today });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddStock(InventoryItem item)
    {
        if (!ModelState.IsValid)
        {
            return View(item);
        }

        item.RefreshStatus();
        item.CreatedAt = DateTime.Now;

        _context.InventoryItems.Add(item);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var item = await _context.InventoryItems.FindAsync(id);
        if (item is null)
        {
            return NotFound();
        }

        return View(item);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, InventoryItem item)
    {
        if (id != item.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(item);
        }

        var existingItem = await _context.InventoryItems.FindAsync(id);
        if (existingItem is null)
        {
            return NotFound();
        }

        existingItem.MaterialName = item.MaterialName;
        existingItem.SKU = item.SKU;
        existingItem.Category = item.Category;
        existingItem.ColorVariant = item.ColorVariant;
        existingItem.Quantity = item.Quantity;
        existingItem.Unit = item.Unit;
        existingItem.Supplier = item.Supplier;
        existingItem.Threshold = item.Threshold;
        existingItem.UnitPrice = item.UnitPrice;
        existingItem.StorageLocation = item.StorageLocation;
        existingItem.ReceiveDate = item.ReceiveDate;
        existingItem.Notes = item.Notes;
        existingItem.UpdatedAt = DateTime.Now;
        existingItem.RefreshStatus();

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var item = await _context.InventoryItems.FindAsync(id);
        if (item is null)
        {
            return NotFound();
        }

        return View(item);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var item = await _context.InventoryItems.FindAsync(id);
        if (item is null)
        {
            return NotFound();
        }

        _context.InventoryItems.Remove(item);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> IssueStock()
    {
        var model = await BuildIssueStockViewModel();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> IssueStock(IssueStockViewModel model)
    {
        var item = await _context.InventoryItems.FindAsync(model.InventoryItemId);
        if (item is null)
        {
            ModelState.AddModelError(nameof(model.InventoryItemId), "Please select a valid material.");
        }
        else if (model.QuantityToIssue > item.Quantity)
        {
            ModelState.AddModelError(nameof(model.QuantityToIssue), "Issue quantity cannot be greater than available stock.");
        }

        if (!ModelState.IsValid)
        {
            var invalidModel = await BuildIssueStockViewModel(model);
            return View(invalidModel);
        }

        item ??= await _context.InventoryItems.FindAsync(model.InventoryItemId);
        if (item is null)
        {
            var invalidModel = await BuildIssueStockViewModel(model);
            ModelState.AddModelError(nameof(model.InventoryItemId), "Please select a valid material.");
            return View(invalidModel);
        }

        item.Quantity -= model.QuantityToIssue;
        item.UpdatedAt = DateTime.Now;
        item.RefreshStatus();

        _context.StockIssues.Add(new StockIssue
        {
            ProductionOrder = model.ProductionOrder,
            InventoryItemId = model.InventoryItemId,
            QuantityToIssue = model.QuantityToIssue,
            Unit = item.Unit,
            IssueDate = model.IssueDate,
            Notes = model.Notes,
            CreatedAt = DateTime.Now
        });

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    private async Task<IssueStockViewModel> BuildIssueStockViewModel(IssueStockViewModel? model = null)
    {
        var items = await _context.InventoryItems
            .OrderBy(item => item.MaterialName)
            .ToListAsync();

        var selected = items.FirstOrDefault(item => item.Id == model?.InventoryItemId) ?? items.FirstOrDefault();

        return new IssueStockViewModel
        {
            ProductionOrder = model?.ProductionOrder ?? "PO-2024-0892",
            InventoryItemId = model?.InventoryItemId ?? selected?.Id ?? 0,
            QuantityToIssue = model?.QuantityToIssue ?? 0,
            Unit = selected?.Unit ?? "Meters",
            IssueDate = model?.IssueDate == default ? DateTime.Today : model?.IssueDate ?? DateTime.Today,
            Notes = model?.Notes,
            AvailableQuantity = selected?.Quantity ?? 0,
            Category = selected?.Category ?? string.Empty,
            InventoryItems = items
                .Select(item => new SelectListItem(
                    $"{item.MaterialName} - {item.ColorVariant}",
                    item.Id.ToString(),
                    item.Id == (model?.InventoryItemId ?? selected?.Id)))
                .ToList()
        };
    }
}
