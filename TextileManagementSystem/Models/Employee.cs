using System.ComponentModel.DataAnnotations;

namespace TextileManagementSystem.Models;

public class Employee
{
    public int Id { get; set; }

    [Required]
    [StringLength(40)]
    public string EmployeeCode { get; set; } = string.Empty;

    [Required]
    [StringLength(120)]
    public string FullName { get; set; } = string.Empty;

    [StringLength(20)]
    public string? CNIC { get; set; }

    [DataType(DataType.Date)]
    public DateTime? DateOfBirth { get; set; }

    [Required]
    [StringLength(20)]
    public string Gender { get; set; } = "Female";

    [Required]
    [StringLength(30)]
    public string MobileNumber { get; set; } = string.Empty;

    [StringLength(120)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(80)]
    public string Department { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Designation { get; set; } = string.Empty;

    [Required]
    [StringLength(80)]
    public string Role { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    public DateTime JoiningDate { get; set; } = DateTime.Today;

    [Required]
    [StringLength(30)]
    public string PayType { get; set; } = "Monthly Salary";

    [Range(0, double.MaxValue)]
    public decimal BasicSalary { get; set; }

    [Range(0, double.MaxValue)]
    public decimal BonusAllowance { get; set; }

    [Required]
    [StringLength(40)]
    public string PaymentMode { get; set; } = "Bank Transfer";

    [StringLength(80)]
    public string? BankName { get; set; }

    [StringLength(80)]
    public string? AccountNumber { get; set; }

    [Required]
    [StringLength(30)]
    public string Status { get; set; } = "Active";

    [Range(0, 100)]
    public decimal AttendancePercentage { get; set; } = 100;

    [Range(0, double.MaxValue)]
    public decimal OvertimeHours { get; set; }

    [StringLength(250)]
    public string? Address { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }

    [StringLength(260)]
    public string? PhotoPath { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime? UpdatedAt { get; set; }
}
