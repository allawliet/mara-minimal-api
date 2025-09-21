using imas.ApiService.Application.Common;
using imas.ApiService.Application.Todos.DTOs;
using imas.ApiService.Domain.Common;
using imas.ApiService.Domain.Todos.Repositories;

namespace imas.ApiService.Application.Todos.Queries;

/// <summary>
/// Get all todos query
/// </summary>
public record GetAllTodosQuery(string UserId) : IQuery<Result<IEnumerable<TodoDto>>>;

/// <summary>
/// Get all todos query handler
/// </summary>
public class GetAllTodosQueryHandler : IQueryHandler<GetAllTodosQuery, Result<IEnumerable<TodoDto>>>
{
    private readonly ITodoRepository _todoRepository;

    public GetAllTodosQueryHandler(ITodoRepository todoRepository)
    {
        _todoRepository = todoRepository;
    }

    public async Task<Result<IEnumerable<TodoDto>>> HandleAsync(GetAllTodosQuery request, CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = UserId.Create(request.UserId);
            var todos = await _todoRepository.GetAllForUserAsync(userId, cancellationToken);

            var todoDtos = todos.Select(MapToDto);
            return Result.Success(todoDtos);
        }
        catch (Exception ex)
        {
            return Result.Failure<IEnumerable<TodoDto>>($"Failed to get todos: {ex.Message}");
        }
    }

    private static TodoDto MapToDto(Domain.Todos.Entities.Todo todo)
    {
        return new TodoDto
        {
            Id = todo.Id,
            Title = todo.Title,
            Description = todo.Description,
            IsCompleted = todo.IsCompleted,
            CompletedAt = todo.CompletedAt,
            CreatedAt = todo.CreatedAt,
            ModifiedAt = todo.ModifiedAt
        };
    }
}

/// <summary>
/// Get paged todos query
/// </summary>
public record GetPagedTodosQuery(string UserId, int Page, int PageSize) : IQuery<Result<PagedResult<TodoDto>>>;

/// <summary>
/// Get paged todos query handler
/// </summary>
public class GetPagedTodosQueryHandler : IQueryHandler<GetPagedTodosQuery, Result<PagedResult<TodoDto>>>
{
    private readonly ITodoRepository _todoRepository;

    public GetPagedTodosQueryHandler(ITodoRepository todoRepository)
    {
        _todoRepository = todoRepository;
    }

    public async Task<Result<PagedResult<TodoDto>>> HandleAsync(GetPagedTodosQuery request, CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = UserId.Create(request.UserId);
            var (todos, totalCount) = await _todoRepository.GetPagedForUserAsync(
                userId, request.Page, request.PageSize, cancellationToken);

            var todoDtos = todos.Select(MapToDto);
            var pagedResult = new PagedResult<TodoDto>(todoDtos, totalCount, request.Page, request.PageSize);
            
            return Result.Success(pagedResult);
        }
        catch (Exception ex)
        {
            return Result.Failure<PagedResult<TodoDto>>($"Failed to get paged todos: {ex.Message}");
        }
    }

    private static TodoDto MapToDto(Domain.Todos.Entities.Todo todo)
    {
        return new TodoDto
        {
            Id = todo.Id,
            Title = todo.Title,
            Description = todo.Description,
            IsCompleted = todo.IsCompleted,
            CompletedAt = todo.CompletedAt,
            CreatedAt = todo.CreatedAt,
            ModifiedAt = todo.ModifiedAt
        };
    }
}

/// <summary>
/// Get completed todos query
/// </summary>
public record GetCompletedTodosQuery(string UserId) : IQuery<Result<IEnumerable<TodoDto>>>;

/// <summary>
/// Get completed todos query handler
/// </summary>
public class GetCompletedTodosQueryHandler : IQueryHandler<GetCompletedTodosQuery, Result<IEnumerable<TodoDto>>>
{
    private readonly ITodoRepository _todoRepository;

