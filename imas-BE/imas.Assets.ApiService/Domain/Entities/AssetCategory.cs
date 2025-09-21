using imas.Assets.ApiService.Domain.Common;

namespace imas.Assets.ApiService.Domain.Entities;

public class AssetCategory : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    // Navigation properties
    public ICollection<Asset> Assets { get; set; } = new List<Asset>();

    // Business logic methods
    public int GetAssetCount() => Assets.Count;
    
    public decimal GetTotalValue() => Assets.Sum(a => a.CurrentValue);
    
    public IEnumerable<Asset> GetAvailableAssets() => Assets.Where(a => a.IsAvailable());
    
    public IEnumerable<Asset> GetAssignedAssets() => Assets.Where(a => a.IsAssigned());
}