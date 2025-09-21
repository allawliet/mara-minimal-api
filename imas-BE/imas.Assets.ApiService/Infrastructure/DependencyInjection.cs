using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using imas.Assets.ApiService.Application.Interfaces;
using imas.Assets.ApiService.Infrastructure.Data;
using imas.Assets.ApiService.Infrastructure.Repositories;

namespace imas.Assets.ApiService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AssetsDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IAssetRepository, AssetRepository>();
        services.AddScoped<IAssetCategoryRepository, AssetCategoryRepository>();

        return services;
    }
}