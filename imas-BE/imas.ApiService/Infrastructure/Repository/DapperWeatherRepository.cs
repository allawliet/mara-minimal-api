using Dapper;
using imas.ApiService.Infrastructure.Data;
using imas.ApiService.Infrastructure.Repository;
using imas.ApiService.Modules.Weather;

namespace imas.ApiService.Infrastructure.Repository;

/// <summary>
/// Dapper-based Weather repository with weather-specific queries
/// </summary>
public class DapperWeatherRepository : DapperRepository<WeatherForecast, string>
{
    private readonly IDbConnectionFactory _connectionFactory;

    public DapperWeatherRepository(IDbConnectionFactory connectionFactory) 
        : base(connectionFactory, "WeatherForecasts")
    {
        _connectionFactory = connectionFactory;
    }

    /// <summary>
    /// Get weather forecasts by location
    /// </summary>
    public async Task<IEnumerable<WeatherForecast>> GetByLocationAsync(string location, int limit = 10, CancellationToken cancellationToken = default)
    {
        var sql = @"
            SELECT TOP (@Limit) * 
            FROM WeatherForecasts 
            WHERE Location = @Location 
            ORDER BY Date ASC";
        
        return await QueryAsync(sql, new { Location = location, Limit = limit }, cancellationToken);
    }

    /// <summary>
    /// Get weather forecasts by date range
    /// </summary>
    public async Task<IEnumerable<WeatherForecast>> GetByDateRangeAsync(DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken = default)
    {
        var sql = @"
            SELECT * 
            FROM WeatherForecasts 
            WHERE Date >= @StartDate AND Date <= @EndDate 
            ORDER BY Date ASC, Location";
        
        return await QueryAsync(sql, new { StartDate = startDate, EndDate = endDate }, cancellationToken);
    }

    /// <summary>
    /// Get weather forecasts by location and date range
    /// </summary>
    public async Task<IEnumerable<WeatherForecast>> GetByLocationAndDateRangeAsync(
        string location, 
        DateOnly startDate, 
        DateOnly endDate, 
        CancellationToken cancellationToken = default)
    {
        var sql = @"
            SELECT * 
            FROM WeatherForecasts 
            WHERE Location = @Location 
                AND Date >= @StartDate 
                AND Date <= @EndDate 
            ORDER BY Date ASC";
        
        return await QueryAsync(sql, new { Location = location, StartDate = startDate, EndDate = endDate }, cancellationToken);
    }

    /// <summary>
    /// Get weather forecasts by temperature range
    /// </summary>
    public async Task<IEnumerable<WeatherForecast>> GetByTemperatureRangeAsync(
        int minTemp, 
        int maxTemp, 
        CancellationToken cancellationToken = default)
    {
        var sql = @"
            SELECT * 
            FROM WeatherForecasts 
            WHERE TemperatureC >= @MinTemp AND TemperatureC <= @MaxTemp 
            ORDER BY Date ASC";
        
        return await QueryAsync(sql, new { MinTemp = minTemp, MaxTemp = maxTemp }, cancellationToken);
    }

    /// <summary>
    /// Get all unique locations
    /// </summary>
    public async Task<IEnumerable<string>> GetUniqueLocationsAsync(CancellationToken cancellationToken = default)
    {
        var sql = "SELECT DISTINCT Location FROM WeatherForecasts ORDER BY Location";
        
        using var connection = await _connectionFactory.CreateConnectionAsync();
        return await connection.QueryAsync<string>(sql);
    }

    /// <summary>
    /// Get weather summary by location
    /// </summary>
    public async Task<WeatherSummary?> GetWeatherSummaryByLocationAsync(string location, CancellationToken cancellationToken = default)
    {
        var sql = @"
            SELECT 
                Location,
                COUNT(*) as TotalForecasts,
                AVG(CAST(TemperatureC as FLOAT)) as AverageTemperature,
                MIN(TemperatureC) as MinTemperature,
                MAX(TemperatureC) as MaxTemperature,
                MIN(Date) as EarliestDate,
                MAX(Date) as LatestDate
            FROM WeatherForecasts 
            WHERE Location = @Location
            GROUP BY Location";
        
        using var connection = await _connectionFactory.CreateConnectionAsync();
        return await connection.QuerySingleOrDefaultAsync<WeatherSummary>(sql, new { Location = location });
    }

    /// <summary>
    /// Clean up old weather forecasts (older than specified days)
    /// </summary>
    public async Task<int> CleanupOldForecastsAsync(int daysOld = 30, CancellationToken cancellationToken = default)
    {
        var sql = "DELETE FROM WeatherForecasts WHERE Date < @CutoffDate";
        var cutoffDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-daysOld));
        
        using var connection = await _connectionFactory.CreateConnectionAsync();
        return await connection.ExecuteAsync(sql, new { CutoffDate = cutoffDate });
    }
}

/// <summary>
/// Weather summary model for aggregated data
/// </summary>
public class WeatherSummary
{
    public string Location { get; set; } = string.Empty;
    public int TotalForecasts { get; set; }
    public double AverageTemperature { get; set; }
    public int MinTemperature { get; set; }
    public int MaxTemperature { get; set; }
    public DateOnly EarliestDate { get; set; }
    public DateOnly LatestDate { get; set; }
}
