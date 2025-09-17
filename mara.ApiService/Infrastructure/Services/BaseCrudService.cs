using System.Linq.Expressions;
using mara.ApiService.Infrastructure.Models;
using mara.ApiService.Infrastructure.Repository;

namespace mara.ApiService.Infrastructure.Services;

/// <summary>
/// Generic CRUD service interface
/// </summary>
public interface ICrudService<TEntity, TId, TCreateRequest, TUpdateRequest>
    where TEntity : class, IEntity<TId>
{
    Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<TEntity>> SearchAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    Task<(IEnumerable<TEntity> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(TId id, CancellationToken cancellationToken = default);
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    
    Task<TEntity> CreateAsync(TCreateRequest request, CancellationToken cancellationToken = default);
    Task<TEntity?> UpdateAsync(TId id, TUpdateRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(TId id, CancellationToken cancellationToken = default);
}

/// <summary>
/// User-scoped CRUD service interface
/// </summary>
public interface IUserScopedCrudService<TEntity, TId, TCreateRequest, TUpdateRequest> : ICrudService<TEntity, TId, TCreateRequest, TUpdateRequest>
    where TEntity : class, IEntity<TId>, ICreatedEntity
{
    Task<TEntity?> GetByIdAsync(TId id, string userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<TEntity>> GetAllAsync(string userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<TEntity>> SearchAsync(string userId, Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    Task<(IEnumerable<TEntity> Items, int TotalCount)> GetPagedAsync(string userId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(TId id, string userId, CancellationToken cancellationToken = default);
    Task<int> CountAsync(string userId, CancellationToken cancellationToken = default);
    
    Task<TEntity> CreateAsync(TCreateRequest request, string userId, CancellationToken cancellationToken = default);
    Task<TEntity?> UpdateAsync(TId id, TUpdateRequest request, string userId, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(TId id, string userId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Base CRUD service implementation
/// </summary>
public abstract class BaseCrudService<TEntity, TId, TCreateRequest, TUpdateRequest> : ICrudService<TEntity, TId, TCreateRequest, TUpdateRequest>
    where TEntity : class, IEntity<TId>
{
    protected readonly IRepository<TEntity, TId> _repository;

    protected BaseCrudService(IRepository<TEntity, TId> repository)
    {
        _repository = repository;
    }

    public virtual Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        return _repository.GetByIdAsync(id, cancellationToken);
    }

    public virtual Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _repository.GetAllAsync(cancellationToken);
    }

    public virtual Task<IEnumerable<TEntity>> SearchAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return _repository.FindAsync(predicate, cancellationToken);
    }

    public virtual Task<(IEnumerable<TEntity> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        return _repository.GetPagedAsync(page, pageSize, cancellationToken: cancellationToken);
    }

    public virtual Task<bool> ExistsAsync(TId id, CancellationToken cancellationToken = default)
    {
        return _repository.ExistsAsync(id, cancellationToken);
    }

    public virtual Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return _repository.CountAsync(cancellationToken);
    }

    public virtual async Task<TEntity> CreateAsync(TCreateRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await MapCreateRequestToEntityAsync(request, cancellationToken);
        return await _repository.AddAsync(entity, cancellationToken);
    }

    public virtual async Task<TEntity?> UpdateAsync(TId id, TUpdateRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity == null)
        {
            return null;
        }

        await MapUpdateRequestToEntityAsync(request, entity, cancellationToken);
        return await _repository.UpdateAsync(entity, cancellationToken);
    }

    public virtual Task<bool> DeleteAsync(TId id, CancellationToken cancellationToken = default)
    {
        return _repository.DeleteAsync(id, cancellationToken);
    }

    // Abstract methods to be implemented by concrete services
    protected abstract Task<TEntity> MapCreateRequestToEntityAsync(TCreateRequest request, CancellationToken cancellationToken = default);
    protected abstract Task MapUpdateRequestToEntityAsync(TUpdateRequest request, TEntity entity, CancellationToken cancellationToken = default);
}

/// <summary>
/// User-scoped CRUD service implementation
/// </summary>
public abstract class BaseUserScopedCrudService<TEntity, TId, TCreateRequest, TUpdateRequest> : 
    BaseCrudService<TEntity, TId, TCreateRequest, TUpdateRequest>, 
    IUserScopedCrudService<TEntity, TId, TCreateRequest, TUpdateRequest>
    where TEntity : class, IEntity<TId>, ICreatedEntity
{
    protected readonly IUserScopedRepository<TEntity, TId> _userScopedRepository;

    protected BaseUserScopedCrudService(IUserScopedRepository<TEntity, TId> repository) : base(repository)
    {
        _userScopedRepository = repository;
    }

    // User-scoped methods
    public virtual Task<TEntity?> GetByIdAsync(TId id, string userId, CancellationToken cancellationToken = default)
    {
        return _userScopedRepository.GetByIdAsync(id, userId, cancellationToken);
    }

    public virtual Task<IEnumerable<TEntity>> GetAllAsync(string userId, CancellationToken cancellationToken = default)
    {
        return _userScopedRepository.GetAllAsync(userId, cancellationToken);
    }

    public virtual Task<IEnumerable<TEntity>> SearchAsync(string userId, Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return _userScopedRepository.FindAsync(userId, predicate, cancellationToken);
    }

    public virtual Task<(IEnumerable<TEntity> Items, int TotalCount)> GetPagedAsync(string userId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        return _userScopedRepository.GetPagedAsync(userId, page, pageSize, cancellationToken: cancellationToken);
    }

    public virtual Task<bool> ExistsAsync(TId id, string userId, CancellationToken cancellationToken = default)
    {
        return _userScopedRepository.ExistsAsync(id, userId, cancellationToken);
    }

    public virtual Task<int> CountAsync(string userId, CancellationToken cancellationToken = default)
    {
        return _userScopedRepository.CountAsync(userId, cancellationToken);
    }

    public virtual async Task<TEntity> CreateAsync(TCreateRequest request, string userId, CancellationToken cancellationToken = default)
    {
        var entity = await MapCreateRequestToEntityAsync(request, cancellationToken);
        
        // Set the user who created this entity
        entity.CreatedBy = userId;
        entity.CreatedAt = DateTime.UtcNow;
        
        return await _repository.AddAsync(entity, cancellationToken);
    }

    public virtual async Task<TEntity?> UpdateAsync(TId id, TUpdateRequest request, string userId, CancellationToken cancellationToken = default)
    {
        var entity = await _userScopedRepository.GetByIdAsync(id, userId, cancellationToken);
        if (entity == null)
        {
            return null;
        }

        await MapUpdateRequestToEntityAsync(request, entity, cancellationToken);
        
        // Set modification metadata
        if (entity is IModifiedEntity modifiedEntity)
        {
            modifiedEntity.ModifiedBy = userId;
            modifiedEntity.ModifiedAt = DateTime.UtcNow;
        }
        
        return await _repository.UpdateAsync(entity, cancellationToken);
    }

    public virtual Task<bool> DeleteAsync(TId id, string userId, CancellationToken cancellationToken = default)
    {
        return _userScopedRepository.DeleteAsync(id, userId, cancellationToken);
    }
}
