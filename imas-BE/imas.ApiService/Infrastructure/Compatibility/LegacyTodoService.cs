using imas.ApiService.Application.Common;
using imas.ApiService.Application.Todos.Commands;
using imas.ApiService.Application.Todos.DTOs;
using imas.ApiService.Application.Todos.Queries;
using imas.ApiService.Domain.Common;
using imas.ApiService.Modules.Todos;

namespace imas.ApiService.Infrastructure.Compatibility;

/// <summary>
/// Compatibility service to bridge legacy TodoService with Clean Architecture
/// This allows gradual migration while maintaining backward compatibility
/// </summary>
public class LegacyTodoService : ITodoService
{
    private readonly IRequestHandler<GetAllTodosQuery, Result<IEnumerable<TodoDto>>> _getAllHandler;
    private readonly IRequestHandler<GetTodoByIdQuery, Result<TodoDto>> _getByIdHandler;
    private readonly IRequestHandler<CreateTodoCommand, Result<TodoDto>> _createHandler;
    private readonly IRequestHandler<UpdateTodoCommand, Result<TodoDto>> _updateHandler;
    private readonly IRequestHandler<DeleteTodoCommand, Result<bool>> _deleteHandler;
    private readonly IRequestHandler<CompleteTodoCommand, Result<TodoDto>> _completeHandler;

    public LegacyTodoService(
        IRequestHandler<GetAllTodosQuery, Result<IEnumerable<TodoDto>>> getAllHandler,
        IRequestHandler<GetTodoByIdQuery, Result<TodoDto>> getByIdHandler,
        IRequestHandler<CreateTodoCommand, Result<TodoDto>> createHandler,
        IRequestHandler<UpdateTodoCommand, Result<TodoDto>> updateHandler,
        IRequestHandler<DeleteTodoCommand, Result<bool>> deleteHandler,
        IRequestHandler<CompleteTodoCommand, Result<TodoDto>> completeHandler)
    {
        _getAllHandler = getAllHandler;
        _getByIdHandler = getByIdHandler;
        _createHandler = createHandler;
        _updateHandler = updateHandler;
        _deleteHandler = deleteHandler;
        _completeHandler = completeHandler;
    }

    public async Task<IEnumerable<Todo>> GetAllAsync(string userId, CancellationToken cancellationToken = default)
    {
        var userIdValue = UserId.Create(userId);
        var query = new GetAllTodosQuery(userIdValue);
        var result = await _getAllHandler.HandleAsync(query, cancellationToken);

        if (result.IsFailure)
        {
            throw new InvalidOperationException(result.Error);
        }

        return result.Value.Select(MapToLegacyEntity);
    }

    public async Task<Todo?> GetByIdAsync(int id, string userId, CancellationToken cancellationToken = default)
    {
        var userIdValue = UserId.Create(userId);
        var query = new GetTodoByIdQuery(id, userIdValue);
        var result = await _getByIdHandler.HandleAsync(query, cancellationToken);

        if (result.IsFailure)
        {
            return null;
        }

        return MapToLegacyEntity(result.Value);
    }

    public async Task<Todo> CreateAsync(CreateTodoRequest request, string userId, CancellationToken cancellationToken = default)
    {
        var userIdValue = UserId.Create(userId);
        var createDto = new CreateTodoDto
        {
            Title = request.Title,
            Description = request.Description
        };
        var command = new CreateTodoCommand(userIdValue, createDto);
        var result = await _createHandler.HandleAsync(command, cancellationToken);

        if (result.IsFailure)
        {
            throw new InvalidOperationException(result.Error);
        }

        return MapToLegacyEntity(result.Value);
    }

    public async Task<Todo?> UpdateAsync(int id, UpdateTodoRequest request, string userId, CancellationToken cancellationToken = default)
    {
        var userIdValue = UserId.Create(userId);
        var updateDto = new UpdateTodoDto
        {
            Title = request.Title,
            Description = request.Description,
            IsCompleted = request.IsCompleted
        };
        
        var command = new UpdateTodoCommand(id, userIdValue, updateDto);
        var result = await _updateHandler.HandleAsync(command, cancellationToken);

        if (result.IsFailure)
        {
            return null;
        }

        return MapToLegacyEntity(result.Value);
    }

    public async Task<bool> DeleteAsync(int id, string userId, CancellationToken cancellationToken = default)
    {
        var userIdValue = UserId.Create(userId);
        var command = new DeleteTodoCommand(id, userId);
        var result = await _deleteHandler.HandleAsync(command, cancellationToken);

        return result.IsSuccess && result.Value;
    }

    public async Task<Todo?> CompleteAsync(int id, string userId, CancellationToken cancellationToken = default)
    {
        var userIdValue = UserId.Create(userId);
        var command = new CompleteTodoCommand(id, userId);
        var result = await _completeHandler.HandleAsync(command, cancellationToken);

        if (result.IsFailure)
        {
            return null;
        }

        return MapToLegacyEntity(result.Value);
    }

    public async Task<IEnumerable<Todo>> GetCompletedTodosAsync(string userId, CancellationToken cancellationToken = default)
    {
        // Get all todos and filter completed ones
        var allTodos = await GetAllAsync(userId, cancellationToken);
        return allTodos.Where(todo => todo.IsCompleted);
    }

    public async Task<IEnumerable<Todo>> GetPendingTodosAsync(string userId, CancellationToken cancellationToken = default)
    {
        // Get all todos and filter pending (not completed) ones
        var allTodos = await GetAllAsync(userId, cancellationToken);
        return allTodos.Where(todo => !todo.IsCompleted);
    }

    private static Todo MapToLegacyEntity(TodoDto dto)
    {
        return new Todo
        {
            Id = dto.Id,
            Title = dto.Title,
            Description = dto.Description,
            IsCompleted = dto.IsCompleted,
            UserId = 1, // TODO: Get actual user ID from context
            CreatedAt = dto.CreatedAt,
            ModifiedAt = dto.ModifiedAt
        };
    }
}
