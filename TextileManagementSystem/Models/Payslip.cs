using System.ComponentModel.DataAnnotations;

namespace TextileManagementSystem.Models;

public class Payslip
{
    public int Id { get; set; }

    [Required]
    public int EmployeeId { get; set; }

    public Employee? Employee { get; set; }

    [Required]
    [StringLength(30)]
    public string PayrollMonth { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal BasicPay { get; set; }

    [Range(0, double.MaxValue)]
    public decimal OvertimePay { get; set; }

    [Range(0, double.MaxValue)]
    public decimal Deductions { get; set; }

    [Range(0, double.MaxValue)]
    public decimal NetPayable { get; set; }

    [Required]
    [StringLength(30)]
    public string Status { get; set; } = "Ready";

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
