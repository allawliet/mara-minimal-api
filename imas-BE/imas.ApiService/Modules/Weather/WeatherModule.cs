using Microsoft.AspNetCore.RateLimiting;
using imas.ApiService.Modules;
using imas.ApiService.Infrastructure.Models;

namespace imas.ApiService.Modules.Weather;

// Entity (inherits from BaseEntity for CRUD operations)
public class WeatherForecast : BaseEntity<string>
{
    public DateOnly Date { get; set; }
    public int TemperatureC { get; set; }
    public string? Summary { get; set; }
    public string Location { get; set; } = "Unknown";
    
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

// Request/Response DTOs
public record CreateWeatherForecastRequest(DateOnly Date, int TemperatureC, string? Summary, string Location = "Unknown");
public record UpdateWeatherForecastRequest(DateOnly Date, int TemperatureC, string? Summary, string Location);

// Enhanced service interface with CRUD operations and weather-specific methods
public interface IWeatherService
{
    // Standard CRUD operations
    Task<IEnumerable<WeatherForecast>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<WeatherForecast?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<WeatherForecast> CreateAsync(CreateWeatherForecastRequest request, CancellationToken cancellationToken = default);
    Task<WeatherForecast?> UpdateAsync(string id, UpdateWeatherForecastRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default);
    
    // Legacy method for compatibility
    Task<IEnumerable<WeatherForecast>> GetForecastAsync(int days = 5);
    
    // Additional weather-specific methods
    Task<IEnumerable<WeatherForecast>> GetForecastByLocationAsync(string location, int days = 5);
    Task<IEnumerable<WeatherForecast>> GetForecastByDateRangeAsync(DateOnly startDate, DateOnly endDate);
}

public class WeatherService : IWeatherService
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };
    
    private readonly List<WeatherForecast> _forecasts = new();
    private static int _idCounter = 1;

    public WeatherService()
    {
        // Initialize with some sample data
        InitializeSampleData();
    }

    // Standard CRUD operations
    public Task<IEnumerable<WeatherForecast>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<WeatherForecast>>(_forecasts);
    }

    public Task<WeatherForecast?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var forecast = _forecasts.FirstOrDefault(f => f.Id == id);
        return Task.FromResult(forecast);
    }

    public Task<WeatherForecast> CreateAsync(CreateWeatherForecastRequest request, CancellationToken cancellationToken = default)
    {
        var forecast = new WeatherForecast
        {
            Id = $"weather_{_idCounter++}_{request.Date:yyyyMMdd}",
            Date = request.Date,
            TemperatureC = request.TemperatureC,
            Summary = request.Summary,
            Location = request.Location
        };
        
        _forecasts.Add(forecast);
        return Task.FromResult(forecast);
    }

    public Task<WeatherForecast?> UpdateAsync(string id, UpdateWeatherForecastRequest request, CancellationToken cancellationToken = default)
    {
        var forecast = _forecasts.FirstOrDefault(f => f.Id == id);
        if (forecast == null)
            return Task.FromResult<WeatherForecast?>(null);

        forecast.Date = request.Date;
        forecast.TemperatureC = request.TemperatureC;
        forecast.Summary = request.Summary;
        forecast.Location = request.Location;

        return Task.FromResult<WeatherForecast?>(forecast);
    }

    public Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var forecast = _forecasts.FirstOrDefault(f => f.Id == id);
        if (forecast == null)
            return Task.FromResult(false);

        _forecasts.Remove(forecast);
        return Task.FromResult(true);
    }

    // Legacy method for compatibility
    public Task<IEnumerable<WeatherForecast>> GetForecastAsync(int days = 5)
    {
        var forecast = Enumerable.Range(1, days).Select(index =>
            new WeatherForecast
            {
                Id = $"generated_{index}_{DateTime.Now:yyyyMMddHHmm}",
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)],
                Location = "Auto-Generated"
            });

        return Task.FromResult(forecast);
    }

    // Weather-specific methods
    public Task<IEnumerable<WeatherForecast>> GetForecastByLocationAsync(string location, int days = 5)
    {
        var locationForecasts = _forecasts.Where(f => f.Location.Equals(location, StringComparison.OrdinalIgnoreCase))
                                         .OrderBy(f => f.Date)
                                         .Take(days);
        return Task.FromResult(locationForecasts);
    }

    public Task<IEnumerable<WeatherForecast>> GetForecastByDateRangeAsync(DateOnly startDate, DateOnly endDate)
    {
        var rangeForecasts = _forecasts.Where(f => f.Date >= startDate && f.Date <= endDate)
                                      .OrderBy(f => f.Date)
                                      .AsEnumerable();
        return Task.FromResult(rangeForecasts);
    }

    private void InitializeSampleData()
    {
        var cities = new[] { "New York", "London", "Tokyo", "Sydney", "Berlin" };
        
        for (int i = 0; i < 10; i++)
        {
            foreach (var city in cities)
            {
                _forecasts.Add(new WeatherForecast
                {
                    Id = $"sample_{city.ToLower()}_{i}",
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(i)),
                    TemperatureC = Random.Shared.Next(-10, 35),
                    Summary = Summaries[Random.Shared.Next(Summaries.Length)],
                    Location = city
                });
            }
        }
    }
}

