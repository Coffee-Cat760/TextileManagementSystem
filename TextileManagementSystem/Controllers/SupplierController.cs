using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TextileManagementSystem.Data;
using TextileManagementSystem.Models;

namespace TextileManagementSystem.Controllers;

public class SupplierController : Controller
{
    private readonly ApplicationDbContext _context;

    public SupplierController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var suppliers = await _context.Suppliers
            .OrderBy(supplier => supplier.SupplierName)
            .ToListAsync();

        var model = new SupplierDashboardViewModel
        {
            Suppliers = suppliers,
            TotalSuppliers = suppliers.Count,
            ActivePartners = suppliers.Count(supplier => supplier.Status == "Active"),
            PendingPayments = suppliers.Count(supplier => supplier.OutstandingBalance > 0),
            TotalOutstanding = suppliers.Sum(supplier => supplier.OutstandingBalance)
        };

        return View(model);
    }

    public async Task<IActionResult> Details(int id)
    {
        var supplier = await _context.Suppliers.FindAsync(id);
        if (supplier is null)
        {
            return NotFound();
        }

        return View(supplier);
    }

    public IActionResult Add()
    {
        return View(new Supplier
        {
            SupplierCode = $"S-{DateTime.Now:HHmm}",
            Status = "Active"
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(Supplier supplier)
    {
        if (!ModelState.IsValid)
        {
            return View(supplier);
        }

        supplier.CreatedAt = DateTime.Now;
        _context.Suppliers.Add(supplier);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var supplier = await _context.Suppliers.FindAsync(id);
        if (supplier is null)
        {
            return NotFound();
        }

        return View(supplier);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Supplier supplier)
    {
        if (id != supplier.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(supplier);
        }

        var existingSupplier = await _context.Suppliers.FindAsync(id);
        if (existingSupplier is null)
        {
            return NotFound();
        }

        existingSupplier.SupplierName = supplier.SupplierName;
        existingSupplier.SupplierCode = supplier.SupplierCode;
        existingSupplier.ContactPerson = supplier.ContactPerson;
        existingSupplier.PhoneNumber = supplier.PhoneNumber;
        existingSupplier.EmailAddress = supplier.EmailAddress;
        existingSupplier.BankAccount = supplier.BankAccount;
        existingSupplier.Status = supplier.Status;
        existingSupplier.MaterialSpecialization = supplier.MaterialSpecialization;
        existingSupplier.OrdersCount = supplier.OrdersCount;
        existingSupplier.OutstandingBalance = supplier.OutstandingBalance;
        existingSupplier.Notes = supplier.Notes;
        existingSupplier.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var supplier = await _context.Suppliers.FindAsync(id);
        if (supplier is null)
        {
            return NotFound();
        }

        return View(supplier);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var supplier = await _context.Suppliers.FindAsync(id);
        if (supplier is null)
        {
            return NotFound();
        }

        _context.Suppliers.Remove(supplier);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> PurchaseOrders()
    {
        var suppliers = await _context.Suppliers
            .OrderBy(supplier => supplier.SupplierName)
            .ToListAsync();

        ViewBag.SupplierLinks = suppliers.ToDictionary(supplier => supplier.SupplierName, supplier => supplier.Id);
        ViewBag.FallbackSupplierId = suppliers.FirstOrDefault()?.Id;

        return View();
    }
}
