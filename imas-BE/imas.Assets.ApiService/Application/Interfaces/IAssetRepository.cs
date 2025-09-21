using imas.Assets.ApiService.Domain.Entities;

namespace imas.Assets.ApiService.Application.Interfaces;

public interface IAssetRepository
{
    Task<IEnumerable<Asset>> GetAllAsync();
    Task<Asset?> GetByIdAsync(int id);
    Task<Asset?> GetBySerialNumberAsync(string serialNumber);
    Task<Asset?> GetByAssetTagAsync(string assetTag);
    Task<IEnumerable<Asset>> GetByCategoryAsync(int categoryId);
    Task<IEnumerable<Asset>> GetByStatusAsync(string status);
    Task<Asset> CreateAsync(Asset asset);
    Task<Asset> UpdateAsync(Asset asset);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<bool> SerialNumberExistsAsync(string serialNumber, int? excludeId = null);
    Task<bool> AssetTagExistsAsync(string assetTag, int? excludeId = null);
}