namespace mara.ApiService.Modules;

public interface IModule
{
    void RegisterServices(IServiceCollection services, IConfiguration configuration);
    void RegisterEndpoints(IEndpointRouteBuilder endpoints);
}
