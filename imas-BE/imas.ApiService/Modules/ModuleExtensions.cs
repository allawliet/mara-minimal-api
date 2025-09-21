using imas.ApiService.Modules.Authentication;
using imas.ApiService.Modules.Todos;
using imas.ApiService.Modules.Weather;

namespace imas.ApiService.Modules;

public static class ModuleExtensions
{
    public static IServiceCollection AddModules(this IServiceCollection services, IConfiguration configuration)
    {
        var modules = GetModules();
        
        foreach (var module in modules)
        {
            module.RegisterServices(services, configuration);
        }
        
        return services;
    }
    
    public static IEndpointRouteBuilder MapModules(this IEndpointRouteBuilder endpoints)
    {
        var modules = GetModules();
        
        foreach (var module in modules)
        {
            module.RegisterEndpoints(endpoints);
        }
        
        return endpoints;
    }
    
    private static List<IModule> GetModules()
    {
        return new List<IModule>
        {
            new AuthenticationModule(),
            new TodosModule(),
            new WeatherModule()
        };
    }
}
