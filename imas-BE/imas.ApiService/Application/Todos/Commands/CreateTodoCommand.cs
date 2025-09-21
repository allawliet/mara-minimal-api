using imas.ApiService.Application.Common;
using imas.ApiService.Application.Todos.DTOs;
using imas.ApiService.Domain.Common;
using imas.ApiService.Domain.Todos.Entities;
using imas.ApiService.Domain.Todos.Repositories;
using imas.ApiService.Domain.Todos.ValueObjects;

namespace imas.ApiService.Application.Todos.Commands;

/// <summary>
/// Create todo command
/// </summary>
public record CreateTodoCommand(UserId UserId, CreateTodoDto CreateTodoDto) : ICommand<Result<TodoDto>>;

/// <summary>
/// Create todo command handler
/// </summary>
public class CreateTodoCommandHandler : ICommandHandler<CreateTodoCommand, Result<TodoDto>>
{
    private readonly ITodoRepository _todoRepository;

    public CreateTodoCommandHandler(ITodoRepository todoRepository)
    {
        _todoRepository = todoRepository;
    }

    public async Task<Result<TodoDto>> HandleAsync(CreateTodoCommand request, CancellationToken cancellationToken = default)
    {
        try
        {
            var title = TodoTitle.Create(request.CreateTodoDto.Title);
            var description = string.IsNullOrEmpty(request.CreateTodoDto.Description) 
                ? null 
                : TodoDescription.Create(request.CreateTodoDto.Description);
            var userId = request.UserId;

            var todo = Todo.Create(title, description, userId);
            
            await _todoRepository.AddAsync(todo, cancellationToken);

            var todoDto = MapToDto(todo);
            return Result.Success(todoDto);
        }
        catch (ArgumentException ex)
        {
            return Result.Failure<TodoDto>(ex.Message);
        }
        catch (Exception ex)
        {
            return Result.Failure<TodoDto>($"Failed to create todo: {ex.Message}");
        }
    }

    private static TodoDto MapToDto(Todo todo)
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
