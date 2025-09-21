using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using imas.Assets.ApiService.Application;
using imas.Assets.ApiService.Infrastructure;
using imas.Assets.ApiService.Infrastructure.Data;
using imas.Assets.ApiService.Infrastructure.Extensions;
using imas.Assets.ApiService.Presentation.Endpoints;
using System.Globalization;
using System.Threading;

// Set environment variable to force invariant globalization
Environment.SetEnvironmentVariable("DOTNET_SYSTEM_GLOBALIZATION_INVARIANT", "false");

// Comprehensive culture configuration
var invariantCulture = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentCulture = invariantCulture;
CultureInfo.DefaultThreadCurrentUICulture = invariantCulture;
Thread.CurrentThread.CurrentCulture = invariantCulture;
Thread.CurrentThread.CurrentUICulture = invariantCulture;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components with authentication
builder.AddServiceDefaultsWithAuth();

// Add services to the container
builder.Services.AddProblemDetails();

// Add Database Configuration (Entity Framework)
builder.Services.AddDatabase(builder.Configuration);

// Add clean architecture layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Add OpenAPI/Swagger support with JWT
builder.Services.AddSwaggerWithJwt("IMAS Assets API", "v1");

var app = builder.Build();

// Initialize database
await app.InitializeDatabaseAsync();

// Configure the HTTP request pipeline
app.UseExceptionHandler();

// Add authentication middleware
app.UseJwtAuthentication();

// Add OpenAPI middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "IMAS Assets API v1");
        c.RoutePrefix = "";
    });
}

// Map default endpoints
app.MapDefaultEndpoints();

// Map Assets API endpoints
app.MapAssetEndpoints();
app.MapAssetCategoryEndpoints();

// Health check endpoint
app.MapGet("/health", () => new
{
    Service = "Assets",
    Status = "Healthy",
    Timestamp = DateTime.UtcNow,
    Version = "1.0.0"
})
.WithName("HealthCheck")
.WithTags("Health");

app.Run();