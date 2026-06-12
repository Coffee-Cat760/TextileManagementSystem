using System.ComponentModel.DataAnnotations;

namespace TextileManagementSystem.Models;

public class AttendanceRecord
{
    public int Id { get; set; }

    [Required]
    public int EmployeeId { get; set; }

    public Employee? Employee { get; set; }

    [DataType(DataType.Date)]
    public DateTime AttendanceDate { get; set; } = DateTime.Today;

    [Required]
    [StringLength(30)]
    public string Shift { get; set; } = "Morning";

    [Required]
    [StringLength(20)]
    public string Status { get; set; } = "Present";

    [DataType(DataType.Time)]
    public TimeSpan? ClockIn { get; set; }

    [DataType(DataType.Time)]
    public TimeSpan? ClockOut { get; set; }

    [StringLength(300)]
    public string? Note { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
