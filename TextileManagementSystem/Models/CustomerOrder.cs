using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TextileManagementSystem.Models;

public class CustomerOrder
{
    public int Id { get; set; }

    [Required]
    [StringLength(40)]
    public string OrderNumber { get; set; } = string.Empty;

    [Required]
    [StringLength(120)]
    public string CustomerName { get; set; } = string.Empty;

    [Required]
    [StringLength(120)]
    public string ItemType { get; set; } = string.Empty;

    [Required]
    [StringLength(140)]
    public string ProductName { get; set; } = string.Empty;

    [Range(1, double.MaxValue)]
    public decimal Quantity { get; set; }

    [Required]
    [StringLength(20)]
    public string Unit { get; set; } = "Meters";

    [Range(0, double.MaxValue)]
    public decimal UnitPrice { get; set; }

    [Range(0, double.MaxValue)]
    public decimal TotalAmount { get; set; }

    [DataType(DataType.Date)]
    public DateTime DeliveryDate { get; set; } = DateTime.Today.AddDays(14);

    [Required]
    [StringLength(40)]
    public string ProductionStage { get; set; } = "Cutting";

    [Range(0, 100)]
    public int ProductionProgress { get; set; }

    [Required]
    [StringLength(30)]
    public string PaymentStatus { get; set; } = "Pending";

    [Required]
    [StringLength(30)]
    public string OrderStatus { get; set; } = "Pending";

    [Required]
    [StringLength(30)]
    public string DeliveryStatus { get; set; } = "On Track";

    [StringLength(40)]
    public string? ProductionOrderNumber { get; set; }

    public int? LinkedProductionOrderId { get; set; }

    public ProductionOrder? LinkedProductionOrder { get; set; }

    [StringLength(600)]
    public string? Notes { get; set; }

    [StringLength(600)]
    public string? InternalNotes { get; set; }

    [StringLength(160)]
    public string? DocumentName { get; set; }

    [StringLength(80)]
    public string? CarrierName { get; set; }

    [StringLength(80)]
    public string? TrackingNumber { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime? UpdatedAt { get; set; }

    [NotMapped]
    public bool IsDelayed => !string.Equals(OrderStatus, "Completed", StringComparison.OrdinalIgnoreCase)
        && DeliveryDate.Date < DateTime.Today;

    [NotMapped]
    public int LeadTimeDays => Math.Max(1, (DeliveryDate.Date - CreatedAt.Date).Days);
}
