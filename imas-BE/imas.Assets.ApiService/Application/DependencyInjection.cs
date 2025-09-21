using imas.Assets.ApiService.Application.Interfaces;
using imas.Assets.ApiService.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace imas.Assets.ApiService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAssetService, AssetService>();
        services.AddScoped<IAssetCategoryService, AssetCategoryService>();

        return services;
    }
}