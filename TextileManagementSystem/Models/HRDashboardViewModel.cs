namespace TextileManagementSystem.Models;

public class HRDashboardViewModel
{
    public IReadOnlyList<Employee> Employees { get; set; } = [];

    public IReadOnlyList<AttendanceRecord> AttendanceRecords { get; set; } = [];

    public IReadOnlyList<SalaryAdvance> SalaryAdvances { get; set; } = [];

    public IReadOnlyList<Payslip> Payslips { get; set; } = [];

    public decimal TotalPayroll { get; set; }

    public decimal TotalOvertimeHours { get; set; }

    public decimal SalaryAdvanceTotal { get; set; }

    public decimal NetPayable { get; set; }
}
