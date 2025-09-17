using System.Linq.Expressions;
using mara.ApiService.Infrastructure.Models;

namespace mara.ApiService.Infrastructure.Repository;

/// <summary>
/// Generic repository interface for basic CRUD operations
/// </summary>
public interface IRepository<TEntity, TId> 
    where TEntity : class, IEntity<TId>
{
    // Read operations
    Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    Task<TEntity?> FindFirstAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(TId id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    // Write operations
    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
    Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<TEntity?> UpdateAsync(TId id, Action<TEntity> updateAction, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(TId id, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<int> DeleteRangeAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    // Pagination
    Task<(IEnumerable<TEntity> Items, int TotalCount)> GetPagedAsync(
        int page, 
        int pageSize, 
        Expression<Func<TEntity, bool>>? filter = null,
        Expression<Func<TEntity, object>>? orderBy = null,
        bool orderByDescending = false,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Extended repository interface for user-specific operations
/// </summary>
public interface IUserScopedRepository<TEntity, TId> : IRepository<TEntity, TId>
    where TEntity : class, IEntity<TId>, ICreatedEntity
{
    Task<TEntity?> GetByIdAsync(TId id, string userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<TEntity>> GetAllAsync(string userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<TEntity>> FindAsync(string userId, Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(TId id, string userId, CancellationToken cancellationToken = default);
    Task<int> CountAsync(string userId, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(TId id, string userId, CancellationToken cancellationToken = default);
    
    Task<(IEnumerable<TEntity> Items, int TotalCount)> GetPagedAsync(
        string userId,
        int page, 
        int pageSize, 
        Expression<Func<TEntity, bool>>? filter = null,
        Expression<Func<TEntity, object>>? orderBy = null,
        bool orderByDescending = false,
        CancellationToken cancellationToken = default);
}
