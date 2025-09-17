using Microsoft.EntityFrameworkCore;
using mara.ApiService.Infrastructure.Data;
using mara.ApiService.Infrastructure.Repository;
using mara.ApiService.Modules.Authentication;
using mara.ApiService.Modules.Todos;
using mara.ApiService.Modules.Weather;

namespace mara.ApiService.Infrastructure.Extensions;

/// <summary>
/// Database configuration extensions
/// </summary>
public static class DatabaseExtensions
{
    /// <summary>
    /// Add database services to the service collection
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configuration">Configuration</param>
    /// <returns>Service collection</returns>
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        var provider = configuration.GetValue<string>("DatabaseProvider") ?? "SqlServer";

        // Add Entity Framework DbContext for SQL Server
        if (provider.ToLower() != "sqlserver")
        {
            throw new InvalidOperationException($"Only SqlServer is supported. Current provider: {provider}");
        }

        services.AddDbContext<MaraDbContext>(options =>
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null);
            }));
        
        // Add SQL Server connection factory for Dapper
        services.AddSingleton<IDbConnectionFactory>(provider =>
            new SqlServerConnectionFactory(connectionString));

        // Register repositories
        RegisterRepositories(services);

        return services;
    }

    /// <summary>
    /// Register repository implementations
    /// </summary>
    /// <param name="services">Service collection</param>
    private static void RegisterRepositories(IServiceCollection services)
    {
        // Register Entity Framework repositories as primary
        services.AddScoped<IUserRepository, EfUserRepository>();
        
        // Register Dapper repositories as additional options
        services.AddScoped<DapperUserRepository>();
        services.AddScoped<DapperTodoRepository>();
        services.AddScoped<DapperWeatherRepository>();
        
        // You can use both EF and Dapper repositories in your services
        // EF for complex operations, Dapper for high-performance queries
    }

    /// <summary>
    /// Initialize database (create database, run migrations, seed data)
    /// </summary>
    /// <param name="app">Application builder</param>
    /// <returns>Application builder</returns>
    public static async Task<WebApplication> InitializeDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<MaraDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<MaraDbContext>>();

        try
        {
            // Ensure database is created and apply migrations
            logger.LogInformation("Applying database migrations...");
            await context.Database.MigrateAsync();
            logger.LogInformation("Database migrations applied successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while applying database migrations.");
            throw;
        }

        return app;
    }
}
