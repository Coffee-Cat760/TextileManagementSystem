using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TextileManagementSystem.Data;
using TextileManagementSystem.Models;

namespace TextileManagementSystem.Controllers;

public class SalesController : Controller
{
    private readonly ApplicationDbContext _context;

    public SalesController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var invoices = await _context.SalesInvoices
            .Include(invoice => invoice.LineItems)
            .OrderByDescending(invoice => invoice.InvoiceDate)
            .ToListAsync();

        var monthStart = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        var netPayable = invoices
            .Where(invoice => !string.Equals(invoice.Status, "Paid", StringComparison.OrdinalIgnoreCase))
            .Sum(invoice => invoice.OutstandingAmount);

        var model = new SalesDashboardViewModel
        {
            Invoices = invoices,
            TotalReceivables = invoices.Sum(invoice => invoice.GrandTotal),
            MonthlySales = invoices
                .Where(invoice => invoice.InvoiceDate >= monthStart)
                .Sum(invoice => invoice.GrandTotal),
            OverdueReceivables = invoices
                .Where(invoice => invoice.IsOverdue || string.Equals(invoice.Status, "Overdue", StringComparison.OrdinalIgnoreCase))
                .Sum(invoice => invoice.OutstandingAmount),
            PendingInvoices = invoices.Count(invoice => !string.Equals(invoice.Status, "Paid", StringComparison.OrdinalIgnoreCase)),
            NetPayable = netPayable
        };

        return View(model);
    }

    public IActionResult CreateInvoice()
    {
        var model = new CreateSalesInvoiceViewModel
        {
            InvoiceNumber = $"INV-{DateTime.Now:yyyy}-{Random.Shared.Next(100, 999)}",
            InvoiceDate = DateTime.Today,
            DueDate = DateTime.Today.AddDays(30)
        };

        PopulateInvoiceLookups(model);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateInvoice(CreateSalesInvoiceViewModel model)
    {
        model.LineItems = model.LineItems
            .Where(line => !string.IsNullOrWhiteSpace(line.ProductDescription))
            .ToList();

        if (model.LineItems.Count == 0)
        {
            ModelState.AddModelError(nameof(model.LineItems), "Add at least one invoice line item.");
        }

        if (!ModelState.IsValid)
        {
            if (model.LineItems.Count == 0)
            {
                model.LineItems.Add(new CreateSalesInvoiceLineViewModel());
            }

            PopulateInvoiceLookups(model);
            return View(model);
        }

        var invoice = new SalesInvoice
        {
            InvoiceNumber = model.InvoiceNumber,
            CustomerName = model.CustomerName,
            BillingAddress = model.BillingAddress,
            Currency = model.Currency.Contains("USD", StringComparison.OrdinalIgnoreCase) ? "USD" : model.Currency,
            InvoiceDate = model.InvoiceDate,
            DueDate = model.DueDate,
            PaymentTerms = model.PaymentTerms,
            PONumber = model.PONumber,
            TaxRate = model.TaxRate,
            Notes = model.Notes,
            Status = string.Equals(model.SubmitAction, "Finalize", StringComparison.OrdinalIgnoreCase) ? "Sent" : "Draft",
            CreatedAt = DateTime.Now,
            LineItems = model.LineItems.Select((line, index) => new SalesInvoiceLine
            {
                ProductDescription = line.ProductDescription,
                HSNCode = line.HSNCode,
                Quantity = line.Quantity,
                Unit = line.Unit,
                Rate = line.Rate,
                DiscountPercent = line.DiscountPercent,
                SortOrder = index + 1
            }).ToList()
        };

        Recalculate(invoice);

        _context.SalesInvoices.Add(invoice);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id = invoice.Id });
    }

    public async Task<IActionResult> Details(int id)
    {
        var invoice = await _context.SalesInvoices
            .Include(item => item.LineItems.OrderBy(line => line.SortOrder))
            .FirstOrDefaultAsync(item => item.Id == id);

        return invoice is null ? NotFound() : View(invoice);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, string status)
    {
        var invoice = await _context.SalesInvoices.FindAsync(id);
        if (invoice is null)
        {
            return NotFound();
        }

        var normalized = status?.Trim();
        if (string.IsNullOrWhiteSpace(normalized))
        {
            return RedirectToAction(nameof(Index));
        }

        invoice.Status = normalized;
        invoice.UpdatedAt = DateTime.Now;

        if (string.Equals(normalized, "Paid", StringComparison.OrdinalIgnoreCase))
        {
            invoice.AmountReceived = invoice.GrandTotal;
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private static void Recalculate(SalesInvoice invoice)
    {
        var subtotal = invoice.LineItems.Sum(line =>
        {
            var gross = line.Quantity * line.Rate;
            var discount = gross * (line.DiscountPercent / 100m);
            line.LineTotal = Math.Round(gross - discount, 2);
            return line.LineTotal;
        });

        invoice.Subtotal = Math.Round(subtotal, 2);
        invoice.TaxAmount = Math.Round(invoice.Subtotal * (invoice.TaxRate / 100m), 2);

        var grossTotal = invoice.Subtotal + invoice.TaxAmount;
        var roundedTotal = Math.Round(grossTotal, 0, MidpointRounding.AwayFromZero);

        invoice.RoundOff = roundedTotal - grossTotal;
        invoice.GrandTotal = roundedTotal;
    }

    private void PopulateInvoiceLookups(CreateSalesInvoiceViewModel model)
    {
        var invoiceCustomers = _context.SalesInvoices
            .AsNoTracking()
            .Select(invoice => invoice.CustomerName);

        var supplierCustomers = _context.Suppliers
            .AsNoTracking()
            .Select(supplier => supplier.SupplierName);

        model.CustomerOptions = invoiceCustomers
            .Concat(supplierCustomers)
            .Where(name => name != "")
            .Distinct()
            .OrderBy(name => name)
            .Take(20)
            .ToList();

        model.BillingAddressOptions = _context.SalesInvoices
            .AsNoTracking()
            .Select(invoice => invoice.BillingAddress)
            .Where(address => address != "")
            .Distinct()
            .OrderBy(address => address)
            .Take(20)
            .ToList();
    }
}
