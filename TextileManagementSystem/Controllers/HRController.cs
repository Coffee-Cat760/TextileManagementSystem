using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TextileManagementSystem.Data;
using TextileManagementSystem.Models;

namespace TextileManagementSystem.Controllers;

public class HRController : Controller
{
    private readonly ApplicationDbContext _context;

    public HRController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        return View(await BuildDashboardViewModel());
    }

    public async Task<IActionResult> Directory()
    {
        var employees = await _context.Employees
            .OrderBy(employee => employee.FullName)
            .ToListAsync();

        return View(employees);
    }

    public IActionResult AddEmployee()
    {
        return View(new Employee
        {
            EmployeeCode = $"FF-{DateTime.Now:yyyy-HHmm}",
            JoiningDate = DateTime.Today,
            Status = "Active"
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddEmployee(Employee employee)
    {
        if (!ModelState.IsValid)
        {
            return View(employee);
        }

        employee.CreatedAt = DateTime.Now;
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Directory));
    }

    public async Task<IActionResult> Profile(int? id)
    {
        var employee = id.HasValue
            ? await _context.Employees.FindAsync(id.Value)
            : await _context.Employees.OrderBy(employee => employee.FullName).FirstOrDefaultAsync();

        if (employee is null)
        {
            return RedirectToAction(nameof(AddEmployee));
        }

        ViewBag.Payslips = await _context.Payslips
            .Where(payslip => payslip.EmployeeId == employee.Id)
            .OrderByDescending(payslip => payslip.CreatedAt)
            .ToListAsync();

        ViewBag.Advances = await _context.SalaryAdvances
            .Where(advance => advance.EmployeeId == employee.Id)
            .OrderByDescending(advance => advance.RequestedDate)
            .ToListAsync();

        return View(employee);
    }

    public async Task<IActionResult> EditEmployee(int id)
    {
        var employee = await _context.Employees.FindAsync(id);
        return employee is null ? NotFound() : View(employee);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditEmployee(int id, Employee employee)
    {
        if (id != employee.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(employee);
        }

        var existing = await _context.Employees.FindAsync(id);
        if (existing is null)
        {
            return NotFound();
        }

        existing.EmployeeCode = employee.EmployeeCode;
        existing.FullName = employee.FullName;
        existing.CNIC = employee.CNIC;
        existing.DateOfBirth = employee.DateOfBirth;
        existing.Gender = employee.Gender;
        existing.MobileNumber = employee.MobileNumber;
        existing.Email = employee.Email;
        existing.Department = employee.Department;
        existing.Designation = employee.Designation;
        existing.Role = employee.Role;
        existing.JoiningDate = employee.JoiningDate;
        existing.PayType = employee.PayType;
        existing.BasicSalary = employee.BasicSalary;
        existing.BonusAllowance = employee.BonusAllowance;
        existing.PaymentMode = employee.PaymentMode;
        existing.BankName = employee.BankName;
        existing.AccountNumber = employee.AccountNumber;
        existing.Status = employee.Status;
        existing.AttendancePercentage = employee.AttendancePercentage;
        existing.OvertimeHours = employee.OvertimeHours;
        existing.Address = employee.Address;
        existing.Notes = employee.Notes;
        existing.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Profile), new { id });
    }

    public async Task<IActionResult> DeleteEmployee(int id)
    {
        var employee = await _context.Employees.FindAsync(id);
        return employee is null ? NotFound() : View(employee);
    }

    [HttpPost, ActionName("DeleteEmployee")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteEmployeeConfirmed(int id)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee is null)
        {
            return NotFound();
        }

        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Directory));
    }

    public async Task<IActionResult> Attendance()
    {
        return View(await BuildDashboardViewModel());
    }

    public async Task<IActionResult> Payslips()
    {
        var payslips = await _context.Payslips
            .Include(payslip => payslip.Employee)
            .OrderBy(payslip => payslip.Employee!.FullName)
            .ToListAsync();

        return View(payslips);
    }

    public async Task<IActionResult> Advances()
    {
        var advances = await _context.SalaryAdvances
            .Include(advance => advance.Employee)
            .OrderByDescending(advance => advance.RequestedDate)
            .ToListAsync();

        return View(advances);
    }

    public async Task<IActionResult> NewAdvance()
    {
        ViewBag.Employees = await BuildEmployeeSelectList();
        return View(new SalaryAdvance
        {
            RequestCode = $"ADV-{DateTime.Now:yyyy-HHmm}",
            RequestedDate = DateTime.Today,
            Status = "Pending",
            RepaymentMonths = 3
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> NewAdvance(SalaryAdvance advance)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Employees = await BuildEmployeeSelectList(advance.EmployeeId);
            return View(advance);
        }

        advance.CreatedAt = DateTime.Now;
        _context.SalaryAdvances.Add(advance);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Advances));
    }

    public async Task<IActionResult> EditAdvance(int id)
    {
        var advance = await _context.SalaryAdvances.FindAsync(id);
        if (advance is null)
        {
            return NotFound();
        }

        ViewBag.Employees = await BuildEmployeeSelectList(advance.EmployeeId);
        return View(advance);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditAdvance(int id, SalaryAdvance advance)
    {
        if (id != advance.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            ViewBag.Employees = await BuildEmployeeSelectList(advance.EmployeeId);
            return View(advance);
        }

        var existing = await _context.SalaryAdvances.FindAsync(id);
        if (existing is null)
        {
            return NotFound();
        }

        existing.EmployeeId = advance.EmployeeId;
        existing.RequestCode = advance.RequestCode;
        existing.Amount = advance.Amount;
        existing.RequestedDate = advance.RequestedDate;
        existing.Reason = advance.Reason;
        existing.RepaymentMonths = advance.RepaymentMonths;
        existing.Status = advance.Status;
        existing.Notes = advance.Notes;
        existing.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Advances));
    }

    public async Task<IActionResult> DeleteAdvance(int id)
    {
        var advance = await _context.SalaryAdvances
            .Include(item => item.Employee)
            .FirstOrDefaultAsync(item => item.Id == id);

        return advance is null ? NotFound() : View(advance);
    }

    [HttpPost, ActionName("DeleteAdvance")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAdvanceConfirmed(int id)
    {
        var advance = await _context.SalaryAdvances.FindAsync(id);
        if (advance is null)
        {
            return NotFound();
        }

        _context.SalaryAdvances.Remove(advance);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Advances));
    }

    public async Task<IActionResult> Loans()
    {
        var advances = await _context.SalaryAdvances
            .Include(advance => advance.Employee)
            .OrderByDescending(advance => advance.RequestedDate)
            .ToListAsync();

        return View(advances);
    }

    public async Task<IActionResult> AttendanceTracking()
    {
        var records = await _context.AttendanceRecords
            .Include(record => record.Employee)
            .OrderByDescending(record => record.AttendanceDate)
            .ThenBy(record => record.Employee!.FullName)
            .ToListAsync();

        return View(records);
    }

    public async Task<IActionResult> MarkAttendance()
    {
        var employees = await _context.Employees
            .OrderBy(employee => employee.FullName)
            .ToListAsync();

        return View(new MarkAttendanceViewModel
        {
            Entries = employees.Select(employee => new AttendanceEntryViewModel
            {
                EmployeeId = employee.Id,
                EmployeeName = employee.FullName,
                EmployeeCode = employee.EmployeeCode,
                Department = employee.Department,
                Status = "Present",
                ClockIn = new TimeSpan(9, 0, 0),
                ClockOut = new TimeSpan(18, 0, 0)
            }).ToList()
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkAttendance(MarkAttendanceViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var employeeIds = model.Entries.Select(entry => entry.EmployeeId).ToList();
        var existingRecords = await _context.AttendanceRecords
            .Where(record => record.AttendanceDate.Date == model.AttendanceDate.Date && employeeIds.Contains(record.EmployeeId))
            .ToListAsync();

        foreach (var entry in model.Entries)
        {
            var record = existingRecords.FirstOrDefault(item => item.EmployeeId == entry.EmployeeId);
            if (record is null)
            {
                _context.AttendanceRecords.Add(new AttendanceRecord
                {
                    EmployeeId = entry.EmployeeId,
                    AttendanceDate = model.AttendanceDate,
                    Shift = model.Shift,
                    Status = entry.Status,
                    ClockIn = entry.ClockIn,
                    ClockOut = entry.ClockOut,
                    Note = entry.Note,
                    CreatedAt = DateTime.Now
                });
            }
            else
            {
                record.Shift = model.Shift;
                record.Status = entry.Status;
                record.ClockIn = entry.ClockIn;
                record.ClockOut = entry.ClockOut;
                record.Note = entry.Note;
            }
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(AttendanceTracking));
    }

    private async Task<HRDashboardViewModel> BuildDashboardViewModel()
    {
        var employees = await _context.Employees.OrderBy(employee => employee.FullName).ToListAsync();
        var attendance = await _context.AttendanceRecords.Include(record => record.Employee).OrderByDescending(record => record.AttendanceDate).ToListAsync();
        var advances = await _context.SalaryAdvances.Include(advance => advance.Employee).OrderByDescending(advance => advance.RequestedDate).ToListAsync();
        var payslips = await _context.Payslips.Include(payslip => payslip.Employee).OrderByDescending(payslip => payslip.CreatedAt).ToListAsync();

        return new HRDashboardViewModel
        {
            Employees = employees,
            AttendanceRecords = attendance,
            SalaryAdvances = advances,
            Payslips = payslips,
            TotalPayroll = employees.Sum(employee => employee.BasicSalary + employee.BonusAllowance),
            TotalOvertimeHours = employees.Sum(employee => employee.OvertimeHours),
            SalaryAdvanceTotal = advances.Where(advance => advance.Status != "Rejected").Sum(advance => advance.Amount),
            NetPayable = payslips.Sum(payslip => payslip.NetPayable)
        };
    }

    private async Task<IReadOnlyList<SelectListItem>> BuildEmployeeSelectList(int selectedId = 0)
    {
        return await _context.Employees
            .OrderBy(employee => employee.FullName)
            .Select(employee => new SelectListItem($"{employee.FullName} ({employee.EmployeeCode})", employee.Id.ToString(), employee.Id == selectedId))
            .ToListAsync();
    }
}
