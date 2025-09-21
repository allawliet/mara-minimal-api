using imas.Assets.ApiService.Domain.Common;

namespace imas.Assets.ApiService.Domain.Entities;

public class AssetAssignment : BaseEntity
{
    public int AssetId { get; set; }
    public int EmployeeId { get; set; }
    public DateTime AssignedDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;

    // Navigation properties
    public Asset? Asset { get; set; }

    // Business logic methods
    public bool IsActive() => ReturnDate == null && Status.Equals("Active", StringComparison.OrdinalIgnoreCase);
    
    public bool IsReturned() => ReturnDate.HasValue && Status.Equals("Returned", StringComparison.OrdinalIgnoreCase);
    
    public TimeSpan GetAssignmentDuration()
    {
        var endDate = ReturnDate ?? DateTime.Now;
        return endDate - AssignedDate;
    }

    public void ReturnAsset(string? returnNotes = null)
    {
        ReturnDate = DateTime.Now;
        Status = "Returned";
        if (!string.IsNullOrEmpty(returnNotes))
        {
            Notes = $"{Notes}\nReturned: {returnNotes}".Trim();
        }
        UpdateTimestamp();
    }

    public bool IsOverdue(int maxDaysAssigned = 365)
    {
        if (IsReturned()) return false;
        return DateTime.Now.Subtract(AssignedDate).TotalDays > maxDaysAssigned;
    }
}