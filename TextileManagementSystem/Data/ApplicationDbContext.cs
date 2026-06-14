using Microsoft.EntityFrameworkCore;
using TextileManagementSystem.Models;

namespace TextileManagementSystem.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();

    public DbSet<StockIssue> StockIssues => Set<StockIssue>();

    public DbSet<Supplier> Suppliers => Set<Supplier>();

    public DbSet<ProductionOrder> ProductionOrders => Set<ProductionOrder>();

    public DbSet<DailyProductionOutput> DailyProductionOutputs => Set<DailyProductionOutput>();

    public DbSet<ProductionAlert> ProductionAlerts => Set<ProductionAlert>();

    public DbSet<Employee> Employees => Set<Employee>();

    public DbSet<AttendanceRecord> AttendanceRecords => Set<AttendanceRecord>();

    public DbSet<SalaryAdvance> SalaryAdvances => Set<SalaryAdvance>();

    public DbSet<Payslip> Payslips => Set<Payslip>();

    public DbSet<SalesInvoice> SalesInvoices => Set<SalesInvoice>();

    public DbSet<SalesInvoiceLine> SalesInvoiceLines => Set<SalesInvoiceLine>();

    public DbSet<CustomerOrder> CustomerOrders => Set<CustomerOrder>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<InventoryItem>(entity =>
        {
            entity.Property(item => item.Quantity).HasPrecision(18, 2);
            entity.Property(item => item.Threshold).HasPrecision(18, 2);
            entity.Property(item => item.UnitPrice).HasPrecision(18, 2);
        });

        modelBuilder.Entity<StockIssue>(entity =>
        {
            entity.Property(issue => issue.QuantityToIssue).HasPrecision(18, 2);
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.Property(supplier => supplier.OutstandingBalance).HasPrecision(18, 2);
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.Property(employee => employee.BasicSalary).HasPrecision(18, 2);
            entity.Property(employee => employee.BonusAllowance).HasPrecision(18, 2);
            entity.Property(employee => employee.AttendancePercentage).HasPrecision(5, 2);
            entity.Property(employee => employee.OvertimeHours).HasPrecision(8, 2);
        });

        modelBuilder.Entity<SalaryAdvance>(entity =>
        {
            entity.Property(advance => advance.Amount).HasPrecision(18, 2);
        });

        modelBuilder.Entity<Payslip>(entity =>
        {
            entity.Property(payslip => payslip.BasicPay).HasPrecision(18, 2);
            entity.Property(payslip => payslip.OvertimePay).HasPrecision(18, 2);
            entity.Property(payslip => payslip.Deductions).HasPrecision(18, 2);
            entity.Property(payslip => payslip.NetPayable).HasPrecision(18, 2);
        });

        modelBuilder.Entity<SalesInvoice>(entity =>
        {
            entity.Property(invoice => invoice.Subtotal).HasPrecision(18, 2);
            entity.Property(invoice => invoice.TaxRate).HasPrecision(5, 2);
            entity.Property(invoice => invoice.TaxAmount).HasPrecision(18, 2);
            entity.Property(invoice => invoice.RoundOff).HasPrecision(18, 2);
            entity.Property(invoice => invoice.GrandTotal).HasPrecision(18, 2);
            entity.Property(invoice => invoice.AmountReceived).HasPrecision(18, 2);
        });

        modelBuilder.Entity<SalesInvoiceLine>(entity =>
        {
            entity.Property(line => line.Quantity).HasPrecision(18, 2);
            entity.Property(line => line.Rate).HasPrecision(18, 2);
            entity.Property(line => line.DiscountPercent).HasPrecision(5, 2);
            entity.Property(line => line.LineTotal).HasPrecision(18, 2);

            entity.HasOne(line => line.SalesInvoice)
                .WithMany(invoice => invoice.LineItems)
                .HasForeignKey(line => line.SalesInvoiceId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CustomerOrder>(entity =>
        {
            entity.Property(order => order.Quantity).HasPrecision(18, 2);
            entity.Property(order => order.UnitPrice).HasPrecision(18, 2);
            entity.Property(order => order.TotalAmount).HasPrecision(18, 2);

            entity.HasOne(order => order.LinkedProductionOrder)
                .WithMany()
                .HasForeignKey(order => order.LinkedProductionOrderId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
