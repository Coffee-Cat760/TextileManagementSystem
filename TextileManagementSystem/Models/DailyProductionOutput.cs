using System.ComponentModel.DataAnnotations;

namespace TextileManagementSystem.Models;

public class DailyProductionOutput
{
    public int Id { get; set; }

    [Required]
    public int ProductionOrderId { get; set; }

    public ProductionOrder? ProductionOrder { get; set; }

    [Required]
    [StringLength(30)]
    public string ProductionStage { get; set; } = "Cutting";

    [Required]
    [StringLength(180)]
    public string AssignedWorkers { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int QuantityCompleted { get; set; }

    [Required]
    [StringLength(20)]
    public string Unit { get; set; } = "Meters";

    [DataType(DataType.Date)]
    public DateTime OutputDate { get; set; } = DateTime.Today;

    [StringLength(500)]
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
