using System.ComponentModel.DataAnnotations;

namespace TextileManagementSystem.Models;

public class SalesInvoiceLine
{
    public int Id { get; set; }

    public int SalesInvoiceId { get; set; }

    public SalesInvoice? SalesInvoice { get; set; }

    [Required]
    [StringLength(160)]
    public string ProductDescription { get; set; } = string.Empty;

    [StringLength(20)]
    public string? HSNCode { get; set; }

    [Range(0.01, double.MaxValue)]
    public decimal Quantity { get; set; } = 1;

    [Required]
    [StringLength(20)]
    public string Unit { get; set; } = "Meters";

    [Range(0, double.MaxValue)]
    public decimal Rate { get; set; }

    [Range(0, 100)]
    public decimal DiscountPercent { get; set; }

    [Range(0, double.MaxValue)]
    public decimal LineTotal { get; set; }

    public int SortOrder { get; set; }
}
