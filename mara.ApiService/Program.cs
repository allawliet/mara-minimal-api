using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using mara.ApiService.Configuration;
using mara.ApiService.Modules;
using mara.ApiService.Modules.Weather;
using mara.ApiService.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components
builder.AddServiceDefaults();

// Add services to the container
builder.Services.AddProblemDetails();

// Configure JWT Authentication
builder.Services.Configure<JwtOptions>(
    builder.Configuration.GetSection(JwtOptions.SectionName));

var jwtOptions = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
    ?? throw new InvalidOperationException("JWT configuration not found");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),
            ClockSkew = TimeSpan.Zero
        };
    });

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
        Title = "MARA API",
        Version = "v1",
        Description = "Minimal API with ASP.NET Aspire - JWT Authentication, Rate Limiting & Modular Architecture"
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

// Register all modules
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
    Name = "MARA API",
    Version = "1.0.0",
    Description = "Minimal API with ASP.NET Aspire - JWT Authentication, Rate Limiting & Modular Architecture",
    Endpoints = new
    {
        Health = "/health",
        Swagger = "/swagger",
        Authentication = "/auth/*",
        Todos = "/todos/*",
        Weather = "/weather/*"
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

// Aspire default endpoints (health, metrics, etc.)
app.MapDefaultEndpoints();

app.Run();
