namespace imas.Assets.ApiService.Application.DTOs;

public class AssetCategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int AssetCount { get; set; }
    public decimal TotalValue { get; set; }
}

public class CreateAssetCategoryDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class UpdateAssetCategoryDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
}