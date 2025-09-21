using Microsoft.Extensions.DependencyInjection;
using imas.HR.ApiService.Application.Employees;

namespace imas.HR.ApiService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register Application Services
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IDepartmentService, DepartmentService>();
        
        return services;
    }
}