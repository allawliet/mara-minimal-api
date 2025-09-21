using Microsoft.AspNetCore.Mvc;
using imas.Assets.ApiService.Application.DTOs;
using imas.Assets.ApiService.Application.Interfaces;

namespace imas.Assets.ApiService.Presentation.Endpoints;

public static class AssetCategoryEndpoints
{
    public static void MapAssetCategoryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/asset-categories")
            .WithTags("Asset Categories")
            .WithOpenApi();

        group.MapGet("/", GetAllCategories)
             .WithName("GetAllAssetCategories")
             .WithSummary("Get all asset categories")
             .Produces<ApiResponse<IEnumerable<AssetCategoryDto>>>();

        group.MapGet("/{id:int}", GetCategoryById)
             .WithName("GetAssetCategoryById")
             .WithSummary("Get asset category by ID")
             .Produces<ApiResponse<AssetCategoryDto>>()
             .Produces(404);

        group.MapPost("/", CreateCategory)
             .WithName("CreateAssetCategory")
             .WithSummary("Create a new asset category")
             .Produces<ApiResponse<AssetCategoryDto>>(201)
             .Produces<ApiResponse<AssetCategoryDto>>(400);

        group.MapPut("/{id:int}", UpdateCategory)
             .WithName("UpdateAssetCategory")
             .WithSummary("Update an asset category")
             .Produces<ApiResponse<AssetCategoryDto>>()
             .Produces<ApiResponse<AssetCategoryDto>>(400)
             .Produces(404);

        group.MapDelete("/{id:int}", DeleteCategory)
             .WithName("DeleteAssetCategory")
             .WithSummary("Delete an asset category")
             .Produces<ApiResponse>(204)
             .Produces(404);
    }

    private static async Task<IResult> GetAllCategories(IAssetCategoryService categoryService)
    {
        var result = await categoryService.GetAllCategoriesAsync();
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }

    private static async Task<IResult> GetCategoryById(int id, IAssetCategoryService categoryService)
    {
        var result = await categoryService.GetCategoryByIdAsync(id);
        return result.Success ? Results.Ok(result) : Results.NotFound(result);
    }

    private static async Task<IResult> CreateCategory([FromBody] CreateAssetCategoryDto createCategoryDto, IAssetCategoryService categoryService)
    {
        var result = await categoryService.CreateCategoryAsync(createCategoryDto);
        return result.Success ? Results.Created($"/api/asset-categories/{result.Data?.Id}", result) : Results.BadRequest(result);
    }

    private static async Task<IResult> UpdateCategory(int id, [FromBody] UpdateAssetCategoryDto updateCategoryDto, IAssetCategoryService categoryService)
    {
        var result = await categoryService.UpdateCategoryAsync(id, updateCategoryDto);
        return result.Success ? Results.Ok(result) : 
               result.Message.Contains("not found") ? Results.NotFound(result) : Results.BadRequest(result);
    }

    private static async Task<IResult> DeleteCategory(int id, IAssetCategoryService categoryService)
    {
        var result = await categoryService.DeleteCategoryAsync(id);
        return result.Success ? Results.NoContent() : 
               result.Message.Contains("not found") ? Results.NotFound(result) : Results.BadRequest(result);
    }
}