using System.ComponentModel.DataAnnotations;

namespace TextileManagementSystem.Models;

public class CreateSalesInvoiceViewModel
{
    [Required]
    [StringLength(30)]
    public string InvoiceNumber { get; set; } = string.Empty;

    [Required]
    [StringLength(10)]
    public string Currency { get; set; } = "USD ($)";

    [Required]
    [StringLength(120)]
    public string CustomerName { get; set; } = string.Empty;

    [Required]
    [StringLength(220)]
    public string BillingAddress { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    public DateTime InvoiceDate { get; set; } = DateTime.Today;

    [DataType(DataType.Date)]
    public DateTime DueDate { get; set; } = DateTime.Today.AddDays(30);

    [Required]
    [StringLength(40)]
    public string PaymentTerms { get; set; } = "Net 30";

    [StringLength(40)]
    public string? PONumber { get; set; }

    [Range(0, 100)]
    public decimal TaxRate { get; set; } = 18;

    [StringLength(600)]
    public string? Notes { get; set; }

    public List<CreateSalesInvoiceLineViewModel> LineItems { get; set; } =
    [
        new CreateSalesInvoiceLineViewModel()
    ];

    public string SubmitAction { get; set; } = "Draft";

    public IReadOnlyList<string> CustomerOptions { get; set; } = [];

    public IReadOnlyList<string> BillingAddressOptions { get; set; } = [];
}

public class CreateSalesInvoiceLineViewModel
{
    [Required]
    [StringLength(160)]
    public string ProductDescription { get; set; } = string.Empty;

    [StringLength(20)]
    public string? HSNCode { get; set; }

    [Range(0.01, double.MaxValue)]
    public decimal Quantity { get; set; } = 1200;

    [Required]
    [StringLength(20)]
    public string Unit { get; set; } = "Meters";

    [Range(0, double.MaxValue)]
    public decimal Rate { get; set; } = 12.50m;

    [Range(0, 100)]
    public decimal DiscountPercent { get; set; } = 5;
}
