using imas.Assets.ApiService.Application.DTOs;

namespace imas.Assets.ApiService.Application.Interfaces;

public interface IAssetService
{
    Task<ApiResponse<IEnumerable<AssetDto>>> GetAllAssetsAsync();
    Task<ApiResponse<AssetDto>> GetAssetByIdAsync(int id);
    Task<ApiResponse<AssetDto>> GetAssetBySerialNumberAsync(string serialNumber);
    Task<ApiResponse<AssetDto>> GetAssetByAssetTagAsync(string assetTag);
    Task<ApiResponse<IEnumerable<AssetDto>>> GetAssetsByCategoryAsync(int categoryId);
    Task<ApiResponse<IEnumerable<AssetDto>>> GetAssetsByStatusAsync(string status);
    Task<ApiResponse<AssetDto>> CreateAssetAsync(CreateAssetDto createAssetDto);
    Task<ApiResponse<AssetDto>> UpdateAssetAsync(int id, UpdateAssetDto updateAssetDto);
    Task<ApiResponse> DeleteAssetAsync(int id);
}