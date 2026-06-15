using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TextileManagementSystem.Data;
using TextileManagementSystem.Models;

namespace TextileManagementSystem.Controllers;

public class OrdersController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public OrdersController(ApplicationDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    public async Task<IActionResult> Index()
    {
        var orders = await LoadOrdersAsync();
        return View(BuildDashboard(orders));
    }

    public async Task<IActionResult> Fulfillment()
    {
        var orders = await LoadOrdersAsync();
        return View(BuildDashboard(orders));
    }

    public IActionResult Create()
    {
        return View(new CustomerOrder
        {
            OrderNumber = $"HFF-{Random.Shared.Next(9000, 9999)}",
            DeliveryDate = DateTime.Today.AddDays(14),
            Quantity = 0,
            UnitPrice = 0,
            PaymentStatus = "Pending",
            OrderStatus = "Pending",
            DeliveryStatus = "On Track"
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CustomerOrder model, string productionLink, IFormFile? documentFile)
    {
        if (await _context.CustomerOrders.AnyAsync(order => order.OrderNumber == model.OrderNumber))
        {
            ModelState.AddModelError(nameof(model.OrderNumber), "This order number already exists.");
        }

        var documentPath = await SaveUploadAsync(documentFile, "customer-orders", ["application/pdf", "image/jpeg", "image/png"]);

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        model.TotalAmount = Math.Round(model.Quantity * model.UnitPrice, 2);
        model.CreatedAt = DateTime.Now;
        model.DocumentName = documentPath;

        if (string.Equals(productionLink, "Create New", StringComparison.OrdinalIgnoreCase))
        {
            var productionOrder = new ProductionOrder
            {
                OrderNumber = $"PO-{Random.Shared.Next(9800, 9999)}",
                ClientName = model.CustomerName,
                ItemName = model.ProductName,
                Material = model.ItemType,
                Quantity = (int)Math.Ceiling(model.Quantity),
                CurrentStage = model.ProductionStage,
                ProgressPercentage = model.ProductionProgress,
                TargetDate = model.DeliveryDate,
                Status = "Active",
                CreatedAt = DateTime.Now
            };

            _context.ProductionOrders.Add(productionOrder);
            model.LinkedProductionOrder = productionOrder;
            model.ProductionOrderNumber = productionOrder.OrderNumber;
        }

        _context.CustomerOrders.Add(model);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Production(int id)
    {
        var order = await _context.CustomerOrders
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == id);

        return order is null ? NotFound() : View(ToUpdateModel(order));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Production(UpdateOrderFulfillmentViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var order = await _context.CustomerOrders
            .Include(item => item.LinkedProductionOrder)
            .FirstOrDefaultAsync(item => item.Id == model.Id);

        if (order is null)
        {
            return NotFound();
        }

        ApplyFulfillment(order, model);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Fulfillment));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, string? orderStatus, string? paymentStatus, string? deliveryStatus)
    {
        var order = await _context.CustomerOrders.FindAsync(id);
        if (order is null)
        {
            return NotFound();
        }

        if (!string.IsNullOrWhiteSpace(orderStatus))
        {
            order.OrderStatus = orderStatus.Trim();
        }

        if (!string.IsNullOrWhiteSpace(paymentStatus))
        {
            order.PaymentStatus = paymentStatus.Trim();
        }

        if (!string.IsNullOrWhiteSpace(deliveryStatus))
        {
            order.DeliveryStatus = deliveryStatus.Trim();
        }

        order.UpdatedAt = DateTime.Now;
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    private async Task<List<CustomerOrder>> LoadOrdersAsync()
    {
        return await _context.CustomerOrders
            .Include(order => order.LinkedProductionOrder)
            .OrderBy(order => order.DeliveryDate)
            .ThenByDescending(order => order.CreatedAt)
            .ToListAsync();
    }

    private static OrderDashboardViewModel BuildDashboard(IReadOnlyList<CustomerOrder> orders)
    {
        var activeOrders = orders.Where(order => !string.Equals(order.OrderStatus, "Completed", StringComparison.OrdinalIgnoreCase)).ToList();

        return new OrderDashboardViewModel
        {
            Orders = orders,
            TotalActiveOrders = activeOrders.Count,
            PendingShipments = orders.Count(order => !string.Equals(order.DeliveryStatus, "Shipped", StringComparison.OrdinalIgnoreCase)),
            SKUsInProduction = orders.Select(order => order.ItemType).Distinct(StringComparer.OrdinalIgnoreCase).Count(),
            RevenueInMotion = activeOrders.Sum(order => order.TotalAmount),
            AverageLeadTime = orders.Count == 0 ? 0 : Math.Round((decimal)orders.Average(order => order.LeadTimeDays), 1),
            CriticalBottlenecks = orders.Count(order => order.IsDelayed || order.ProductionProgress < 50),
            DailyTargetCompletion = orders.Count == 0 ? 0 : (int)Math.Round(orders.Average(order => order.ProductionProgress))
        };
    }

    private static UpdateOrderFulfillmentViewModel ToUpdateModel(CustomerOrder order)
    {
        return new UpdateOrderFulfillmentViewModel
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            CustomerName = order.CustomerName,
            ProductName = order.ProductName,
            ItemType = order.ItemType,
            Quantity = order.Quantity,
            Unit = order.Unit,
            ProductionStage = order.ProductionStage,
            ProductionProgress = order.ProductionProgress,
            PaymentStatus = order.PaymentStatus,
            OrderStatus = order.OrderStatus,
            DeliveryStatus = order.DeliveryStatus,
            DeliveryDate = order.DeliveryDate,
            DocumentName = order.DocumentName,
            CarrierName = order.CarrierName,
            TrackingNumber = order.TrackingNumber,
            InternalNotes = order.InternalNotes
        };
    }

    private static void ApplyFulfillment(CustomerOrder order, UpdateOrderFulfillmentViewModel model)
    {
        order.ProductionStage = model.ProductionStage;
        order.ProductionProgress = model.ProductionProgress;
        order.PaymentStatus = model.PaymentStatus;
        order.OrderStatus = model.OrderStatus;
        order.DeliveryStatus = model.DeliveryStatus;
        order.DeliveryDate = model.DeliveryDate;
        order.DocumentName = model.DocumentName;
        order.CarrierName = model.CarrierName;
        order.TrackingNumber = model.TrackingNumber;
        order.InternalNotes = model.InternalNotes;
        order.UpdatedAt = DateTime.Now;

        if (order.LinkedProductionOrder is not null)
        {
            order.LinkedProductionOrder.CurrentStage = model.ProductionStage;
            order.LinkedProductionOrder.ProgressPercentage = model.ProductionProgress;
            order.LinkedProductionOrder.TargetDate = model.DeliveryDate;
            order.LinkedProductionOrder.Status = model.OrderStatus == "Completed" ? "Completed" : "Active";
            order.LinkedProductionOrder.UpdatedAt = DateTime.Now;
        }
    }

    private async Task<string?> SaveUploadAsync(IFormFile? file, string folderName, string[] allowedContentTypes)
    {
        if (file is null || file.Length == 0)
        {
            return null;
        }

        if (!allowedContentTypes.Contains(file.ContentType) || file.Length > 10 * 1024 * 1024)
        {
            ModelState.AddModelError(string.Empty, "Upload must be a PDF, JPG, or PNG file smaller than 10MB.");
            return null;
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        var safeFileName = $"{Guid.NewGuid():N}{extension}";
        var uploadRoot = Path.Combine(_environment.WebRootPath, "uploads", "orders", folderName);
        System.IO.Directory.CreateDirectory(uploadRoot);

        var physicalPath = Path.Combine(uploadRoot, safeFileName);
        await using var stream = System.IO.File.Create(physicalPath);
        await file.CopyToAsync(stream);

        return $"/uploads/orders/{folderName}/{safeFileName}";
    }
}
