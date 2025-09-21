using System.Security.Claims;
using System.Text;
using System.Globalization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using imas.ApiService.Modules;
using imas.ApiService.Modules.Weather;
using imas.ApiService.Infrastructure.Extensions;
// Clean Architecture Dependencies
using imas.ApiService.Application;
using imas.ApiService.Infrastructure;
using JwtOptions = imas.ServiceDefaults.Configuration.JwtOptions;

var builder = WebApplication.CreateBuilder(args);

// Configure culture settings to handle invariant mode
CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

// Add service defaults & Aspire components with authentication
builder.AddServiceDefaultsWithAuth();

// Add services to the container
builder.Services.AddProblemDetails();

// Add Controllers for Clean Architecture
builder.Services.AddControllers();

// Add Authorization
builder.Services.AddAuthorization();

// Configure Rate Limiting
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = 429;
    
    // Authentication endpoints - more restrictive
    options.AddFixedWindowLimiter("AuthPolicy", limiterOptions =>
    {
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.PermitLimit = 5; // 5 attempts per minute
    });
    
    // Todo endpoints - moderate limiting
    options.AddTokenBucketLimiter("TodosPolicy", limiterOptions =>
    {
        limiterOptions.TokenLimit = 100;
        limiterOptions.ReplenishmentPeriod = TimeSpan.FromMinutes(1);
        limiterOptions.TokensPerPeriod = 100;
    });
    
    // Weather endpoints - lenient limiting
    options.AddSlidingWindowLimiter("WeatherPolicy", limiterOptions =>
    {
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.PermitLimit = 200; // 200 requests per minute
        limiterOptions.SegmentsPerWindow = 4;
    });
});

// Add OpenAPI/Swagger support with JWT
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "IMAS API Gateway",
        Version = "v1",
        Description = "Integrated Management and Administration System - JWT Authentication, Rate Limiting & Modular Architecture"
    });

    // Custom schema ID selector to avoid conflicts between legacy and shared contracts
    options.CustomSchemaIds(type =>
    {
        if (type.FullName != null)
        {
            // Use different prefixes for legacy vs shared contracts
            if (type.FullName.StartsWith("imas.Shared.Contracts"))
            {
                return $"Shared{type.Name}";
            }
            else if (type.FullName.StartsWith("imas.ApiService.Modules"))
            {
                return $"Legacy{type.Name}";
            }
        }
        return type.Name;
    });

    // Add JWT authentication to Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Add Database Configuration (Entity Framework + Dapper)
builder.Services.AddDatabase(builder.Configuration);

// Clean Architecture Layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Register all modules (legacy support)
builder.Services.AddModules(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "MARA API v1");
        options.RoutePrefix = "swagger";
    });
}

// Add security middleware
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

// Health check endpoint (public)
app.MapGet("/health", () => new
{
    Status = "Healthy",
    Timestamp = DateTime.UtcNow,
    Version = "1.0.0",
    Environment = app.Environment.EnvironmentName
})
.WithName("HealthCheck")
.WithTags("Health")
.AllowAnonymous();

// API Info endpoint (public)
app.MapGet("/", () => new
{
    Name = "IMAS API",
    Version = "1.0.0",
    Description = "Integrated Management and Administration System - JWT Authentication, Rate Limiting & Modular Architecture",
    Endpoints = new
    {
        Health = "/health",
        Swagger = "/swagger",
        Authentication = "/auth/*",
        Todos = "/todos/*",
        Weather = "/weather/*",
        Gateway = "/api/gateway/*"
    },
    Documentation = "/swagger"
})
.WithName("ApiInfo")
.WithTags("Info")
.AllowAnonymous();

// Backward compatibility endpoints
app.MapGet("/weatherforecast", async (IWeatherService weatherService) =>
{
    // Redirect to the new weather endpoint for backward compatibility
    return await weatherService.GetForecastAsync();
})
.WithName("GetWeatherForecastLegacy")
.WithTags("Weather", "Legacy")
.AllowAnonymous();

// Register all module endpoints
app.MapModules();

// Map Clean Architecture Controllers
app.MapControllers();

// Aspire default endpoints (health, metrics, etc.)
app.MapDefaultEndpoints();

// Initialize database (apply migrations)
await app.InitializeDatabaseAsync();

app.Run();
