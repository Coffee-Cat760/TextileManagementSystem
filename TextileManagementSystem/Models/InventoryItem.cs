using System.ComponentModel.DataAnnotations;

namespace TextileManagementSystem.Models;

public class InventoryItem
{
    public int Id { get; set; }

    [Required]
    [StringLength(120)]
    public string MaterialName { get; set; } = string.Empty;

    [Required]
    [StringLength(40)]
    public string SKU { get; set; } = string.Empty;

    [Required]
    [StringLength(60)]
    public string Category { get; set; } = string.Empty;

    [Required]
    [StringLength(80)]
    public string ColorVariant { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal Quantity { get; set; }

    [Required]
    [StringLength(20)]
    public string Unit { get; set; } = "Meters";

    [Required]
    [StringLength(120)]
    public string Supplier { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal Threshold { get; set; }

    [Required]
    [StringLength(30)]
    public string Status { get; set; } = "In Stock";

    [Range(0, double.MaxValue)]
    public decimal UnitPrice { get; set; }

    [Required]
    [StringLength(120)]
    public string StorageLocation { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    public DateTime ReceiveDate { get; set; } = DateTime.Today;

    [StringLength(500)]
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime? UpdatedAt { get; set; }

    public void RefreshStatus()
    {
        Status = Quantity <= 0 ? "Out of Stock" : Quantity <= Threshold ? "Low Stock" : "In Stock";
    }
}
