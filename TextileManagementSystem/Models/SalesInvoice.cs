using System.ComponentModel.DataAnnotations;

namespace TextileManagementSystem.Models;

public class SalesInvoice
{
    public int Id { get; set; }

    [Required]
    [StringLength(30)]
    public string InvoiceNumber { get; set; } = string.Empty;

    [Required]
    [StringLength(120)]
    public string CustomerName { get; set; } = string.Empty;

    [Required]
    [StringLength(220)]
    public string BillingAddress { get; set; } = string.Empty;

    [Required]
    [StringLength(10)]
    public string Currency { get; set; } = "USD";

    [DataType(DataType.Date)]
    public DateTime InvoiceDate { get; set; } = DateTime.Today;

    [DataType(DataType.Date)]
    public DateTime DueDate { get; set; } = DateTime.Today.AddDays(30);

    [Required]
    [StringLength(40)]
    public string PaymentTerms { get; set; } = "Net 30";

    [StringLength(40)]
    public string? PONumber { get; set; }

    [Required]
    [StringLength(20)]
    public string Status { get; set; } = "Draft";

    [Range(0, double.MaxValue)]
    public decimal Subtotal { get; set; }

    [Range(0, 100)]
    public decimal TaxRate { get; set; } = 18;

    [Range(0, double.MaxValue)]
    public decimal TaxAmount { get; set; }

    [Range(-1000, 1000)]
    public decimal RoundOff { get; set; }

    [Range(0, double.MaxValue)]
    public decimal GrandTotal { get; set; }

    [Range(0, double.MaxValue)]
    public decimal AmountReceived { get; set; }

    [StringLength(600)]
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime? UpdatedAt { get; set; }

    public ICollection<SalesInvoiceLine> LineItems { get; set; } = [];

    public decimal OutstandingAmount => Math.Max(0, GrandTotal - AmountReceived);

    public bool IsOverdue => !string.Equals(Status, "Paid", StringComparison.OrdinalIgnoreCase)
        && DueDate.Date < DateTime.Today;
}
