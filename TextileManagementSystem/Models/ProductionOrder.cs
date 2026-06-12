using System.ComponentModel.DataAnnotations;

namespace TextileManagementSystem.Models;

public class ProductionOrder
{
    public int Id { get; set; }

    [Required]
    [StringLength(40)]
    public string OrderNumber { get; set; } = string.Empty;

    [Required]
    [StringLength(120)]
    public string ClientName { get; set; } = string.Empty;

    [Required]
    [StringLength(120)]
    public string ItemName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Material { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    [Required]
    [StringLength(30)]
    public string CurrentStage { get; set; } = "Cutting";

    [Range(0, 100)]
    public int ProgressPercentage { get; set; }

    [DataType(DataType.Date)]
    public DateTime TargetDate { get; set; } = DateTime.Today.AddDays(7);

    [Required]
    [StringLength(30)]
    public string Status { get; set; } = "Active";

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime? UpdatedAt { get; set; }
}
