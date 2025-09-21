using Microsoft.EntityFrameworkCore;
using imas.HR.ApiService.Data;

namespace imas.HR.ApiService.Infrastructure.Extensions;

/// <summary>
/// Database configuration extensions for HR service
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

        services.AddDbContext<HRDbContext>(options =>
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null);
            }));

        return services;
    }

    /// <summary>
    /// Initialize database (create database, run migrations, seed data)
    /// </summary>
    /// <param name="app">Application builder</param>
    /// <returns>Application builder</returns>
    public static async Task<WebApplication> InitializeDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<HRDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<HRDbContext>>();

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