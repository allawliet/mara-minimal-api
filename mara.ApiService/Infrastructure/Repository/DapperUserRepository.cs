using Dapper;
using mara.ApiService.Infrastructure.Data;
using mara.ApiService.Infrastructure.Repository;
using mara.ApiService.Modules.Authentication;

namespace mara.ApiService.Infrastructure.Repository;

/// <summary>
/// Dapper-based User repository with custom queries
/// </summary>
public class DapperUserRepository : DapperRepository<User, int>
{
    private readonly IDbConnectionFactory _connectionFactory;

    public DapperUserRepository(IDbConnectionFactory connectionFactory) 
        : base(connectionFactory, "Users")
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        var sql = "SELECT * FROM Users WHERE Username = @Username";
        return await QuerySingleOrDefaultAsync(sql, new { Username = username }, cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var sql = "SELECT * FROM Users WHERE Email = @Email";
        return await QuerySingleOrDefaultAsync(sql, new { Email = email }, cancellationToken);
    }

    public async Task<bool> UsernameExistsAsync(string username, CancellationToken cancellationToken = default)
    {
        var sql = "SELECT COUNT(1) FROM Users WHERE Username = @Username";
        using var connection = await _connectionFactory.CreateConnectionAsync();
        var count = await connection.QuerySingleAsync<int>(sql, new { Username = username });
        return count > 0;
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        var sql = "SELECT COUNT(1) FROM Users WHERE Email = @Email";
        using var connection = await _connectionFactory.CreateConnectionAsync();
        var count = await connection.QuerySingleAsync<int>(sql, new { Email = email });
        return count > 0;
    }
}
