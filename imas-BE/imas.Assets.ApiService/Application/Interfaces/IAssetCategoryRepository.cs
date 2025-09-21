using imas.Assets.ApiService.Domain.Entities;

namespace imas.Assets.ApiService.Application.Interfaces;

public interface IAssetCategoryRepository
{
    Task<IEnumerable<AssetCategory>> GetAllAsync();
    Task<AssetCategory?> GetByIdAsync(int id);
    Task<AssetCategory?> GetByNameAsync(string name);
    Task<AssetCategory> CreateAsync(AssetCategory category);
    Task<AssetCategory> UpdateAsync(AssetCategory category);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<bool> NameExistsAsync(string name, int? excludeId = null);
}