    public GetCompletedTodosQueryHandler(ITodoRepository todoRepository)
    {
        _todoRepository = todoRepository;
    }

    public async Task<Result<IEnumerable<TodoDto>>> HandleAsync(GetCompletedTodosQuery request, CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = UserId.Create(request.UserId);
            var todos = await _todoRepository.GetCompletedForUserAsync(userId, cancellationToken);

            var todoDtos = todos.Select(MapToDto);
            return Result.Success(todoDtos);
        }
        catch (Exception ex)
        {
            return Result.Failure<IEnumerable<TodoDto>>($"Failed to get completed todos: {ex.Message}");
        }
    }

    private static TodoDto MapToDto(Domain.Todos.Entities.Todo todo)
    {
        return new TodoDto
        {
            Id = todo.Id,
            Title = todo.Title,
            Description = todo.Description,
            IsCompleted = todo.IsCompleted,
            CompletedAt = todo.CompletedAt,
            CreatedAt = todo.CreatedAt,
            ModifiedAt = todo.ModifiedAt
        };
    }
}

/// <summary>
/// Get pending todos query
/// </summary>
public record GetPendingTodosQuery(string UserId) : IQuery<Result<IEnumerable<TodoDto>>>;

/// <summary>
/// Get pending todos query handler
/// </summary>
public class GetPendingTodosQueryHandler : IQueryHandler<GetPendingTodosQuery, Result<IEnumerable<TodoDto>>>
{
    private readonly ITodoRepository _todoRepository;

    public GetPendingTodosQueryHandler(ITodoRepository todoRepository)
    {
        _todoRepository = todoRepository;
    }

    public async Task<Result<IEnumerable<TodoDto>>> HandleAsync(GetPendingTodosQuery request, CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = UserId.Create(request.UserId);
            var todos = await _todoRepository.GetPendingForUserAsync(userId, cancellationToken);

            var todoDtos = todos.Select(MapToDto);
            return Result.Success(todoDtos);
        }
        catch (Exception ex)
        {
            return Result.Failure<IEnumerable<TodoDto>>($"Failed to get pending todos: {ex.Message}");
        }
    }

    private static TodoDto MapToDto(Domain.Todos.Entities.Todo todo)
    {
        return new TodoDto
        {
            Id = todo.Id,
            Title = todo.Title,
            Description = todo.Description,
            IsCompleted = todo.IsCompleted,
            CompletedAt = todo.CompletedAt,
            CreatedAt = todo.CreatedAt,
            ModifiedAt = todo.ModifiedAt
        };
    }
}

/// <summary>
/// Get todo statistics query
/// </summary>
public record GetTodoStatsQuery(string UserId) : IQuery<Result<TodoStatsDto>>;

/// <summary>
/// Get todo statistics query handler
/// </summary>
public class GetTodoStatsQueryHandler : IQueryHandler<GetTodoStatsQuery, Result<TodoStatsDto>>
{
    private readonly ITodoRepository _todoRepository;

    public GetTodoStatsQueryHandler(ITodoRepository todoRepository)
    {
        _todoRepository = todoRepository;
    }

    public async Task<Result<TodoStatsDto>> HandleAsync(GetTodoStatsQuery request, CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = UserId.Create(request.UserId);
            
            var totalCount = await _todoRepository.CountForUserAsync(userId, cancellationToken);
            var completedCount = await _todoRepository.CountCompletedForUserAsync(userId, cancellationToken);
            var pendingCount = await _todoRepository.CountPendingForUserAsync(userId, cancellationToken);
            
            var completionRate = totalCount > 0 ? (double)completedCount / totalCount * 100 : 0;

            var stats = new TodoStatsDto
            {
                TotalCount = totalCount,
                CompletedCount = completedCount,
                PendingCount = pendingCount,
                CompletionRate = Math.Round(completionRate, 2)
            };

            return Result.Success(stats);
        }
        catch (Exception ex)
        {
            return Result.Failure<TodoStatsDto>($"Failed to get todo statistics: {ex.Message}");
        }
    }
}
