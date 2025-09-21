using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using imas.Finance.ApiService.Application;
using imas.Finance.ApiService.Infrastructure;
using imas.Finance.ApiService.Infrastructure.Data;
using imas.Finance.ApiService.Infrastructure.Extensions;
using imas.Finance.ApiService.Presentation.Endpoints;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Configure culture settings to handle invariant mode
CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

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
builder.Services.AddSwaggerWithJwt("IMAS Finance API", "v1");

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
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "IMAS Finance API v1");
        c.RoutePrefix = "";
    });
}

// Map default endpoints
app.MapDefaultEndpoints();

// Map Finance API endpoints
app.MapInvoiceEndpoints();
app.MapPaymentEndpoints();

// Health check endpoint
app.MapGet("/health", () => new
{
    Service = "Finance",
    Status = "Healthy",
    Timestamp = DateTime.UtcNow,
    Version = "1.0.0"
})
.WithName("HealthCheck")
.WithTags("Health");

app.Run();