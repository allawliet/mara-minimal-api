using Microsoft.AspNetCore.Mvc;
using imas.Assets.ApiService.Application.DTOs;
using imas.Assets.ApiService.Application.Interfaces;

namespace imas.Assets.ApiService.Presentation.Endpoints;

public static class AssetEndpoints
{
    public static void MapAssetEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/assets")
            .WithTags("Assets")
            .WithOpenApi();

        group.MapGet("/", GetAllAssets)
             .WithName("GetAllAssets")
             .WithSummary("Get all assets")
             .Produces<ApiResponse<IEnumerable<AssetDto>>>();

        group.MapGet("/{id:int}", GetAssetById)
             .WithName("GetAssetById")
             .WithSummary("Get asset by ID")
             .Produces<ApiResponse<AssetDto>>()
             .Produces(404);

        group.MapGet("/serial/{serialNumber}", GetAssetBySerialNumber)
             .WithName("GetAssetBySerialNumber")
             .WithSummary("Get asset by serial number")
             .Produces<ApiResponse<AssetDto>>()
             .Produces(404);

        group.MapGet("/tag/{assetTag}", GetAssetByAssetTag)
             .WithName("GetAssetByAssetTag")
             .WithSummary("Get asset by asset tag")
             .Produces<ApiResponse<AssetDto>>()
             .Produces(404);

        group.MapGet("/category/{categoryId:int}", GetAssetsByCategory)
             .WithName("GetAssetsByCategory")
             .WithSummary("Get assets by category")
             .Produces<ApiResponse<IEnumerable<AssetDto>>>();

        group.MapGet("/status/{status}", GetAssetsByStatus)
             .WithName("GetAssetsByStatus")
             .WithSummary("Get assets by status")
             .Produces<ApiResponse<IEnumerable<AssetDto>>>();

        group.MapPost("/", CreateAsset)
             .WithName("CreateAsset")
             .WithSummary("Create a new asset")
             .Produces<ApiResponse<AssetDto>>(201)
             .Produces<ApiResponse<AssetDto>>(400);

        group.MapPut("/{id:int}", UpdateAsset)
             .WithName("UpdateAsset")
             .WithSummary("Update an asset")
             .Produces<ApiResponse<AssetDto>>()
             .Produces<ApiResponse<AssetDto>>(400)
             .Produces(404);

        group.MapDelete("/{id:int}", DeleteAsset)
             .WithName("DeleteAsset")
             .WithSummary("Delete an asset")
             .Produces<ApiResponse>(204)
             .Produces(404);
    }

    private static async Task<IResult> GetAllAssets(IAssetService assetService)
    {
        var result = await assetService.GetAllAssetsAsync();
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }

    private static async Task<IResult> GetAssetById(int id, IAssetService assetService)
    {
        var result = await assetService.GetAssetByIdAsync(id);
        return result.Success ? Results.Ok(result) : Results.NotFound(result);
    }

    private static async Task<IResult> GetAssetBySerialNumber(string serialNumber, IAssetService assetService)
    {
        var result = await assetService.GetAssetBySerialNumberAsync(serialNumber);
        return result.Success ? Results.Ok(result) : Results.NotFound(result);
    }

    private static async Task<IResult> GetAssetByAssetTag(string assetTag, IAssetService assetService)
    {
        var result = await assetService.GetAssetByAssetTagAsync(assetTag);
        return result.Success ? Results.Ok(result) : Results.NotFound(result);
    }

    private static async Task<IResult> GetAssetsByCategory(int categoryId, IAssetService assetService)
    {
        var result = await assetService.GetAssetsByCategoryAsync(categoryId);
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }

    private static async Task<IResult> GetAssetsByStatus(string status, IAssetService assetService)
    {
        var result = await assetService.GetAssetsByStatusAsync(status);
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }

    private static async Task<IResult> CreateAsset([FromBody] CreateAssetDto createAssetDto, IAssetService assetService)
    {
        var result = await assetService.CreateAssetAsync(createAssetDto);
        return result.Success ? Results.Created($"/api/assets/{result.Data?.Id}", result) : Results.BadRequest(result);
    }

    private static async Task<IResult> UpdateAsset(int id, [FromBody] UpdateAssetDto updateAssetDto, IAssetService assetService)
    {
        var result = await assetService.UpdateAssetAsync(id, updateAssetDto);
        return result.Success ? Results.Ok(result) : 
               result.Message.Contains("not found") ? Results.NotFound(result) : Results.BadRequest(result);
    }

    private static async Task<IResult> DeleteAsset(int id, IAssetService assetService)
    {
        var result = await assetService.DeleteAssetAsync(id);
        return result.Success ? Results.NoContent() : 
               result.Message.Contains("not found") ? Results.NotFound(result) : Results.BadRequest(result);
    }
}