namespace TextileManagementSystem.Models;

public class MarkAttendanceViewModel
{
    public DateTime AttendanceDate { get; set; } = DateTime.Today;

    public string Department { get; set; } = "All Departments";

    public string Shift { get; set; } = "Morning";

    public List<AttendanceEntryViewModel> Entries { get; set; } = [];
}

public class AttendanceEntryViewModel
{
    public int EmployeeId { get; set; }

    public string EmployeeName { get; set; } = string.Empty;

    public string EmployeeCode { get; set; } = string.Empty;

    public string Department { get; set; } = string.Empty;

    public string Status { get; set; } = "Present";

    public TimeSpan? ClockIn { get; set; } = new(9, 0, 0);

    public TimeSpan? ClockOut { get; set; } = new(18, 0, 0);

    public string? Note { get; set; }
}
