using System.Linq.Expressions;
using System.Collections.Concurrent;
using imas.ApiService.Infrastructure.Models;

namespace imas.ApiService.Infrastructure.Repository;

/// <summary>
/// In-memory implementation of the generic repository
/// Thread-safe using ConcurrentDictionary
/// </summary>
public class InMemoryRepository<TEntity, TId> : IRepository<TEntity, TId>
    where TEntity : class, IEntity<TId>
    where TId : notnull
{
    protected readonly ConcurrentDictionary<TId, TEntity> _entities = new();
    protected readonly object _lock = new();

    public virtual Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        _entities.TryGetValue(id, out var entity);
        return Task.FromResult(entity);
    }

    public virtual Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = _entities.Values.Where(e => !IsDeleted(e)).ToList();
        return Task.FromResult<IEnumerable<TEntity>>(entities);
    }

    public virtual Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var compiled = predicate.Compile();
        var entities = _entities.Values.Where(e => !IsDeleted(e) && compiled(e)).ToList();
        return Task.FromResult<IEnumerable<TEntity>>(entities);
    }

    public virtual Task<TEntity?> FindFirstAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var compiled = predicate.Compile();
        var entity = _entities.Values.Where(e => !IsDeleted(e)).FirstOrDefault(compiled);
        return Task.FromResult(entity);
    }

    public virtual Task<bool> ExistsAsync(TId id, CancellationToken cancellationToken = default)
    {
        var exists = _entities.TryGetValue(id, out var entity) && !IsDeleted(entity);
        return Task.FromResult(exists);
    }

    public virtual Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var compiled = predicate.Compile();
        var exists = _entities.Values.Where(e => !IsDeleted(e)).Any(compiled);
        return Task.FromResult(exists);
    }

    public virtual Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        var count = _entities.Values.Count(e => !IsDeleted(e));
        return Task.FromResult(count);
    }

    public virtual Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var compiled = predicate.Compile();
        var count = _entities.Values.Where(e => !IsDeleted(e)).Count(compiled);
        return Task.FromResult(count);
    }

    public virtual Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            // Set creation timestamp if entity supports it
            if (entity is ICreatedEntity createdEntity && createdEntity.CreatedAt == default)
            {
                createdEntity.CreatedAt = DateTime.UtcNow;
            }

            _entities.TryAdd(entity.Id, entity);
            return Task.FromResult(entity);
        }
    }

    public virtual Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var addedEntities = new List<TEntity>();
            foreach (var entity in entities)
            {
                // Set creation timestamp if entity supports it
                if (entity is ICreatedEntity createdEntity && createdEntity.CreatedAt == default)
                {
                    createdEntity.CreatedAt = DateTime.UtcNow;
                }

                if (_entities.TryAdd(entity.Id, entity))
                {
                    addedEntities.Add(entity);
                }
            }
            return Task.FromResult<IEnumerable<TEntity>>(addedEntities);
        }
    }

    public virtual Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            // Set modification timestamp if entity supports it
            if (entity is IModifiedEntity modifiedEntity)
            {
                modifiedEntity.ModifiedAt = DateTime.UtcNow;
            }

            _entities.AddOrUpdate(entity.Id, entity, (key, oldValue) => entity);
            return Task.FromResult(entity);
        }
    }

    public virtual Task<TEntity?> UpdateAsync(TId id, Action<TEntity> updateAction, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (!_entities.TryGetValue(id, out var entity) || IsDeleted(entity))
            {
                return Task.FromResult<TEntity?>(null);
            }

            updateAction(entity);

            // Set modification timestamp if entity supports it
            if (entity is IModifiedEntity modifiedEntity)
            {
                modifiedEntity.ModifiedAt = DateTime.UtcNow;
            }

            return Task.FromResult<TEntity?>(entity);
        }
    }

    public virtual Task<bool> DeleteAsync(TId id, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (!_entities.TryGetValue(id, out var entity))
            {
                return Task.FromResult(false);
            }

            // Soft delete if entity supports it
            if (entity is ISoftDeleteEntity softDeleteEntity)
            {
                softDeleteEntity.IsDeleted = true;
                softDeleteEntity.DeletedAt = DateTime.UtcNow;
                return Task.FromResult(true);
            }

            // Hard delete
            return Task.FromResult(_entities.TryRemove(id, out _));
        }
    }

    public virtual Task<bool> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(entity.Id, cancellationToken);
    }

    public virtual Task<int> DeleteRangeAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var compiled = predicate.Compile();
            var entitiesToDelete = _entities.Values.Where(e => !IsDeleted(e) && compiled(e)).ToList();
            
            var deletedCount = 0;
            foreach (var entity in entitiesToDelete)
            {
                if (entity is ISoftDeleteEntity softDeleteEntity)
                {
                    softDeleteEntity.IsDeleted = true;
                    softDeleteEntity.DeletedAt = DateTime.UtcNow;
                    deletedCount++;
                }
                else if (_entities.TryRemove(entity.Id, out _))
                {
                    deletedCount++;
                }
            }

            return Task.FromResult(deletedCount);
        }
    }

    public virtual Task<(IEnumerable<TEntity> Items, int TotalCount)> GetPagedAsync(
        int page, 
        int pageSize, 
        Expression<Func<TEntity, bool>>? filter = null,
        Expression<Func<TEntity, object>>? orderBy = null,
        bool orderByDescending = false,
        CancellationToken cancellationToken = default)
    {
        var query = _entities.Values.Where(e => !IsDeleted(e)).AsQueryable();

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

    protected virtual bool IsDeleted(TEntity entity)
    {
        return entity is ISoftDeleteEntity { IsDeleted: true };
    }
}
