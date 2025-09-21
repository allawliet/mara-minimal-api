using System.Linq.Expressions;
using imas.ApiService.Infrastructure.Models;

namespace imas.ApiService.Infrastructure.Repository;

/// <summary>
/// User-scoped repository that filters data by user ownership
/// </summary>
public class UserScopedInMemoryRepository<TEntity, TId> : InMemoryRepository<TEntity, TId>, IUserScopedRepository<TEntity, TId>
    where TEntity : class, IEntity<TId>, ICreatedEntity
    where TId : notnull
{
    public virtual Task<TEntity?> GetByIdAsync(TId id, string userId, CancellationToken cancellationToken = default)
    {
        if (_entities.TryGetValue(id, out var entity) && 
            !IsDeleted(entity) && 
            entity.CreatedBy.Equals(userId, StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult<TEntity?>(entity);
        }
        return Task.FromResult<TEntity?>(null);
    }

    public virtual Task<IEnumerable<TEntity>> GetAllAsync(string userId, CancellationToken cancellationToken = default)
    {
        var entities = _entities.Values
            .Where(e => !IsDeleted(e) && e.CreatedBy.Equals(userId, StringComparison.OrdinalIgnoreCase))
            .ToList();
        return Task.FromResult<IEnumerable<TEntity>>(entities);
    }

    public virtual Task<IEnumerable<TEntity>> FindAsync(string userId, Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var compiled = predicate.Compile();
        var entities = _entities.Values
            .Where(e => !IsDeleted(e) && 
                       e.CreatedBy.Equals(userId, StringComparison.OrdinalIgnoreCase) && 
                       compiled(e))
            .ToList();
        return Task.FromResult<IEnumerable<TEntity>>(entities);
    }

    public virtual Task<bool> ExistsAsync(TId id, string userId, CancellationToken cancellationToken = default)
    {
        var exists = _entities.TryGetValue(id, out var entity) && 
                    !IsDeleted(entity) && 
                    entity.CreatedBy.Equals(userId, StringComparison.OrdinalIgnoreCase);
        return Task.FromResult(exists);
    }

    public virtual Task<int> CountAsync(string userId, CancellationToken cancellationToken = default)
    {
        var count = _entities.Values
            .Count(e => !IsDeleted(e) && e.CreatedBy.Equals(userId, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(count);
    }

    public virtual Task<bool> DeleteAsync(TId id, string userId, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (!_entities.TryGetValue(id, out var entity) || 
                IsDeleted(entity) || 
                !entity.CreatedBy.Equals(userId, StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(false);
            }

            // Soft delete if entity supports it
            if (entity is ISoftDeleteEntity softDeleteEntity)
            {
                softDeleteEntity.IsDeleted = true;
                softDeleteEntity.DeletedAt = DateTime.UtcNow;
                softDeleteEntity.DeletedBy = userId;
                return Task.FromResult(true);
            }

            // Hard delete
            return Task.FromResult(_entities.TryRemove(id, out _));
        }
    }

    public virtual Task<(IEnumerable<TEntity> Items, int TotalCount)> GetPagedAsync(
        string userId,
        int page, 
        int pageSize, 
        Expression<Func<TEntity, bool>>? filter = null,
        Expression<Func<TEntity, object>>? orderBy = null,
        bool orderByDescending = false,
        CancellationToken cancellationToken = default)
    {
        var query = _entities.Values
            .Where(e => !IsDeleted(e) && e.CreatedBy.Equals(userId, StringComparison.OrdinalIgnoreCase))
            .AsQueryable();

        if (filter != null)
        {
            query = query.Where(filter);
        }

        var totalCount = query.Count();

        if (orderBy != null)
        {
            query = orderByDescending 
                ? query.OrderByDescending(orderBy)
                : query.OrderBy(orderBy);
        }

        var items = query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return Task.FromResult<(IEnumerable<TEntity>, int)>((items, totalCount));
    }

    public override Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        // Ensure creation metadata is set
        if (entity.CreatedAt == default)
        {
            entity.CreatedAt = DateTime.UtcNow;
        }

        return base.AddAsync(entity, cancellationToken);
    }
}
