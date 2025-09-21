using imas.ApiService.Domain.Common;
using imas.ApiService.Domain.Todos.Entities;

namespace imas.ApiService.Domain.Todos.Repositories;

/// <summary>
/// Todo repository interface following DDD patterns
/// </summary>
public interface ITodoRepository
{
    Task<Todo?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Todo?> GetByIdForUserAsync(int id, UserId userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Todo>> GetAllForUserAsync(UserId userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Todo>> GetCompletedForUserAsync(UserId userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Todo>> GetPendingForUserAsync(UserId userId, CancellationToken cancellationToken = default);
    Task<(IEnumerable<Todo> Items, int TotalCount)> GetPagedForUserAsync(
        UserId userId, 
        int page, 
        int pageSize, 
        CancellationToken cancellationToken = default);
    
    Task<int> CountForUserAsync(UserId userId, CancellationToken cancellationToken = default);
    Task<int> CountCompletedForUserAsync(UserId userId, CancellationToken cancellationToken = default);
    Task<int> CountPendingForUserAsync(UserId userId, CancellationToken cancellationToken = default);
    
    Task<Todo> AddAsync(Todo todo, CancellationToken cancellationToken = default);
    Task UpdateAsync(Todo todo, CancellationToken cancellationToken = default);
    Task DeleteAsync(Todo todo, CancellationToken cancellationToken = default);
    
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsForUserAsync(int id, UserId userId, CancellationToken cancellationToken = default);
}
