using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using imas.ApiService.Application.Common;
using imas.ApiService.Application.Todos.EventHandlers;
using imas.ApiService.Domain.Todos.Repositories;
using imas.ApiService.Infrastructure.Compatibility;
using imas.ApiService.Infrastructure.Data;
using imas.ApiService.Infrastructure.Persistence;
using imas.ApiService.Infrastructure.Persistence.Repositories;
using imas.ApiService.Infrastructure.Services;
using imas.ApiService.Modules.Todos;

namespace imas.ApiService.Infrastructure;

/// <summary>
/// Dependency injection configuration for Infrastructure layer
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database
        services.AddDbContext<ImasDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Repositories
        services.AddScoped<ITodoRepository, TodoRepository>();

        // Domain Event Infrastructure
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        // Domain Event Handlers
        services.AddScoped<IDomainEventHandler<Domain.Todos.Events.TodoCreated>, TodoCreatedEventHandler>();
        services.AddScoped<IDomainEventHandler<Domain.Todos.Events.TodoCompleted>, TodoCompletedEventHandler>();
        services.AddScoped<IDomainEventHandler<Domain.Todos.Events.TodoUpdated>, TodoUpdatedEventHandler>();
        services.AddScoped<IDomainEventHandler<Domain.Todos.Events.TodoDeleted>, TodoDeletedEventHandler>();
        services.AddScoped<IDomainEventHandler<Domain.Todos.Events.TodoReopened>, TodoReopenedEventHandler>();

        // Legacy Compatibility Layer
        services.AddScoped<ITodoService, LegacyTodoService>();

        return services;
    }
}
