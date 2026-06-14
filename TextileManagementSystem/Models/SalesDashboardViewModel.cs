namespace TextileManagementSystem.Models;

public class SalesDashboardViewModel
{
    public IReadOnlyList<SalesInvoice> Invoices { get; set; } = [];

    public decimal TotalReceivables { get; set; }

    public decimal MonthlySales { get; set; }

    public decimal OverdueReceivables { get; set; }

    public int PendingInvoices { get; set; }

    public decimal NetPayable { get; set; }
}