public class WeatherModule : IModule
{
    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IWeatherService, WeatherService>();
    }

    public void RegisterEndpoints(IEndpointRouteBuilder endpoints)
    {
        var weatherGroup = endpoints.MapGroup("/weather")
            .WithTags("Weather")
            .RequireRateLimiting("WeatherPolicy");

        // CRUD endpoints
        weatherGroup.MapGet("/", GetAllWeatherForecastsAsync)
            .WithName("GetAllWeatherForecasts")
            .WithSummary("Get all weather forecasts")
            .AllowAnonymous();

        weatherGroup.MapGet("/{id}", GetWeatherForecastByIdAsync)
            .WithName("GetWeatherForecastById")
            .WithSummary("Get weather forecast by ID")
            .AllowAnonymous();

        weatherGroup.MapPost("/", CreateWeatherForecastAsync)
            .WithName("CreateWeatherForecast")
            .WithSummary("Create weather forecast")
            .RequireAuthorization();

        weatherGroup.MapPut("/{id}", UpdateWeatherForecastAsync)
            .WithName("UpdateWeatherForecast")
            .WithSummary("Update weather forecast")
            .RequireAuthorization();

        weatherGroup.MapDelete("/{id}", DeleteWeatherForecastAsync)
            .WithName("DeleteWeatherForecast")
            .WithSummary("Delete weather forecast")
            .RequireAuthorization();

        // Legacy and special endpoints
        weatherGroup.MapGet("/forecast", GetWeatherForecastAsync)
            .WithName("GetWeatherForecast")
            .WithSummary("Get generated weather forecast")
            .AllowAnonymous();

        weatherGroup.MapGet("/forecast/{days:int}", GetCustomWeatherForecastAsync)
            .WithName("GetCustomWeatherForecast")
            .WithSummary("Get custom days weather forecast")
            .AllowAnonymous();

        weatherGroup.MapGet("/location/{location}", GetWeatherForecastByLocationAsync)
            .WithName("GetWeatherForecastByLocation")
            .WithSummary("Get weather forecast by location")
            .AllowAnonymous();

        weatherGroup.MapGet("/range", GetWeatherForecastByDateRangeAsync)
            .WithName("GetWeatherForecastByDateRange")
            .WithSummary("Get weather forecast by date range")
            .AllowAnonymous();
    }

    // CRUD endpoints implementation
    private static async Task<IResult> GetAllWeatherForecastsAsync(IWeatherService weatherService)
    {
        var forecasts = await weatherService.GetAllAsync();
        return Results.Ok(forecasts);
    }

    private static async Task<IResult> GetWeatherForecastByIdAsync(string id, IWeatherService weatherService)
    {
        var forecast = await weatherService.GetByIdAsync(id);
        return forecast is not null ? Results.Ok(forecast) : Results.NotFound();
    }

    private static async Task<IResult> CreateWeatherForecastAsync(CreateWeatherForecastRequest request, IWeatherService weatherService)
    {
        var forecast = await weatherService.CreateAsync(request);
        return Results.Created($"/weather/{forecast.Id}", forecast);
    }

    private static async Task<IResult> UpdateWeatherForecastAsync(string id, UpdateWeatherForecastRequest request, IWeatherService weatherService)
    {
        var forecast = await weatherService.UpdateAsync(id, request);
        return forecast is not null ? Results.Ok(forecast) : Results.NotFound();
    }

    private static async Task<IResult> DeleteWeatherForecastAsync(string id, IWeatherService weatherService)
    {
        var success = await weatherService.DeleteAsync(id);
        return success ? Results.NoContent() : Results.NotFound();
    }

    // Legacy endpoints
    private static async Task<IResult> GetWeatherForecastAsync(IWeatherService weatherService)
    {
        var forecast = await weatherService.GetForecastAsync();
        return Results.Ok(forecast);
    }

    private static async Task<IResult> GetCustomWeatherForecastAsync(int days, IWeatherService weatherService)
    {
        if (days < 1 || days > 30)
        {
            return Results.BadRequest("Days must be between 1 and 30");
        }

        var forecast = await weatherService.GetForecastAsync(days);
        return Results.Ok(forecast);
    }

    // Weather-specific endpoints
    private static async Task<IResult> GetWeatherForecastByLocationAsync(string location, IWeatherService weatherService, int days = 5)
    {
        var forecasts = await weatherService.GetForecastByLocationAsync(location, days);
        return Results.Ok(forecasts);
    }

    private static async Task<IResult> GetWeatherForecastByDateRangeAsync(DateOnly startDate, DateOnly endDate, IWeatherService weatherService)
    {
        var forecasts = await weatherService.GetForecastByDateRangeAsync(startDate, endDate);
        return Results.Ok(forecasts);
    }
}
