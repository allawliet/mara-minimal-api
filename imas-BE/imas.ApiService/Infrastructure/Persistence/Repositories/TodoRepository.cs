using Microsoft.EntityFrameworkCore;
using imas.ApiService.Domain.Common;
using imas.ApiService.Domain.Todos.Entities;
using imas.ApiService.Domain.Todos.Repositories;
using imas.ApiService.Domain.Todos.ValueObjects;
using imas.ApiService.Infrastructure.Data;

namespace imas.ApiService.Infrastructure.Persistence.Repositories;

/// <summary>
/// Entity Framework implementation of ITodoRepository
/// </summary>
public class TodoRepository : ITodoRepository
{
    private readonly ImasDbContext _context;

    public TodoRepository(ImasDbContext context)
    {
        _context = context;
    }

    public async Task<Todo?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var efTodo = await _context.Todos
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

        return efTodo != null ? MapToDomainEntity(efTodo) : null;
    }

    public async Task<Todo?> GetByIdForUserAsync(int id, UserId userId, CancellationToken cancellationToken = default)
    {
        var userIdInt = int.Parse(userId.Value); // Convert string UserId to int for EF query
        var efTodo = await _context.Todos
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userIdInt, cancellationToken);

        return efTodo != null ? MapToDomainEntity(efTodo) : null;
    }

    public async Task<IEnumerable<Todo>> GetAllForUserAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var userIdInt = int.Parse(userId.Value);
        var efTodos = await _context.Todos
            .Where(t => t.UserId == userIdInt)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);

        return efTodos.Select(MapToDomainEntity);
    }

    public async Task<IEnumerable<Todo>> GetCompletedForUserAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var userIdInt = int.Parse(userId.Value);
        var efTodos = await _context.Todos
            .Where(t => t.UserId == userIdInt && t.IsCompleted)
            .OrderByDescending(t => t.UpdatedAt) // Use UpdatedAt since CompletedAt doesn't exist
            .ToListAsync(cancellationToken);

        return efTodos.Select(MapToDomainEntity);
    }

    public async Task<IEnumerable<Todo>> GetPendingForUserAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var userIdInt = int.Parse(userId.Value);
        var efTodos = await _context.Todos
            .Where(t => t.UserId == userIdInt && !t.IsCompleted)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);

        return efTodos.Select(MapToDomainEntity);
    }

    public async Task<(IEnumerable<Todo> Items, int TotalCount)> GetPagedForUserAsync(
        UserId userId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var userIdInt = int.Parse(userId.Value);
        var query = _context.Todos
            .Where(t => t.UserId == userIdInt);

        var totalCount = await query.CountAsync(cancellationToken);

        var efTodos = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var domainTodos = efTodos.Select(MapToDomainEntity);

        return (domainTodos, totalCount);
    }

    public async Task<int> CountForUserAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var userIdInt = int.Parse(userId.Value);
        return await _context.Todos
            .CountAsync(t => t.UserId == userIdInt, cancellationToken);
    }

    public async Task<int> CountCompletedForUserAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var userIdInt = int.Parse(userId.Value);
        return await _context.Todos
            .CountAsync(t => t.UserId == userIdInt && t.IsCompleted, cancellationToken);
    }

    public async Task<int> CountPendingForUserAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var userIdInt = int.Parse(userId.Value);
        return await _context.Todos
            .CountAsync(t => t.UserId == userIdInt && !t.IsCompleted, cancellationToken);
    }

    public Task<Todo> AddAsync(Todo todo, CancellationToken cancellationToken = default)
    {
        var efTodo = MapToEfEntity(todo);
        _context.Todos.Add(efTodo);
        
        return Task.FromResult(todo); // Return the domain entity
    }

    public async Task UpdateAsync(Todo todo, CancellationToken cancellationToken = default)
    {
        var efTodo = await _context.Todos
            .FirstOrDefaultAsync(t => t.Id == todo.Id, cancellationToken);

        if (efTodo != null)
        {
            // Update properties
            efTodo.Title = todo.Title.Value;
            efTodo.Description = todo.Description?.Value;
            efTodo.IsCompleted = todo.IsCompleted;
            efTodo.UpdatedAt = todo.ModifiedAt ?? DateTime.UtcNow;
        }
    }

    public async Task DeleteAsync(Todo todo, CancellationToken cancellationToken = default)
    {
        var efTodo = await _context.Todos
            .FirstOrDefaultAsync(t => t.Id == todo.Id, cancellationToken);

        if (efTodo != null)
        {
            _context.Todos.Remove(efTodo);
        }
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Todos
            .AnyAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsForUserAsync(int id, UserId userId, CancellationToken cancellationToken = default)
    {
        var userIdInt = int.Parse(userId.Value);
        return await _context.Todos
            .AnyAsync(t => t.Id == id && t.UserId == userIdInt, cancellationToken);
    }

    private static Todo MapToDomainEntity(Modules.Todos.Todo efTodo)
    {
        var userId = UserId.Create(efTodo.UserId.ToString());
        var title = TodoTitle.Create(efTodo.Title);
        var description = string.IsNullOrEmpty(efTodo.Description) 
            ? null 
            : TodoDescription.Create(efTodo.Description);

        var domainTodo = Todo.Create(title, description, userId);
        domainTodo.SetId(efTodo.Id);
        
        return domainTodo;
    }

    private static Modules.Todos.Todo MapToEfEntity(Todo domainTodo)
    {
        return new Modules.Todos.Todo
        {
            Id = domainTodo.Id,
            UserId = int.Parse(domainTodo.UserId.Value),
            Title = domainTodo.Title.Value,
            Description = domainTodo.Description?.Value,
            IsCompleted = domainTodo.IsCompleted,
            CreatedAt = domainTodo.CreatedAt,
            UpdatedAt = domainTodo.ModifiedAt ?? DateTime.UtcNow
        };
    }
}
