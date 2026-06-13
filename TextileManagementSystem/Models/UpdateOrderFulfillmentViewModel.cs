using System.ComponentModel.DataAnnotations;

namespace TextileManagementSystem.Models;

public class UpdateOrderFulfillmentViewModel
{
    public int Id { get; set; }

    public string OrderNumber { get; set; } = string.Empty;

    public string CustomerName { get; set; } = string.Empty;

    public string ProductName { get; set; } = string.Empty;

    public string ItemType { get; set; } = string.Empty;

    public decimal Quantity { get; set; }

    public string Unit { get; set; } = "Meters";

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

    [DataType(DataType.Date)]
    public DateTime DeliveryDate { get; set; }

    [StringLength(160)]
    public string? DocumentName { get; set; }

    [StringLength(80)]
    public string? CarrierName { get; set; }

    [StringLength(80)]
    public string? TrackingNumber { get; set; }

    [StringLength(600)]
    public string? InternalNotes { get; set; }
}
