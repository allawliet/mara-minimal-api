using Microsoft.Data.SqlClient;
using System.Data;

namespace imas.ApiService.Infrastructure.Data;

/// <summary>
/// SQL Server connection factory implementation
/// </summary>
public class SqlServerConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public SqlServerConnectionFactory(string connectionString)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }

    /// <summary>
    /// Creates a new SQL Server connection
    /// </summary>
    /// <returns>SQL Server connection</returns>
    public IDbConnection CreateConnection()
    {
        var connection = new SqlConnection(_connectionString);
        return connection;
    }

    /// <summary>
    /// Creates a new SQL Server connection asynchronously
    /// </summary>
    /// <returns>SQL Server connection</returns>
    public async Task<IDbConnection> CreateConnectionAsync()
    {
        var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        return connection;
    }
}
