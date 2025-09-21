using imas.ApiService.Application.Common;
using imas.ApiService.Application.Todos.DTOs;
using imas.ApiService.Domain.Common;
using imas.ApiService.Domain.Todos.Repositories;

namespace imas.ApiService.Application.Todos.Commands;

/// <summary>
/// Delete todo command
/// </summary>
public record DeleteTodoCommand(int Id, string UserId) : ICommand<Result<bool>>;

/// <summary>
/// Delete todo command handler
/// </summary>
public class DeleteTodoCommandHandler : ICommandHandler<DeleteTodoCommand, Result<bool>>
{
    private readonly ITodoRepository _todoRepository;

    public DeleteTodoCommandHandler(ITodoRepository todoRepository)
    {
        _todoRepository = todoRepository;
    }

    public async Task<Result<bool>> HandleAsync(DeleteTodoCommand request, CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = UserId.Create(request.UserId);
            var todo = await _todoRepository.GetByIdForUserAsync(request.Id, userId, cancellationToken);

            if (todo == null)
                return Result.Failure<bool>("Todo not found or access denied");

            todo.Delete(userId);
            await _todoRepository.UpdateAsync(todo, cancellationToken);

            return Result.Success<bool>(true);
        }
        catch (Exception ex)
        {
            return Result.Failure<bool>($"Failed to delete todo: {ex.Message}");
        }
    }
}

/// <summary>
/// Complete todo command
/// </summary>
public record CompleteTodoCommand(int Id, string UserId) : ICommand<Result<TodoDto>>;

/// <summary>
/// Complete todo command handler
/// </summary>
public class CompleteTodoCommandHandler : ICommandHandler<CompleteTodoCommand, Result<TodoDto>>
{
    private readonly ITodoRepository _todoRepository;

    public CompleteTodoCommandHandler(ITodoRepository todoRepository)
    {
        _todoRepository = todoRepository;
    }

    public async Task<Result<TodoDto>> HandleAsync(CompleteTodoCommand request, CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = UserId.Create(request.UserId);
            var todo = await _todoRepository.GetByIdForUserAsync(request.Id, userId, cancellationToken);

            if (todo == null)
                return Result.Failure<TodoDto>("Todo not found or access denied");

            todo.Complete(userId);
            await _todoRepository.UpdateAsync(todo, cancellationToken);

            var todoDto = new TodoDto
            {
                Id = todo.Id,
                Title = todo.Title.Value,
                Description = todo.Description?.Value,
                IsCompleted = todo.IsCompleted,
                CompletedAt = todo.CompletedAt,
                CreatedAt = todo.CreatedAt,
                ModifiedAt = todo.ModifiedAt
            };

            return Result.Success<TodoDto>(todoDto);
        }
        catch (Exception ex)
        {
            return Result.Failure<TodoDto>($"Failed to complete todo: {ex.Message}");
        }
    }
}

/// <summary>
/// Reopen todo command
/// </summary>
public record ReopenTodoCommand(int Id, string UserId) : ICommand<Result<TodoDto>>;

/// <summary>
/// Reopen todo command handler
/// </summary>
public class ReopenTodoCommandHandler : ICommandHandler<ReopenTodoCommand, Result<TodoDto>>
{
    private readonly ITodoRepository _todoRepository;

    public ReopenTodoCommandHandler(ITodoRepository todoRepository)
    {
        _todoRepository = todoRepository;
    }

    public async Task<Result<TodoDto>> HandleAsync(ReopenTodoCommand request, CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = UserId.Create(request.UserId);
            var todo = await _todoRepository.GetByIdForUserAsync(request.Id, userId, cancellationToken);

            if (todo == null)
                return Result.Failure<TodoDto>("Todo not found or access denied");

            todo.Reopen(userId);
            await _todoRepository.UpdateAsync(todo, cancellationToken);

            var todoDto = new TodoDto
            {
                Id = todo.Id,
                Title = todo.Title.Value,
                Description = todo.Description?.Value,
                IsCompleted = todo.IsCompleted,
                CompletedAt = todo.CompletedAt,
                CreatedAt = todo.CreatedAt,
                ModifiedAt = todo.ModifiedAt
            };

            return Result.Success<TodoDto>(todoDto);
        }
        catch (Exception ex)
        {
            return Result.Failure<TodoDto>($"Failed to reopen todo: {ex.Message}");
        }
    }
}
