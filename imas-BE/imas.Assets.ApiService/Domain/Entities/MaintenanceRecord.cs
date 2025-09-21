using imas.Assets.ApiService.Domain.Common;

namespace imas.Assets.ApiService.Domain.Entities;

public class MaintenanceRecord : BaseEntity
{
    public int AssetId { get; set; }
    public string MaintenanceType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime ScheduledDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public decimal Cost { get; set; }
    public int TechnicianId { get; set; }
    public string Status { get; set; } = string.Empty;

    // Navigation properties
    public Asset? Asset { get; set; }

    // Business logic methods
    public bool IsCompleted() => CompletedDate.HasValue && Status.Equals("Completed", StringComparison.OrdinalIgnoreCase);
    
    public bool IsOverdue() => !IsCompleted() && DateTime.Now > ScheduledDate;
    
    public bool IsPending() => Status.Equals("Pending", StringComparison.OrdinalIgnoreCase);
    
    public bool IsInProgress() => Status.Equals("In Progress", StringComparison.OrdinalIgnoreCase);
    
    public TimeSpan GetDuration()
    {
        if (!CompletedDate.HasValue) return TimeSpan.Zero;
        return CompletedDate.Value - ScheduledDate;
    }

    public void MarkAsCompleted()
    {
        CompletedDate = DateTime.Now;
        Status = "Completed";
        UpdateTimestamp();
    }

    public void StartMaintenance()
    {
        Status = "In Progress";
        UpdateTimestamp();
    }
}