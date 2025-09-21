using imas.ApiService.Application.Common;
using imas.ApiService.Application.Todos.DTOs;
using imas.ApiService.Domain.Common;
using imas.ApiService.Domain.Todos.Repositories;

namespace imas.ApiService.Application.Todos.Queries;

/// <summary>
/// Get todo by ID query
/// </summary>
public record GetTodoByIdQuery(int Id, UserId UserId) : IQuery<Result<TodoDto>>;

/// <summary>
/// Get todo by ID query handler
/// </summary>
public class GetTodoByIdQueryHandler : IQueryHandler<GetTodoByIdQuery, Result<TodoDto>>
{
    private readonly ITodoRepository _todoRepository;

    public GetTodoByIdQueryHandler(ITodoRepository todoRepository)
    {
        _todoRepository = todoRepository;
    }

    public async Task<Result<TodoDto>> HandleAsync(GetTodoByIdQuery request, CancellationToken cancellationToken = default)
    {
        try
        {
            var todo = await _todoRepository.GetByIdForUserAsync(request.Id, request.UserId, cancellationToken);

            if (todo == null)
                return Result.Failure<TodoDto>("Todo not found");

            var todoDto = MapToDto(todo);
            return Result.Success(todoDto);
        }
        catch (Exception ex)
        {
            return Result.Failure<TodoDto>($"Failed to get todo: {ex.Message}");
        }
    }

    private static TodoDto MapToDto(Domain.Todos.Entities.Todo todo)
    {
        return new TodoDto
        {
            Id = todo.Id,
            Title = todo.Title.Value,
            Description = todo.Description?.Value,
            IsCompleted = todo.IsCompleted,
            CompletedAt = todo.CompletedAt,
            CreatedAt = todo.CreatedAt,
            ModifiedAt = todo.ModifiedAt
        };
    }
}
