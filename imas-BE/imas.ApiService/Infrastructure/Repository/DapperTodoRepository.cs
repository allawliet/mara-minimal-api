using Dapper;
using imas.ApiService.Infrastructure.Data;
using imas.ApiService.Infrastructure.Repository;
using imas.ApiService.Modules.Todos;

namespace imas.ApiService.Infrastructure.Repository;

/// <summary>
/// Dapper-based Todo repository with user-scoped queries
/// </summary>
public class DapperTodoRepository : DapperRepository<Todo, int>
{
    private readonly IDbConnectionFactory _connectionFactory;

    public DapperTodoRepository(IDbConnectionFactory connectionFactory) 
        : base(connectionFactory, "Todos")
    {
        _connectionFactory = connectionFactory;
    }

    /// <summary>
    /// Get all todos for a specific user
    /// </summary>
    public async Task<IEnumerable<Todo>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        var sql = "SELECT * FROM Todos WHERE UserId = @UserId ORDER BY CreatedAt DESC";
        return await QueryAsync(sql, new { UserId = userId }, cancellationToken);
    }

    /// <summary>
    /// Get completed todos for a specific user
    /// </summary>
    public async Task<IEnumerable<Todo>> GetCompletedByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        var sql = "SELECT * FROM Todos WHERE UserId = @UserId AND IsCompleted = 1 ORDER BY UpdatedAt DESC";
        return await QueryAsync(sql, new { UserId = userId }, cancellationToken);
    }

    /// <summary>
    /// Get pending todos for a specific user
    /// </summary>
    public async Task<IEnumerable<Todo>> GetPendingByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        var sql = "SELECT * FROM Todos WHERE UserId = @UserId AND IsCompleted = 0 ORDER BY CreatedAt ASC";
        return await QueryAsync(sql, new { UserId = userId }, cancellationToken);
    }

    /// <summary>
    /// Get a todo by ID for a specific user (ensures user owns the todo)
    /// </summary>
    public async Task<Todo?> GetByIdAndUserIdAsync(int id, int userId, CancellationToken cancellationToken = default)
    {
        var sql = "SELECT * FROM Todos WHERE Id = @Id AND UserId = @UserId";
        return await QuerySingleOrDefaultAsync(sql, new { Id = id, UserId = userId }, cancellationToken);
    }

    /// <summary>
    /// Update a todo for a specific user
    /// </summary>
    public async Task<Todo?> UpdateByUserIdAsync(int id, int userId, Todo entity, CancellationToken cancellationToken = default)
    {
        var sql = @"
            UPDATE Todos 
            SET Title = @Title, 
                Description = @Description, 
                IsCompleted = @IsCompleted, 
                UpdatedAt = GETUTCDATE() 
            OUTPUT INSERTED.*
            WHERE Id = @Id AND UserId = @UserId";
        
        using var connection = await _connectionFactory.CreateConnectionAsync();
        return await connection.QuerySingleOrDefaultAsync<Todo>(sql, new 
        { 
            Id = id, 
            UserId = userId, 
            entity.Title, 
            entity.Description, 
            entity.IsCompleted 
        });
    }

    /// <summary>
    /// Delete a todo for a specific user
    /// </summary>
    public async Task<bool> DeleteByUserIdAsync(int id, int userId, CancellationToken cancellationToken = default)
    {
        var sql = "DELETE FROM Todos WHERE Id = @Id AND UserId = @UserId";
        using var connection = await _connectionFactory.CreateConnectionAsync();
        var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id, UserId = userId });
        return rowsAffected > 0;
    }

    /// <summary>
    /// Count todos for a specific user
    /// </summary>
    public async Task<int> CountByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        var sql = "SELECT COUNT(*) FROM Todos WHERE UserId = @UserId";
        using var connection = await _connectionFactory.CreateConnectionAsync();
        return await connection.QuerySingleAsync<int>(sql, new { UserId = userId });
    }
}
