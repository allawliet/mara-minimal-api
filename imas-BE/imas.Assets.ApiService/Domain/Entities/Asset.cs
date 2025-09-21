using imas.Assets.ApiService.Domain.Common;

namespace imas.Assets.ApiService.Domain.Entities;

public class Asset : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string SerialNumber { get; set; } = string.Empty;
    public string AssetTag { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string Manufacturer { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public int LocationId { get; set; }
    public DateTime PurchaseDate { get; set; }
    public decimal PurchasePrice { get; set; }
    public decimal CurrentValue { get; set; }
    public string Status { get; set; } = string.Empty;

    // Navigation properties
    public AssetCategory? Category { get; set; }
    public ICollection<MaintenanceRecord> MaintenanceRecords { get; set; } = new List<MaintenanceRecord>();
    public ICollection<AssetAssignment> Assignments { get; set; } = new List<AssetAssignment>();

    // Business logic methods
    public bool IsAvailable() => Status.Equals("Available", StringComparison.OrdinalIgnoreCase);
    
    public bool IsAssigned() => Status.Equals("Assigned", StringComparison.OrdinalIgnoreCase);
    
    public bool IsUnderMaintenance() => Status.Equals("Maintenance", StringComparison.OrdinalIgnoreCase);
    
    public decimal CalculateDepreciation(decimal depreciationRate = 0.1m)
    {
        var yearsOwned = (DateTime.Now - PurchaseDate).Days / 365.25;
        var depreciation = PurchasePrice * (decimal)yearsOwned * depreciationRate;
        return Math.Max(PurchasePrice - depreciation, 0);
    }

    public bool RequiresMaintenance()
    {
        var lastMaintenance = MaintenanceRecords
            .Where(mr => mr.CompletedDate.HasValue)
            .OrderByDescending(mr => mr.CompletedDate)
            .FirstOrDefault();

        if (lastMaintenance == null) return true;
        
        return DateTime.Now.Subtract(lastMaintenance.CompletedDate.Value).TotalDays > 90;
    }

    public AssetAssignment? GetCurrentAssignment()
    {
        return Assignments.FirstOrDefault(a => a.ReturnDate == null && a.Status == "Active");
    }
}