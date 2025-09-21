using imas.Assets.ApiService.Application.DTOs;
using imas.Assets.ApiService.Application.Interfaces;
using imas.Assets.ApiService.Domain.Entities;

namespace imas.Assets.ApiService.Application.Services;

public class AssetCategoryService : IAssetCategoryService
{
    private readonly IAssetCategoryRepository _categoryRepository;

    public AssetCategoryService(IAssetCategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<ApiResponse<IEnumerable<AssetCategoryDto>>> GetAllCategoriesAsync()
    {
        try
        {
            var categories = await _categoryRepository.GetAllAsync();
            var categoryDtos = categories.Select(MapToDto);
            return ApiResponse<IEnumerable<AssetCategoryDto>>.SuccessResult(categoryDtos);
        }
        catch (Exception ex)
        {
            return ApiResponse<IEnumerable<AssetCategoryDto>>.FailureResult($"Error retrieving categories: {ex.Message}");
        }
    }

    public async Task<ApiResponse<AssetCategoryDto>> GetCategoryByIdAsync(int id)
    {
        try
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                return ApiResponse<AssetCategoryDto>.FailureResult("Category not found");

            var categoryDto = MapToDto(category);
            return ApiResponse<AssetCategoryDto>.SuccessResult(categoryDto);
        }
        catch (Exception ex)
        {
            return ApiResponse<AssetCategoryDto>.FailureResult($"Error retrieving category: {ex.Message}");
        }
    }

    public async Task<ApiResponse<AssetCategoryDto>> CreateCategoryAsync(CreateAssetCategoryDto createCategoryDto)
    {
        try
        {
            // Check for duplicate name
            if (await _categoryRepository.NameExistsAsync(createCategoryDto.Name))
                return ApiResponse<AssetCategoryDto>.FailureResult("Category name already exists");

            var category = new AssetCategory
            {
                Name = createCategoryDto.Name,
                Description = createCategoryDto.Description
            };

            var createdCategory = await _categoryRepository.CreateAsync(category);
            var categoryDto = MapToDto(createdCategory);
            return ApiResponse<AssetCategoryDto>.SuccessResult(categoryDto, "Category created successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<AssetCategoryDto>.FailureResult($"Error creating category: {ex.Message}");
        }
    }

    public async Task<ApiResponse<AssetCategoryDto>> UpdateCategoryAsync(int id, UpdateAssetCategoryDto updateCategoryDto)
    {
        try
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                return ApiResponse<AssetCategoryDto>.FailureResult("Category not found");

            // Check for duplicate name if provided
            if (!string.IsNullOrEmpty(updateCategoryDto.Name) && 
                await _categoryRepository.NameExistsAsync(updateCategoryDto.Name, id))
                return ApiResponse<AssetCategoryDto>.FailureResult("Category name already exists");

            // Update fields if provided
            if (!string.IsNullOrEmpty(updateCategoryDto.Name))
                category.Name = updateCategoryDto.Name;
            if (!string.IsNullOrEmpty(updateCategoryDto.Description))
                category.Description = updateCategoryDto.Description;

            category.UpdateTimestamp();

            var updatedCategory = await _categoryRepository.UpdateAsync(category);
            var categoryDto = MapToDto(updatedCategory);
            return ApiResponse<AssetCategoryDto>.SuccessResult(categoryDto, "Category updated successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<AssetCategoryDto>.FailureResult($"Error updating category: {ex.Message}");
        }
    }

    public async Task<ApiResponse> DeleteCategoryAsync(int id)
    {
        try
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                return ApiResponse.FailureResult("Category not found");

            // Check if category has assets
            if (category.GetAssetCount() > 0)
                return ApiResponse.FailureResult("Cannot delete category with existing assets");

            await _categoryRepository.DeleteAsync(id);
            return ApiResponse.SuccessResult("Category deleted successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse.FailureResult($"Error deleting category: {ex.Message}");
        }
    }

    private static AssetCategoryDto MapToDto(AssetCategory category)
    {
        return new AssetCategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt,
            AssetCount = category.GetAssetCount(),
            TotalValue = category.GetTotalValue()
        };
    }
}