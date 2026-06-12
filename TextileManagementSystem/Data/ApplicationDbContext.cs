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
    }
}
