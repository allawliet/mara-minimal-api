using System.Data;

namespace mara.ApiService.Infrastructure.Data;

/// <summary>
/// Interface for database connection factory
/// </summary>
public interface IDbConnectionFactory
{
    /// <summary>
    /// Creates a new database connection
    /// </summary>
    /// <returns>Database connection</returns>
    IDbConnection CreateConnection();
    
    /// <summary>
    /// Creates a new database connection asynchronously
    /// </summary>
    /// <returns>Database connection</returns>
    Task<IDbConnection> CreateConnectionAsync();
}
