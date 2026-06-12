using Microsoft.EntityFrameworkCore;
using TextileManagementSystem.Models;

namespace TextileManagementSystem.Data;

public static class HRDataSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (await context.Employees.AnyAsync())
        {
            return;
        }

        var employees = new List<Employee>
        {
            new() { EmployeeCode = "FF-1025", FullName = "Fatima Zahra", CNIC = "42101-1234567-8", DateOfBirth = new DateTime(1994, 4, 18), Gender = "Female", MobileNumber = "+92 300 1234567", Email = "fatima.z@fabricflow.com", Department = "Weaving", Designation = "Senior Machine Operator", Role = "Senior Operator", JoiningDate = new DateTime(2021, 10, 12), PayType = "Monthly Fixed", BasicSalary = 1250, BonusAllowance = 120, PaymentMode = "Bank Transfer", BankName = "HBL", AccountNumber = "PK00-HBL-1025", Status = "Active", AttendancePercentage = 98, OvertimeHours = 24.5m, Address = "Street 4, Industrial Area, Faisalabad" },
            new() { EmployeeCode = "FF-2032", FullName = "Ahmed Hassan", CNIC = "35201-9876543-2", DateOfBirth = new DateTime(1990, 9, 3), Gender = "Male", MobileNumber = "+92 301 4567890", Email = "ahmed.h@fabricflow.com", Department = "Stitching", Designation = "Line Supervisor", Role = "Supervisor", JoiningDate = new DateTime(2022, 6, 2), PayType = "Monthly Salary", BasicSalary = 1100, BonusAllowance = 85, PaymentMode = "Bank Transfer", Status = "On Leave", AttendancePercentage = 82, OvertimeHours = 12 },
            new() { EmployeeCode = "FF-1091", FullName = "Zainab Bibi", CNIC = "61101-4455667-3", DateOfBirth = new DateTime(1992, 12, 11), Gender = "Female", MobileNumber = "+92 302 5556677", Email = "zainab.b@fabricflow.com", Department = "Quality Control", Designation = "QC Auditor", Role = "Auditor", JoiningDate = new DateTime(2020, 11, 30), PayType = "Piece Rate", BasicSalary = 1480, BonusAllowance = 150, PaymentMode = "Bank Transfer", Status = "Active", AttendancePercentage = 100, OvertimeHours = 35.2m },
            new() { EmployeeCode = "FF-2041", FullName = "Amara Smith", CNIC = "12201-1122334-4", DateOfBirth = new DateTime(1996, 2, 21), Gender = "Female", MobileNumber = "+92 303 9988776", Email = "amara.s@fabricflow.com", Department = "Logistics", Designation = "Warehouse Staff", Role = "Staff", JoiningDate = new DateTime(2023, 3, 5), PayType = "Daily Rate", BasicSalary = 980, BonusAllowance = 30, PaymentMode = "Cash", Status = "Active", AttendancePercentage = 94, OvertimeHours = 8 }
        };

        context.Employees.AddRange(employees);
        await context.SaveChangesAsync();

        context.AttendanceRecords.AddRange(
            new AttendanceRecord { EmployeeId = employees[0].Id, AttendanceDate = DateTime.Today, Shift = "Morning", Status = "Present", ClockIn = new TimeSpan(8, 55, 0), ClockOut = new TimeSpan(18, 0, 0) },
            new AttendanceRecord { EmployeeId = employees[1].Id, AttendanceDate = DateTime.Today, Shift = "Morning", Status = "Leave" },
            new AttendanceRecord { EmployeeId = employees[2].Id, AttendanceDate = DateTime.Today, Shift = "Morning", Status = "Present", ClockIn = new TimeSpan(9, 5, 0), ClockOut = new TimeSpan(18, 0, 0), Note = "Late arrival approved by supervisor." },
            new AttendanceRecord { EmployeeId = employees[3].Id, AttendanceDate = DateTime.Today, Shift = "Day", Status = "Absent", Note = "No check-in recorded." });

        context.SalaryAdvances.AddRange(
            new SalaryAdvance { EmployeeId = employees[0].Id, RequestCode = "ADV-2023-089", Amount = 1200, RequestedDate = DateTime.Today.AddDays(-3), Reason = "Medical Emergency", RepaymentMonths = 3, Status = "Pending" },
            new SalaryAdvance { EmployeeId = employees[2].Id, RequestCode = "ADV-2023-085", Amount = 850, RequestedDate = DateTime.Today.AddDays(-5), Reason = "Home Repair", RepaymentMonths = 4, Status = "Approved" },
            new SalaryAdvance { EmployeeId = employees[1].Id, RequestCode = "ADV-2023-082", Amount = 2500, RequestedDate = DateTime.Today.AddDays(-7), Reason = "Vehicle Installment", RepaymentMonths = 6, Status = "Rejected" });

        context.Payslips.AddRange(
            new Payslip { EmployeeId = employees[0].Id, PayrollMonth = "November 2023", BasicPay = 1250, OvertimePay = 142, Deductions = 45, NetPayable = 1347, Status = "Ready" },
            new Payslip { EmployeeId = employees[1].Id, PayrollMonth = "November 2023", BasicPay = 1100, OvertimePay = 65, Deductions = 30, NetPayable = 1135, Status = "Generated" },
            new Payslip { EmployeeId = employees[2].Id, PayrollMonth = "November 2023", BasicPay = 1480, OvertimePay = 210, Deductions = 60, NetPayable = 1630, Status = "Ready" });

        await context.SaveChangesAsync();
    }
}
