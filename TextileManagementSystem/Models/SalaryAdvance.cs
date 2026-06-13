using System.ComponentModel.DataAnnotations;

namespace TextileManagementSystem.Models;

public class SalaryAdvance
{
    public int Id { get; set; }

    [Required]
    public int EmployeeId { get; set; }

    public Employee? Employee { get; set; }

    [Required]
    [StringLength(40)]
    public string RequestCode { get; set; } = string.Empty;

    [Range(1, double.MaxValue)]
    public decimal Amount { get; set; }

    [DataType(DataType.Date)]
    public DateTime RequestedDate { get; set; } = DateTime.Today;

    [Required]
    [StringLength(120)]
    public string Reason { get; set; } = string.Empty;

    [Range(1, 36)]
    public int RepaymentMonths { get; set; } = 3;

    [Required]
    [StringLength(30)]
    public string Status { get; set; } = "Pending";

    [StringLength(500)]
    public string? Notes { get; set; }

    [StringLength(260)]
    public string? SupportingDocumentPath { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime? UpdatedAt { get; set; }
}
