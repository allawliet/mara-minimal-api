using imas.Finance.ApiService.Domain.Common;

namespace imas.Finance.ApiService.Domain.Entities;

public class Expense : BaseEntity
{
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Currency { get; set; } = "USD";
    public DateTime ExpenseDate { get; set; }
    public string Status { get; set; } = "Pending";
    public string Receipt { get; set; } = string.Empty;
    public int? BudgetId { get; set; }
    public int? EmployeeId { get; set; }

    // Navigation properties
    public Budget? Budget { get; set; }

    // Business logic methods
    public bool IsApproved() => Status.Equals("Approved", StringComparison.OrdinalIgnoreCase);
    
    public bool IsPending() => Status.Equals("Pending", StringComparison.OrdinalIgnoreCase);
    
    public bool IsRejected() => Status.Equals("Rejected", StringComparison.OrdinalIgnoreCase);
    
    public bool IsReimbursed() => Status.Equals("Reimbursed", StringComparison.OrdinalIgnoreCase);

    public void Approve()
    {
        if (IsPending())
        {
            Status = "Approved";
            UpdateTimestamp();
        }
    }

    public void Reject(string reason = "")
    {
        if (IsPending() || IsApproved())
        {
            Status = "Rejected";
            Receipt = $"{Receipt} | Rejected: {reason}".Trim();
            UpdateTimestamp();
        }
    }

    public void MarkAsReimbursed()
    {
        if (IsApproved())
        {
            Status = "Reimbursed";
            UpdateTimestamp();
        }
    }

    public bool HasReceipt() => !string.IsNullOrEmpty(Receipt);
    
    public bool IsValidCategory()
    {
        var validCategories = new[] { "Travel", "Office Supplies", "Meals", "Entertainment", "Training", "Equipment", "Software", "Other" };
        return validCategories.Contains(Category, StringComparer.OrdinalIgnoreCase);
    }

    public string GetFormattedAmount()
    {
        return $"{Currency} {Amount:N2}";
    }

    public TimeSpan GetAgeInDays()
    {
        return DateTime.Now - ExpenseDate;
    }

    public bool IsOldExpense(int maxDays = 90)
    {
        return GetAgeInDays().TotalDays > maxDays;
    }

    public bool RequiresApproval(decimal approvalThreshold = 1000m)
    {
        return Amount >= approvalThreshold;
    }
}