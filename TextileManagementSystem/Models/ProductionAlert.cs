using System.ComponentModel.DataAnnotations;

namespace TextileManagementSystem.Models;

public class ProductionAlert
{
    public int Id { get; set; }

    [Required]
    public int ProductionOrderId { get; set; }

    public ProductionOrder? ProductionOrder { get; set; }

    [Required]
    [StringLength(120)]
    public string ItemType { get; set; } = string.Empty;

    [Required]
    [StringLength(30)]
    public string DelayedStage { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    public DateTime TargetDate { get; set; } = DateTime.Today;

    [Required]
    [StringLength(120)]
    public string AssignedTeam { get; set; } = string.Empty;

    [Range(0, int.MaxValue)]
    public int DaysBehind { get; set; }

    [Required]
    [StringLength(160)]
    public string SuggestedAction { get; set; } = string.Empty;

    [Required]
    [StringLength(30)]
    public string Severity { get; set; } = "Warning";

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
