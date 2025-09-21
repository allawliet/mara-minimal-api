using imas.ApiService.Application.Common;
using imas.ApiService.Application.Todos.DTOs;
using imas.ApiService.Domain.Common;
using imas.ApiService.Domain.Todos.Repositories;
using imas.ApiService.Domain.Todos.ValueObjects;

namespace imas.ApiService.Application.Todos.Commands;

/// <summary>
/// Update todo command
/// </summary>
public record UpdateTodoCommand(int Id, UserId UserId, UpdateTodoDto UpdateTodoDto) : ICommand<Result<TodoDto>>;

/// <summary>
/// Update todo command handler
/// </summary>
public class UpdateTodoCommandHandler : ICommandHandler<UpdateTodoCommand, Result<TodoDto>>
{
    private readonly ITodoRepository _todoRepository;

    public UpdateTodoCommandHandler(ITodoRepository todoRepository)
    {
        _todoRepository = todoRepository;
    }

    public async Task<Result<TodoDto>> HandleAsync(UpdateTodoCommand request, CancellationToken cancellationToken = default)
    {
        try
        {
            var todo = await _todoRepository.GetByIdForUserAsync(request.Id, request.UserId, cancellationToken);

            if (todo == null)
                return Result.Failure<TodoDto>("Todo not found or access denied");

            var title = TodoTitle.Create(request.UpdateTodoDto.Title);
            var description = string.IsNullOrEmpty(request.UpdateTodoDto.Description) 
                ? null 
                : TodoDescription.Create(request.UpdateTodoDto.Description);

            // Update basic properties
            todo.Update(title, description, request.UserId);

            await _todoRepository.UpdateAsync(todo, cancellationToken);

            var todoDto = MapToDto(todo);
            return Result.Success(todoDto);
        }
        catch (ArgumentException ex)
        {
            return Result.Failure<TodoDto>(ex.Message);
        }
        catch (Exception ex)
        {
            return Result.Failure<TodoDto>($"Failed to update todo: {ex.Message}");
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
