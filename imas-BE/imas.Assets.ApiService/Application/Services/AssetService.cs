using imas.Assets.ApiService.Application.DTOs;
using imas.Assets.ApiService.Application.Interfaces;
using imas.Assets.ApiService.Domain.Entities;

namespace imas.Assets.ApiService.Application.Services;

public class AssetService : IAssetService
{
    private readonly IAssetRepository _assetRepository;
    private readonly IAssetCategoryRepository _categoryRepository;

    public AssetService(IAssetRepository assetRepository, IAssetCategoryRepository categoryRepository)
    {
        _assetRepository = assetRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<ApiResponse<IEnumerable<AssetDto>>> GetAllAssetsAsync()
    {
        try
        {
            var assets = await _assetRepository.GetAllAsync();
            var assetDtos = assets.Select(MapToDto);
            return ApiResponse<IEnumerable<AssetDto>>.SuccessResult(assetDtos);
        }
        catch (Exception ex)
        {
            return ApiResponse<IEnumerable<AssetDto>>.FailureResult($"Error retrieving assets: {ex.Message}");
        }
    }

    public async Task<ApiResponse<AssetDto>> GetAssetByIdAsync(int id)
    {
        try
        {
            var asset = await _assetRepository.GetByIdAsync(id);
            if (asset == null)
                return ApiResponse<AssetDto>.FailureResult("Asset not found");

            var assetDto = MapToDto(asset);
            return ApiResponse<AssetDto>.SuccessResult(assetDto);
        }
        catch (Exception ex)
        {
            return ApiResponse<AssetDto>.FailureResult($"Error retrieving asset: {ex.Message}");
        }
    }

    public async Task<ApiResponse<AssetDto>> GetAssetBySerialNumberAsync(string serialNumber)
    {
        try
        {
            var asset = await _assetRepository.GetBySerialNumberAsync(serialNumber);
            if (asset == null)
                return ApiResponse<AssetDto>.FailureResult("Asset not found");

            var assetDto = MapToDto(asset);
            return ApiResponse<AssetDto>.SuccessResult(assetDto);
        }
        catch (Exception ex)
        {
            return ApiResponse<AssetDto>.FailureResult($"Error retrieving asset: {ex.Message}");
        }
    }

    public async Task<ApiResponse<AssetDto>> GetAssetByAssetTagAsync(string assetTag)
    {
        try
        {
            var asset = await _assetRepository.GetByAssetTagAsync(assetTag);
            if (asset == null)
                return ApiResponse<AssetDto>.FailureResult("Asset not found");

            var assetDto = MapToDto(asset);
            return ApiResponse<AssetDto>.SuccessResult(assetDto);
        }
        catch (Exception ex)
        {
            return ApiResponse<AssetDto>.FailureResult($"Error retrieving asset: {ex.Message}");
        }
    }

    public async Task<ApiResponse<IEnumerable<AssetDto>>> GetAssetsByCategoryAsync(int categoryId)
    {
        try
        {
            var assets = await _assetRepository.GetByCategoryAsync(categoryId);
            var assetDtos = assets.Select(MapToDto);
            return ApiResponse<IEnumerable<AssetDto>>.SuccessResult(assetDtos);
        }
        catch (Exception ex)
        {
            return ApiResponse<IEnumerable<AssetDto>>.FailureResult($"Error retrieving assets: {ex.Message}");
        }
    }

    public async Task<ApiResponse<IEnumerable<AssetDto>>> GetAssetsByStatusAsync(string status)
    {
        try
        {
            var assets = await _assetRepository.GetByStatusAsync(status);
            var assetDtos = assets.Select(MapToDto);
            return ApiResponse<IEnumerable<AssetDto>>.SuccessResult(assetDtos);
        }
        catch (Exception ex)
        {
            return ApiResponse<IEnumerable<AssetDto>>.FailureResult($"Error retrieving assets: {ex.Message}");
        }
    }

    public async Task<ApiResponse<AssetDto>> CreateAssetAsync(CreateAssetDto createAssetDto)
    {
        try
        {
            // Validate category exists
            if (!await _categoryRepository.ExistsAsync(createAssetDto.CategoryId))
                return ApiResponse<AssetDto>.FailureResult("Category not found");

            // Check for duplicate serial number
            if (await _assetRepository.SerialNumberExistsAsync(createAssetDto.SerialNumber))
                return ApiResponse<AssetDto>.FailureResult("Serial number already exists");

            // Check for duplicate asset tag
            if (await _assetRepository.AssetTagExistsAsync(createAssetDto.AssetTag))
                return ApiResponse<AssetDto>.FailureResult("Asset tag already exists");

            var asset = new Asset
            {
                Name = createAssetDto.Name,
                Description = createAssetDto.Description,
                SerialNumber = createAssetDto.SerialNumber,
                AssetTag = createAssetDto.AssetTag,
                Model = createAssetDto.Model,
                Manufacturer = createAssetDto.Manufacturer,
                CategoryId = createAssetDto.CategoryId,
                LocationId = createAssetDto.LocationId,
                PurchaseDate = createAssetDto.PurchaseDate,
                PurchasePrice = createAssetDto.PurchasePrice,
                CurrentValue = createAssetDto.CurrentValue,
                Status = createAssetDto.Status
            };

            var createdAsset = await _assetRepository.CreateAsync(asset);
            var assetDto = MapToDto(createdAsset);
            return ApiResponse<AssetDto>.SuccessResult(assetDto, "Asset created successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<AssetDto>.FailureResult($"Error creating asset: {ex.Message}");
        }
    }

    public async Task<ApiResponse<AssetDto>> UpdateAssetAsync(int id, UpdateAssetDto updateAssetDto)
    {
        try
        {
            var asset = await _assetRepository.GetByIdAsync(id);
            if (asset == null)
                return ApiResponse<AssetDto>.FailureResult("Asset not found");

            // Check for duplicate serial number if provided
            if (!string.IsNullOrEmpty(updateAssetDto.SerialNumber) && 
                await _assetRepository.SerialNumberExistsAsync(updateAssetDto.SerialNumber, id))
                return ApiResponse<AssetDto>.FailureResult("Serial number already exists");

            // Check for duplicate asset tag if provided
            if (!string.IsNullOrEmpty(updateAssetDto.AssetTag) && 
                await _assetRepository.AssetTagExistsAsync(updateAssetDto.AssetTag, id))
                return ApiResponse<AssetDto>.FailureResult("Asset tag already exists");

            // Update fields if provided
            if (!string.IsNullOrEmpty(updateAssetDto.Name))
                asset.Name = updateAssetDto.Name;
            if (!string.IsNullOrEmpty(updateAssetDto.Description))
                asset.Description = updateAssetDto.Description;
            if (!string.IsNullOrEmpty(updateAssetDto.SerialNumber))
                asset.SerialNumber = updateAssetDto.SerialNumber;
            if (!string.IsNullOrEmpty(updateAssetDto.AssetTag))
                asset.AssetTag = updateAssetDto.AssetTag;
            if (!string.IsNullOrEmpty(updateAssetDto.Model))
                asset.Model = updateAssetDto.Model;
            if (!string.IsNullOrEmpty(updateAssetDto.Manufacturer))
                asset.Manufacturer = updateAssetDto.Manufacturer;
            if (updateAssetDto.CategoryId.HasValue)
            {
                if (!await _categoryRepository.ExistsAsync(updateAssetDto.CategoryId.Value))
                    return ApiResponse<AssetDto>.FailureResult("Category not found");
                asset.CategoryId = updateAssetDto.CategoryId.Value;
            }
            if (updateAssetDto.LocationId.HasValue)
                asset.LocationId = updateAssetDto.LocationId.Value;
            if (updateAssetDto.PurchaseDate.HasValue)
                asset.PurchaseDate = updateAssetDto.PurchaseDate.Value;
            if (updateAssetDto.PurchasePrice.HasValue)
                asset.PurchasePrice = updateAssetDto.PurchasePrice.Value;
            if (updateAssetDto.CurrentValue.HasValue)
                asset.CurrentValue = updateAssetDto.CurrentValue.Value;
            if (!string.IsNullOrEmpty(updateAssetDto.Status))
                asset.Status = updateAssetDto.Status;

            asset.UpdateTimestamp();

            var updatedAsset = await _assetRepository.UpdateAsync(asset);
            var assetDto = MapToDto(updatedAsset);
            return ApiResponse<AssetDto>.SuccessResult(assetDto, "Asset updated successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<AssetDto>.FailureResult($"Error updating asset: {ex.Message}");
        }
    }

    public async Task<ApiResponse> DeleteAssetAsync(int id)
    {
        try
        {
            if (!await _assetRepository.ExistsAsync(id))
                return ApiResponse.FailureResult("Asset not found");

            await _assetRepository.DeleteAsync(id);
            return ApiResponse.SuccessResult("Asset deleted successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse.FailureResult($"Error deleting asset: {ex.Message}");
        }
    }

    private static AssetDto MapToDto(Asset asset)
    {
        return new AssetDto
        {
            Id = asset.Id,
            Name = asset.Name,
            Description = asset.Description,
            SerialNumber = asset.SerialNumber,
            AssetTag = asset.AssetTag,
            Model = asset.Model,
            Manufacturer = asset.Manufacturer,
            CategoryId = asset.CategoryId,
            CategoryName = asset.Category?.Name,
            LocationId = asset.LocationId,
            PurchaseDate = asset.PurchaseDate,
            PurchasePrice = asset.PurchasePrice,
            CurrentValue = asset.CurrentValue,
            Status = asset.Status,
            CreatedAt = asset.CreatedAt,
            UpdatedAt = asset.UpdatedAt,
            RequiresMaintenance = asset.RequiresMaintenance(),
            CurrentAssigneeId = asset.GetCurrentAssignment()?.EmployeeId
        };
    }
}