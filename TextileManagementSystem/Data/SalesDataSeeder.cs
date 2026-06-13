using Microsoft.EntityFrameworkCore;
using TextileManagementSystem.Models;

namespace TextileManagementSystem.Data;

public static class SalesDataSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (await context.SalesInvoices.AnyAsync())
        {
            return;
        }

        var invoices = new List<SalesInvoice>
        {
            new()
            {
                InvoiceNumber = "INV-2023-452",
                CustomerName = "SineRoute Fabrics",
                BillingAddress = "Factory Area, Block A, Karachi",
                Currency = "USD",
                InvoiceDate = DateTime.Today.AddDays(-12),
                DueDate = DateTime.Today.AddDays(18),
                PaymentTerms = "Net 30",
                PONumber = "PO-9081",
                Status = "Paid",
                TaxRate = 18,
                AmountReceived = 177000,
                CreatedAt = DateTime.Now.AddDays(-12),
                LineItems =
                [
                    new() { ProductDescription = "Egyptian Cotton Indigo Twill", HSNCode = "5208", Quantity = 1200, Unit = "Meters", Rate = 125m, DiscountPercent = 6, SortOrder = 1 },
                    new() { ProductDescription = "Premium Cotton Dyeing", HSNCode = "3204", Quantity = 280, Unit = "Kg", Rate = 65m, DiscountPercent = 0, SortOrder = 2 }
                ]
            },
            new()
            {
                InvoiceNumber = "INV-2023-448",
                CustomerName = "Urban Looms",
                BillingAddress = "Site 7, Lahore Industrial Estate",
                Currency = "USD",
                InvoiceDate = DateTime.Today.AddDays(-16),
                DueDate = DateTime.Today.AddDays(-2),
                PaymentTerms = "Net 14",
                PONumber = "PO-8817",
                Status = "Overdue",
                TaxRate = 18,
                AmountReceived = 0,
                CreatedAt = DateTime.Now.AddDays(-16),
                LineItems =
                [
                    new() { ProductDescription = "Linen Canvas Rolls", HSNCode = "5309", Quantity = 930, Unit = "Meters", Rate = 95m, DiscountPercent = 4, SortOrder = 1 },
                    new() { ProductDescription = "Finishing Services", HSNCode = "9988", Quantity = 1, Unit = "Lot", Rate = 14700m, DiscountPercent = 0, SortOrder = 2 }
                ]
            },
            new()
            {
                InvoiceNumber = "INV-2023-443",
                CustomerName = "Indigo Fabrics",
                BillingAddress = "Textile Park, Faisalabad",
                Currency = "USD",
                InvoiceDate = DateTime.Today.AddDays(-9),
                DueDate = DateTime.Today.AddDays(21),
                PaymentTerms = "Net 30",
                PONumber = "PO-8740",
                Status = "Sent",
                TaxRate = 18,
                AmountReceived = 120000,
                CreatedAt = DateTime.Now.AddDays(-9),
                LineItems =
                [
                    new() { ProductDescription = "Modal Jersey Knits", HSNCode = "6006", Quantity = 760, Unit = "Meters", Rate = 132m, DiscountPercent = 3, SortOrder = 1 },
                    new() { ProductDescription = "Logistics and Packing", HSNCode = "9965", Quantity = 1, Unit = "Lot", Rate = 9500m, DiscountPercent = 0, SortOrder = 2 }
                ]
            }
        };

        foreach (var invoice in invoices)
        {
            Recalculate(invoice);
        }

        context.SalesInvoices.AddRange(invoices);
        await context.SaveChangesAsync();
    }

    private static void Recalculate(SalesInvoice invoice)
    {
        var subtotal = invoice.LineItems.Sum(line =>
        {
            var gross = line.Quantity * line.Rate;
            var discount = gross * (line.DiscountPercent / 100m);
            line.LineTotal = Math.Round(gross - discount, 2);
            return line.LineTotal;
        });

        invoice.Subtotal = Math.Round(subtotal, 2);
        invoice.TaxAmount = Math.Round(invoice.Subtotal * (invoice.TaxRate / 100m), 2);

        var grossTotal = invoice.Subtotal + invoice.TaxAmount;
        var roundedTotal = Math.Round(grossTotal, 0, MidpointRounding.AwayFromZero);

        invoice.RoundOff = roundedTotal - grossTotal;
        invoice.GrandTotal = roundedTotal;
    }
}
