using imas.Assets.ApiService.Application.DTOs;

namespace imas.Assets.ApiService.Application.Interfaces;

public interface IAssetCategoryService
{
    Task<ApiResponse<IEnumerable<AssetCategoryDto>>> GetAllCategoriesAsync();
    Task<ApiResponse<AssetCategoryDto>> GetCategoryByIdAsync(int id);
    Task<ApiResponse<AssetCategoryDto>> CreateCategoryAsync(CreateAssetCategoryDto createCategoryDto);
    Task<ApiResponse<AssetCategoryDto>> UpdateCategoryAsync(int id, UpdateAssetCategoryDto updateCategoryDto);
    Task<ApiResponse> DeleteCategoryAsync(int id);
}