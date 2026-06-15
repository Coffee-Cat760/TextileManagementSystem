using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text;
using TextileManagementSystem.Data;
using TextileManagementSystem.Models;

namespace TextileManagementSystem.Controllers;

public class HRController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public HRController(ApplicationDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
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
    public async Task<IActionResult> AddEmployee(Employee employee, IFormFile? photoFile)
    {
        if (!ModelState.IsValid)
        {
            return View(employee);
        }

        employee.PhotoPath = await SaveUploadAsync(photoFile, "employees", ["image/jpeg", "image/png", "image/webp"]);
        if (!ModelState.IsValid)
        {
            return View(employee);
        }

        employee.CreatedAt = DateTime.Now;
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();
        await CreateOrUpdatePayslipAsync(employee.Id, GetCurrentPayrollMonth());

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
    public async Task<IActionResult> EditEmployee(int id, Employee employee, IFormFile? photoFile)
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
        var uploadedPhotoPath = await SaveUploadAsync(photoFile, "employees", ["image/jpeg", "image/png", "image/webp"]);
        if (!ModelState.IsValid)
        {
            return View(employee);
        }

        existing.PhotoPath = uploadedPhotoPath ?? existing.PhotoPath;
        existing.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();
        await CreateOrUpdatePayslipAsync(existing.Id, GetCurrentPayrollMonth());
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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GeneratePayslips(string payrollMonth)
    {
        payrollMonth = string.IsNullOrWhiteSpace(payrollMonth)
            ? GetCurrentPayrollMonth()
            : payrollMonth;

        var employees = await _context.Employees
            .OrderBy(employee => employee.FullName)
            .ToListAsync();

        foreach (var employee in employees)
        {
            await CreateOrUpdatePayslipAsync(employee.Id, payrollMonth);
        }

        return RedirectToAction(nameof(Payslips), new { month = payrollMonth });
    }

    public async Task<IActionResult> DownloadPayslip(int id)
    {
        var payslip = await _context.Payslips
            .Include(item => item.Employee)
            .FirstOrDefaultAsync(item => item.Id == id);

        if (payslip is null || payslip.Employee is null)
        {
            return NotFound();
        }

        var pdf = GeneratePayslipPdf(payslip);
        var fileName = $"{payslip.Employee.EmployeeCode}-{payslip.PayrollMonth.Replace(" ", "-")}-Payslip.pdf";

        return File(pdf, "application/pdf", fileName);
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
    public async Task<IActionResult> NewAdvance(SalaryAdvance advance, IFormFile? documentFile)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Employees = await BuildEmployeeSelectList(advance.EmployeeId);
            return View(advance);
        }

        advance.SupportingDocumentPath = await SaveUploadAsync(documentFile, "advances", ["application/pdf", "image/jpeg", "image/png"]);
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
    public async Task<IActionResult> EditAdvance(int id, SalaryAdvance advance, IFormFile? documentFile)
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
        var uploadedDocumentPath = await SaveUploadAsync(documentFile, "advances", ["application/pdf", "image/jpeg", "image/png"]);
        if (!ModelState.IsValid)
        {
            ViewBag.Employees = await BuildEmployeeSelectList(advance.EmployeeId);
            return View(advance);
        }

        existing.SupportingDocumentPath = uploadedDocumentPath ?? existing.SupportingDocumentPath;
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

    public async Task<IActionResult> AttendanceTracking(string period = "today")
    {
        period = period.ToLowerInvariant();
        var today = DateTime.Today;
        var startDate = period switch
        {
            "weekly" => today.AddDays(-6),
            "monthly" => new DateTime(today.Year, today.Month, 1),
            _ => today
        };
        var endDate = period == "today" ? today : today.AddDays(1);

        var records = await _context.AttendanceRecords
            .Include(record => record.Employee)
            .Where(record => record.AttendanceDate.Date >= startDate && record.AttendanceDate.Date <= endDate)
            .OrderByDescending(record => record.AttendanceDate)
            .ThenBy(record => record.Employee!.FullName)
            .ToListAsync();

        ViewBag.Period = period;
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

    private async Task CreateOrUpdatePayslipAsync(int employeeId, string payrollMonth)
    {
        var employee = await _context.Employees.FindAsync(employeeId);
        if (employee is null)
        {
            return;
        }

        var approvedAdvances = await _context.SalaryAdvances
            .Where(advance => advance.EmployeeId == employeeId && advance.Status == "Approved")
            .ToListAsync();

        var monthlyAdvanceDeduction = approvedAdvances.Sum(advance =>
            advance.RepaymentMonths <= 0 ? 0 : advance.Amount / advance.RepaymentMonths);
        var overtimePay = Math.Round(employee.OvertimeHours * 6, 2);
        var deductions = Math.Round(monthlyAdvanceDeduction, 2);
        var netPayable = Math.Max(0, employee.BasicSalary + employee.BonusAllowance + overtimePay - deductions);

        var payslip = await _context.Payslips
            .FirstOrDefaultAsync(item => item.EmployeeId == employeeId && item.PayrollMonth == payrollMonth);

        if (payslip is null)
        {
            _context.Payslips.Add(new Payslip
            {
                EmployeeId = employeeId,
                PayrollMonth = payrollMonth,
                BasicPay = employee.BasicSalary,
                OvertimePay = overtimePay,
                Deductions = deductions,
                NetPayable = netPayable,
                Status = "Generated",
                CreatedAt = DateTime.Now
            });
        }
        else
        {
            payslip.BasicPay = employee.BasicSalary;
            payslip.OvertimePay = overtimePay;
            payslip.Deductions = deductions;
            payslip.NetPayable = netPayable;
            payslip.Status = "Generated";
        }

        await _context.SaveChangesAsync();
    }

    private static string GetCurrentPayrollMonth()
    {
        return DateTime.Today.ToString("MMMM yyyy");
    }

    private static byte[] GeneratePayslipPdf(Payslip payslip)
    {
        var employee = payslip.Employee!;
        var lines = new[]
        {
            "FabricFlow ERP - Employee Payslip",
            $"Payroll Month: {payslip.PayrollMonth}",
            $"Generated: {DateTime.Now:MMM dd, yyyy hh:mm tt}",
            "",
            $"Employee: {employee.FullName}",
            $"Employee ID: {employee.EmployeeCode}",
            $"Department: {employee.Department}",
            $"Designation: {employee.Designation}",
            $"Pay Type: {employee.PayType}",
            "",
            $"Basic Pay: {payslip.BasicPay:C}",
            $"Bonus / Allowance: {employee.BonusAllowance:C}",
            $"Overtime Pay: {payslip.OvertimePay:C}",
            $"Deductions: {payslip.Deductions:C}",
            "----------------------------------------",
            $"Net Payable: {payslip.NetPayable:C}",
            $"Status: {payslip.Status}",
            "",
            "This is a system generated payslip."
        };

        var content = new StringBuilder();
        content.AppendLine("BT");
        content.AppendLine("/F1 18 Tf");
        content.AppendLine("50 780 Td");
        content.AppendLine($"({EscapePdf(lines[0])}) Tj");
        content.AppendLine("/F1 11 Tf");
        content.AppendLine("0 -28 Td");

        foreach (var line in lines.Skip(1))
        {
            content.AppendLine($"({EscapePdf(line)}) Tj");
            content.AppendLine("0 -18 Td");
        }

        content.AppendLine("ET");

        var stream = content.ToString();
        var objects = new List<string>
        {
            "<< /Type /Catalog /Pages 2 0 R >>",
            "<< /Type /Pages /Kids [3 0 R] /Count 1 >>",
            "<< /Type /Page /Parent 2 0 R /MediaBox [0 0 612 792] /Resources << /Font << /F1 4 0 R >> >> /Contents 5 0 R >>",
            "<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>",
            $"<< /Length {Encoding.ASCII.GetByteCount(stream)} >>\nstream\n{stream}endstream"
        };

        using var memory = new MemoryStream();
        using var writer = new StreamWriter(memory, Encoding.ASCII, leaveOpen: true);
        writer.WriteLine("%PDF-1.4");
        var offsets = new List<long> { 0 };

        for (var i = 0; i < objects.Count; i++)
        {
            writer.Flush();
            offsets.Add(memory.Position);
            writer.WriteLine($"{i + 1} 0 obj");
            writer.WriteLine(objects[i]);
            writer.WriteLine("endobj");
        }

        writer.Flush();
        var xrefPosition = memory.Position;
        writer.WriteLine("xref");
        writer.WriteLine($"0 {objects.Count + 1}");
        writer.WriteLine("0000000000 65535 f ");
        foreach (var offset in offsets.Skip(1))
        {
            writer.WriteLine($"{offset:0000000000} 00000 n ");
        }

        writer.WriteLine("trailer");
        writer.WriteLine($"<< /Size {objects.Count + 1} /Root 1 0 R >>");
        writer.WriteLine("startxref");
        writer.WriteLine(xrefPosition);
        writer.WriteLine("%%EOF");
        writer.Flush();

        return memory.ToArray();
    }

    private static string EscapePdf(string value)
    {
        return value.Replace("\\", "\\\\").Replace("(", "\\(").Replace(")", "\\)");
    }

    private async Task<string?> SaveUploadAsync(IFormFile? file, string folderName, string[] allowedContentTypes)
    {
        if (file is null || file.Length == 0)
        {
            return null;
        }

        if (!allowedContentTypes.Contains(file.ContentType) || file.Length > 10 * 1024 * 1024)
        {
            ModelState.AddModelError(string.Empty, "Upload must be a valid file type and smaller than 10MB.");
            return null;
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        var safeFileName = $"{Guid.NewGuid():N}{extension}";
        var uploadRoot = Path.Combine(_environment.WebRootPath, "uploads", "hr", folderName);
        System.IO.Directory.CreateDirectory(uploadRoot);

        var physicalPath = Path.Combine(uploadRoot, safeFileName);
        await using var stream = System.IO.File.Create(physicalPath);
        await file.CopyToAsync(stream);

        return $"/uploads/hr/{folderName}/{safeFileName}";
    }
}
