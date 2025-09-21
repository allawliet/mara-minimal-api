using Microsoft.Extensions.DependencyInjection;
using imas.ApiService.Application.Common;
using imas.ApiService.Application.Todos.Commands;
using imas.ApiService.Application.Todos.DTOs;
using imas.ApiService.Application.Todos.Queries;

namespace imas.ApiService.Application;

/// <summary>
/// Dependency injection configuration for Application layer
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Command Handlers
        services.AddScoped<IRequestHandler<CreateTodoCommand, Result<TodoDto>>, CreateTodoCommandHandler>();
        services.AddScoped<IRequestHandler<UpdateTodoCommand, Result<TodoDto>>, UpdateTodoCommandHandler>();
        services.AddScoped<IRequestHandler<DeleteTodoCommand, Result<bool>>, DeleteTodoCommandHandler>();
        services.AddScoped<IRequestHandler<CompleteTodoCommand, Result<TodoDto>>, CompleteTodoCommandHandler>();
        services.AddScoped<IRequestHandler<ReopenTodoCommand, Result<TodoDto>>, ReopenTodoCommandHandler>();

        // Query Handlers
        services.AddScoped<IRequestHandler<GetTodoByIdQuery, Result<TodoDto>>, GetTodoByIdQueryHandler>();
        services.AddScoped<IRequestHandler<GetAllTodosQuery, Result<IEnumerable<TodoDto>>>, GetAllTodosQueryHandler>();
        services.AddScoped<IRequestHandler<GetPagedTodosQuery, Result<PagedResult<TodoDto>>>, GetPagedTodosQueryHandler>();
        services.AddScoped<IRequestHandler<GetCompletedTodosQuery, Result<IEnumerable<TodoDto>>>, GetCompletedTodosQueryHandler>();
        services.AddScoped<IRequestHandler<GetPendingTodosQuery, Result<IEnumerable<TodoDto>>>, GetPendingTodosQueryHandler>();
        services.AddScoped<IRequestHandler<GetTodoStatsQuery, Result<TodoStatsDto>>, GetTodoStatsQueryHandler>();

        return services;
    }
}
