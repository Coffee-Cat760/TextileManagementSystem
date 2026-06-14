using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TextileManagementSystem.Data;
using TextileManagementSystem.Models;

namespace TextileManagementSystem.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: /Dashboard/Index
    public async Task<IActionResult> Index()
    {
        // ── Summary Cards ────────────────────────────────────────────────

        ViewBag.TotalOrders = await _context.CustomerOrders.CountAsync();

        // DailyProductionOutput: field = OutputDate, QuantityCompleted (int), Unit
        ViewBag.TodayProduction = await _context.DailyProductionOutputs
            .Where(d => d.OutputDate.Date == DateTime.Today)
            .SumAsync(d => (int?)d.QuantityCompleted) ?? 0;

        // InventoryItem: Quantity, Threshold — both decimal ✓
        ViewBag.LowStockAlerts = await _context.InventoryItems
            .CountAsync(i => i.Quantity <= i.Threshold);

        // SalesInvoice: InvoiceDate, GrandTotal ✓  |  Status (not PaymentStatus)
        ViewBag.MonthlyRevenue = await _context.SalesInvoices
            .Where(s => s.InvoiceDate.Month == DateTime.Now.Month
                     && s.InvoiceDate.Year  == DateTime.Now.Year)
            .SumAsync(s => (decimal?)s.GrandTotal) ?? 0m;

        // "Pending" or "Draft" invoices count as pending payments
        ViewBag.PendingPayments = await _context.SalesInvoices
            .CountAsync(s => s.Status == "Pending" || s.Status == "Draft");

        // Employee: Status == "Active"  (no IsActive bool — uses string Status)
        ViewBag.ActiveWorkers = await _context.Employees
            .CountAsync(e => e.Status == "Active");

        // ── Production Analytics — last 7 days bar chart ─────────────────
        var last7Days = Enumerable.Range(0, 7)
            .Select(i => DateTime.Today.AddDays(-6 + i))
            .ToList();

        var dailyOutputs = await _context.DailyProductionOutputs
            .Where(d => d.OutputDate.Date >= DateTime.Today.AddDays(-6))
            .ToListAsync();

        var productionSeries = last7Days.Select(day => new
        {
            Label = day.ToString("ddd"),
            Qty   = dailyOutputs
                        .Where(d => d.OutputDate.Date == day.Date)
                        .Sum(d => d.QuantityCompleted)
        }).ToList();

        ViewBag.ProductionLabels = productionSeries.Select(x => x.Label).ToList();
        ViewBag.ProductionData   = productionSeries.Select(x => x.Qty).ToList();

        // ── Order Fulfillment donut ──────────────────────────────────────
        ViewBag.CompletedOrders  = await _context.CustomerOrders
            .CountAsync(o => o.OrderStatus == "Completed");
        ViewBag.InProgressOrders = await _context.CustomerOrders
            .CountAsync(o => o.OrderStatus == "In Production");

        // ── Recent Orders (latest 5) ─────────────────────────────────────
        ViewBag.RecentOrders = await _context.CustomerOrders
            .OrderByDescending(o => o.CreatedAt)
            .Take(5)
            .ToListAsync();

        // ── Low Stock Alerts — InventoryItem.MaterialName, Quantity, Threshold, Unit
        ViewBag.LowStockItems = await _context.InventoryItems
            .Where(i => i.Quantity <= i.Threshold)
            .OrderBy(i => i.Quantity)
            .Take(3)
            .ToListAsync();

        // ── Upcoming Deliveries (next 5 by delivery date) ─────────────────
        ViewBag.UpcomingDeliveries = await _context.CustomerOrders
            .Where(o => o.DeliveryDate.Date >= DateTime.Today
                     && o.OrderStatus != "Completed")
            .OrderBy(o => o.DeliveryDate)
            .Take(5)
            .ToListAsync();

        return View();
    }
}