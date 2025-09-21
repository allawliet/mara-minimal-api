using imas.Finance.ApiService.Domain.Common;

namespace imas.Finance.ApiService.Domain.Entities;

public class Budget : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal AllocatedAmount { get; set; }
    public decimal SpentAmount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = "Active";

    // Navigation properties
    public ICollection<Expense> Expenses { get; set; } = new List<Expense>();

    // Business logic methods
    public bool IsActive() => Status.Equals("Active", StringComparison.OrdinalIgnoreCase);
    
    public bool IsCompleted() => Status.Equals("Completed", StringComparison.OrdinalIgnoreCase);
    
    public bool IsExpired() => DateTime.Now > EndDate;
    
    public decimal GetRemainingAmount() => Math.Max(AllocatedAmount - SpentAmount, 0);
    
    public decimal GetSpentPercentage()
    {
        if (AllocatedAmount <= 0) return 0;
        return (SpentAmount / AllocatedAmount) * 100;
    }

    public bool IsOverBudget() => SpentAmount > AllocatedAmount;
    
    public decimal GetOverBudgetAmount() => Math.Max(SpentAmount - AllocatedAmount, 0);

    public TimeSpan GetRemainingDuration()
    {
        if (IsExpired()) return TimeSpan.Zero;
        return EndDate - DateTime.Now;
    }

    public void AddExpense(decimal amount)
    {
        SpentAmount += amount;
        UpdateTimestamp();
    }

    public void RemoveExpense(decimal amount)
    {
        SpentAmount = Math.Max(SpentAmount - amount, 0);
        UpdateTimestamp();
    }

    public void CompleteBudget()
    {
        Status = "Completed";
        UpdateTimestamp();
    }

    public bool CanAddExpense(decimal amount)
    {
        return IsActive() && !IsExpired() && (SpentAmount + amount <= AllocatedAmount * 1.1m); // Allow 10% overage
    }

    public string GetBudgetHealthStatus()
    {
        var spentPercentage = GetSpentPercentage();
        return spentPercentage switch
        {
            < 50 => "Good",
            < 80 => "Warning",
            < 100 => "Critical",
            _ => "Over Budget"
        };
    }
}