using System.ComponentModel.DataAnnotations;

namespace TextileManagementSystem.Models;

public class Supplier
{
    public int Id { get; set; }

    [Required]
    [StringLength(120)]
    public string SupplierName { get; set; } = string.Empty;

    [Required]
    [StringLength(30)]
    public string SupplierCode { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string ContactPerson { get; set; } = string.Empty;

    [Required]
    [Phone]
    [StringLength(30)]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(120)]
    public string EmailAddress { get; set; } = string.Empty;

    [Required]
    [StringLength(80)]
    public string BankAccount { get; set; } = string.Empty;

    [Required]
    [StringLength(30)]
    public string Status { get; set; } = "Active";

    [Required]
    [StringLength(160)]
    public string MaterialSpecialization { get; set; } = string.Empty;

    [Range(0, int.MaxValue)]
    public int OrdersCount { get; set; }

    [Range(0, double.MaxValue)]
    public decimal OutstandingBalance { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime? UpdatedAt { get; set; }

    public string Initials
    {
        get
        {
            var parts = SupplierName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
            {
                return "SP";
            }

            return string.Concat(parts.Take(2).Select(part => char.ToUpperInvariant(part[0])));
        }
    }

    public string MaskedBankAccount => BankAccount.Length <= 4
        ? "****"
        : $"**** {BankAccount[^4..]}";
}
