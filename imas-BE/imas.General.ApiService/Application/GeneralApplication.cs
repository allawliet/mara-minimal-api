namespace imas.General.ApiService.Application;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }

    public static ApiResponse<T> SuccessResult(T data, string message = "Operation successful")
    {
        return new ApiResponse<T> { Success = true, Message = message, Data = data };
    }

    public static ApiResponse<T> FailureResult(string message)
    {
        return new ApiResponse<T> { Success = false, Message = message };
    }
}

public class ApiResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;

    public static ApiResponse SuccessResult(string message = "Operation successful")
    {
        return new ApiResponse { Success = true, Message = message };
    }

    public static ApiResponse FailureResult(string message)
    {
        return new ApiResponse { Success = false, Message = message };
    }
}

public interface IGeneralService
{
    Task<ApiResponse<object>> GetDepartmentsAsync();
    Task<ApiResponse<object>> GetCompaniesAsync();
    Task<ApiResponse<object>> GetLocationsAsync();
}

public class GeneralService : IGeneralService
{
    public async Task<ApiResponse<object>> GetDepartmentsAsync()
    {
        var departments = new[] { new { Id = 1, Name = "IT", Description = "Information Technology" } };
        return ApiResponse<object>.SuccessResult(departments);
    }

    public async Task<ApiResponse<object>> GetCompaniesAsync()
    {
        var companies = new[] { new { Id = 1, Name = "ACME Corp", Industry = "Technology" } };
        return ApiResponse<object>.SuccessResult(companies);
    }

    public async Task<ApiResponse<object>> GetLocationsAsync()
    {
        var locations = new[] { new { Id = 1, Name = "Headquarters", City = "New York" } };
        return ApiResponse<object>.SuccessResult(locations);
    }
}

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IGeneralService, GeneralService>();
        return services;
    }
}