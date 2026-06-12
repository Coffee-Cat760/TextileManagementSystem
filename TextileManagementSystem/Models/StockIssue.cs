using System.ComponentModel.DataAnnotations;

namespace TextileManagementSystem.Models;

public class StockIssue
{
    public int Id { get; set; }

    [Required]
    [StringLength(60)]
    public string ProductionOrder { get; set; } = string.Empty;

    [Required]
    public int InventoryItemId { get; set; }

    public InventoryItem? InventoryItem { get; set; }

    [Range(0.01, double.MaxValue)]
    public decimal QuantityToIssue { get; set; }

    [Required]
    [StringLength(20)]
    public string Unit { get; set; } = "Meters";

    [DataType(DataType.Date)]
    public DateTime IssueDate { get; set; } = DateTime.Today;

    [StringLength(500)]
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
