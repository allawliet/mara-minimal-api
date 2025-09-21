using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using imas.HR.ApiService.Application;
using imas.HR.ApiService.Infrastructure;
using imas.HR.ApiService.Infrastructure.Extensions;
using imas.HR.ApiService.Presentation.Endpoints;
using imas.HR.ApiService.Services;
using imas.HR.ApiService.Data;
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

// Add independent HR Service
builder.Services.AddScoped<IHRService, HRService>();

// Add Clean Architecture layers (keeping for backward compatibility)
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Add OpenAPI/Swagger support with JWT
builder.Services.AddSwaggerWithJwt("IMAS HR API", "v1");

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
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "IMAS HR API v1");
        c.RoutePrefix = "";
    });
}

// Map default endpoints
app.MapDefaultEndpoints();

// Test endpoint (simple)
app.MapGet("/test", () => "HR Service is working!")
.WithName("TestEndpoint")
.WithTags("Test");

// Simple employees test endpoint
app.MapGet("/api/employees/test", () => new { Message = "Employees endpoint working", Count = 0 })
.WithName("TestEmployees")
.WithTags("Employees");

// Map independent HR API endpoints (new clean endpoints)
app.MapIndependentHREndpoints();

// Map legacy Clean Architecture endpoints (keeping for now)
app.MapEmployeeEndpoints();
app.MapDepartmentEndpoints();

// Health check endpoint
app.MapGet("/health", () => new
{
    Service = "HR",
    Status = "Healthy",
    Timestamp = DateTime.UtcNow,
    Version = "1.0.0"
})
.WithName("HealthCheck")
.WithTags("Health");

app.Run();