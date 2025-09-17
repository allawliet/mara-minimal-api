using System.Data;

namespace mara.ApiService.Infrastructure.Repository;

/// <summary>
/// Simplified Dapper repository interface for basic CRUD operations
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
/// <typeparam name="TId">ID type</typeparam>
public interface IDapperRepository<TEntity, TId>
    where TEntity : class
    where TId : IEquatable<TId>
{
    // Basic CRUD operations
    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
    Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<TEntity?> UpdateAsync(TId id, TEntity entity, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(TId id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(TId id, CancellationToken cancellationToken = default);
    Task<int> CountAsync(CancellationToken cancellationToken = default);

    // Raw SQL operations
    Task<IEnumerable<TEntity>> QueryAsync(string sql, object? parameters = null, CancellationToken cancellationToken = default);
    Task<TEntity?> QuerySingleOrDefaultAsync(string sql, object? parameters = null, CancellationToken cancellationToken = default);
    Task<int> ExecuteAsync(string sql, object? parameters = null, CancellationToken cancellationToken = default);
}
