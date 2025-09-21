using imas.Finance.ApiService.Domain.Common;

namespace imas.Finance.ApiService.Domain.Entities;

public class Invoice : BaseEntity
{
    public string InvoiceNumber { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; } = "USD";
    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }
    public string Status { get; set; } = "Draft";

    // Navigation properties
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();

    // Business logic methods
    public bool IsPaid() => Status.Equals("Paid", StringComparison.OrdinalIgnoreCase);
    
    public bool IsOverdue() => DateTime.Now > DueDate && !IsPaid();
    
    public bool IsDraft() => Status.Equals("Draft", StringComparison.OrdinalIgnoreCase);
    
    public bool IsSent() => Status.Equals("Sent", StringComparison.OrdinalIgnoreCase);
    
    public decimal GetOutstandingAmount()
    {
        var totalPaid = Payments.Where(p => p.IsCompleted()).Sum(p => p.Amount);
        return Math.Max(TotalAmount - totalPaid, 0);
    }

    public decimal GetPaidAmount()
    {
        return Payments.Where(p => p.IsCompleted()).Sum(p => p.Amount);
    }

    public TimeSpan GetDaysOverdue()
    {
        if (!IsOverdue()) return TimeSpan.Zero;
        return DateTime.Now - DueDate;
    }

    public void CalculateTotalAmount(decimal taxRate = 0.1m)
    {
        TaxAmount = Amount * taxRate;
        TotalAmount = Amount + TaxAmount;
        UpdateTimestamp();
    }

    public void MarkAsSent()
    {
        if (IsDraft())
        {
            Status = "Sent";
            UpdateTimestamp();
        }
    }

    public void MarkAsPaid()
    {
        if (GetOutstandingAmount() <= 0)
        {
            Status = "Paid";
            UpdateTimestamp();
        }
    }

    public bool CanBeDeleted() => IsDraft() && Payments.Count == 0;
}