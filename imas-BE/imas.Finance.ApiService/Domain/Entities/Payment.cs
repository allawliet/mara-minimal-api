using imas.Finance.ApiService.Domain.Common;

namespace imas.Finance.ApiService.Domain.Entities;

public class Payment : BaseEntity
{
    public string PaymentNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string Currency { get; set; } = "USD";
    public DateTime PaymentDate { get; set; }
    public string Status { get; set; } = "Pending";
    public string Reference { get; set; } = string.Empty;
    public int? InvoiceId { get; set; }

    // Navigation properties
    public Invoice? Invoice { get; set; }

    // Business logic methods
    public bool IsCompleted() => Status.Equals("Completed", StringComparison.OrdinalIgnoreCase);
    
    public bool IsPending() => Status.Equals("Pending", StringComparison.OrdinalIgnoreCase);
    
    public bool IsFailed() => Status.Equals("Failed", StringComparison.OrdinalIgnoreCase);
    
    public bool IsCancelled() => Status.Equals("Cancelled", StringComparison.OrdinalIgnoreCase);

    public void CompletePayment()
    {
        if (IsPending())
        {
            Status = "Completed";
            PaymentDate = DateTime.UtcNow;
            UpdateTimestamp();
        }
    }

    public void FailPayment(string reason = "")
    {
        if (IsPending())
        {
            Status = "Failed";
            Reference = $"{Reference} | Failed: {reason}".Trim();
            UpdateTimestamp();
        }
    }

    public void CancelPayment(string reason = "")
    {
        if (IsPending())
        {
            Status = "Cancelled";
            Reference = $"{Reference} | Cancelled: {reason}".Trim();
            UpdateTimestamp();
        }
    }

    public bool IsValidPaymentMethod()
    {
        var validMethods = new[] { "Cash", "Credit Card", "Debit Card", "Bank Transfer", "Check", "Online Payment" };
        return validMethods.Contains(PaymentMethod, StringComparer.OrdinalIgnoreCase);
    }

    public string GetFormattedAmount()
    {
        return $"{Currency} {Amount:N2}";
    }
}