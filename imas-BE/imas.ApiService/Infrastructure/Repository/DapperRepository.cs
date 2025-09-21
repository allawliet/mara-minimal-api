using Dapper;
using imas.ApiService.Infrastructure.Data;
using imas.ApiService.Infrastructure.Models;
using System.Data;

namespace imas.ApiService.Infrastructure.Repository;

/// <summary>
/// Dapper-based repository implementation
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
/// <typeparam name="TId">ID type</typeparam>
public class DapperRepository<TEntity, TId> : IDapperRepository<TEntity, TId>
    where TEntity : class
    where TId : IEquatable<TId>
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly string _tableName;

    public DapperRepository(IDbConnectionFactory connectionFactory, string tableName)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        _tableName = tableName ?? throw new ArgumentNullException(nameof(tableName));
    }

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        var sql = $"SELECT * FROM {_tableName}";
        return await connection.QueryAsync<TEntity>(sql);
    }

    public virtual async Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        var sql = $"SELECT * FROM {_tableName} WHERE Id = @Id";
        return await connection.QuerySingleOrDefaultAsync<TEntity>(sql, new { Id = id });
    }

    public virtual async Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        
        // Get entity properties for dynamic SQL generation
        var properties = typeof(TEntity).GetProperties()
            .Where(p => p.Name != "Id" && p.CanWrite)
            .ToList();

        var columnNames = string.Join(", ", properties.Select(p => p.Name));
        var parameterNames = string.Join(", ", properties.Select(p => $"@{p.Name}"));

        var sql = $"INSERT INTO {_tableName} ({columnNames}) OUTPUT INSERTED.* VALUES ({parameterNames})";
        
        var result = await connection.QuerySingleAsync<TEntity>(sql, entity);
        return result;
    }

    public virtual async Task<TEntity?> UpdateAsync(TId id, TEntity entity, CancellationToken cancellationToken = default)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        
        // Get entity properties for dynamic SQL generation
        var properties = typeof(TEntity).GetProperties()
            .Where(p => p.Name != "Id" && p.CanWrite)
            .ToList();

        var setClause = string.Join(", ", properties.Select(p => $"{p.Name} = @{p.Name}"));
        var sql = $"UPDATE {_tableName} SET {setClause} OUTPUT INSERTED.* WHERE Id = @Id";
        
        // Create parameters including the ID
        var parameters = new Dictionary<string, object>();
        foreach (var property in properties)
        {
            parameters[property.Name] = property.GetValue(entity) ?? DBNull.Value;
        }
        parameters["Id"] = id;
        
        var result = await connection.QuerySingleOrDefaultAsync<TEntity>(sql, entity);
        return result;
    }

    public virtual async Task<bool> DeleteAsync(TId id, CancellationToken cancellationToken = default)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        var sql = $"DELETE FROM {_tableName} WHERE Id = @Id";
        var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });
        return rowsAffected > 0;
    }

    public virtual async Task<bool> ExistsAsync(TId id, CancellationToken cancellationToken = default)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        var sql = $"SELECT COUNT(1) FROM {_tableName} WHERE Id = @Id";
        var count = await connection.QuerySingleAsync<int>(sql, new { Id = id });
        return count > 0;
    }

    public virtual async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        var sql = $"SELECT COUNT(*) FROM {_tableName}";
        return await connection.QuerySingleAsync<int>(sql);
    }

    /// <summary>
    /// Execute a custom query
    /// </summary>
    /// <param name="sql">SQL query</param>
    /// <param name="parameters">Query parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Query results</returns>
    public async Task<IEnumerable<TEntity>> QueryAsync(string sql, object? parameters = null, CancellationToken cancellationToken = default)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        return await connection.QueryAsync<TEntity>(sql, parameters);
    }

    /// <summary>
    /// Execute a custom query and return a single result
    /// </summary>
    /// <param name="sql">SQL query</param>
    /// <param name="parameters">Query parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Single query result</returns>
    public async Task<TEntity?> QuerySingleOrDefaultAsync(string sql, object? parameters = null, CancellationToken cancellationToken = default)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        return await connection.QuerySingleOrDefaultAsync<TEntity>(sql, parameters);
    }

    /// <summary>
    /// Execute a command (INSERT, UPDATE, DELETE)
    /// </summary>
    /// <param name="sql">SQL command</param>
    /// <param name="parameters">Command parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Number of rows affected</returns>
    public async Task<int> ExecuteAsync(string sql, object? parameters = null, CancellationToken cancellationToken = default)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        return await connection.ExecuteAsync(sql, parameters);
    }
}
