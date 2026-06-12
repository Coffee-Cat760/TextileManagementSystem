using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TextileManagementSystem.Models;

public class IssueStockViewModel
{
    [Required]
    [StringLength(60)]
    public string ProductionOrder { get; set; } = "PO-2024-0892";

    [Required]
    public int InventoryItemId { get; set; }

    [Range(0.01, double.MaxValue)]
    public decimal QuantityToIssue { get; set; }

    public string Unit { get; set; } = "Meters";

    [DataType(DataType.Date)]
    public DateTime IssueDate { get; set; } = DateTime.Today;

    [StringLength(500)]
    public string? Notes { get; set; }

    public decimal AvailableQuantity { get; set; }

    public string Category { get; set; } = string.Empty;

    public IReadOnlyList<SelectListItem> InventoryItems { get; set; } = [];
}
