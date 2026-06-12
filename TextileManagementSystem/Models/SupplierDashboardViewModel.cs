namespace TextileManagementSystem.Models;

public class SupplierDashboardViewModel
{
    public IReadOnlyList<Supplier> Suppliers { get; set; } = [];

    public int TotalSuppliers { get; set; }

    public int ActivePartners { get; set; }

    public int PendingPayments { get; set; }

    public decimal TotalOutstanding { get; set; }
}
