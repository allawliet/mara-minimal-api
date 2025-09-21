using System.Security.Claims;
using imas.ApiService.Infrastructure.Models;
using imas.ApiService.Modules;
using imas.Shared.Contracts.Authentication;
using imas.ServiceDefaults.Configuration;
using imas.ServiceDefaults.Authentication;

namespace imas.ApiService.Modules.Authentication;

// Note: Using shared contracts from imas.Shared.Contracts.Authentication
// Legacy local contracts are replaced with shared ones

public record LoginResponse(string Token, string Username, string Email);

public class User : BaseEntity<int>
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class AuthenticationModule : IModule
{
    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        // JWT configuration is already handled in Program.cs, just register services
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IUserService, UserService>();
    }

    public void RegisterEndpoints(IEndpointRouteBuilder endpoints)
    {
        var authGroup = endpoints.MapGroup("/auth")
            .WithTags("Authentication")
            .RequireRateLimiting("AuthPolicy");

        authGroup.MapPost("/login", LoginAsync)
            .WithName("Login")
            .AllowAnonymous();

        authGroup.MapPost("/register", RegisterAsync)
            .WithName("Register")
            .AllowAnonymous();

        authGroup.MapGet("/profile", GetProfileAsync)
            .WithName("GetProfile")
            .RequireAuthorization();
    }

    private static async Task<IResult> LoginAsync(
        LoginRequest request,
        IUserService userService,
        ICentralizedTokenService tokenService)
    {
        var user = await userService.ValidateUserAsync(request.Username, request.Password);
        if (user == null)
        {
            return Results.Unauthorized();
        }

        var token = tokenService.GenerateToken(user.Username, user.Email, user.Roles);
        return Results.Ok(new LoginResponse(token, user.Username, user.Email));
    }

    private static async Task<IResult> RegisterAsync(
        RegisterRequest request,
        IUserService userService,
        ICentralizedTokenService tokenService)
    {
        if (await userService.UserExistsAsync(request.Username))
        {
            return Results.BadRequest("Username already exists");
        }

        var user = await userService.CreateUserAsync(request.Username, request.Email, request.Password);
        var token = tokenService.GenerateToken(user.Username, user.Email, user.Roles);
        
        return Results.Ok(new LoginResponse(token, user.Username, user.Email));
    }

    private static async Task<IResult> GetProfileAsync(ClaimsPrincipal user, IUserService userService)
    {
        var username = user.FindFirst(ClaimTypes.Name)?.Value;
        if (string.IsNullOrEmpty(username))
        {
            return Results.Unauthorized();
        }

        var userProfile = await userService.GetUserAsync(username);
        if (userProfile == null)
        {
            return Results.NotFound();
        }

        return Results.Ok(new
        {
            userProfile.Username,
            userProfile.Email,
            userProfile.Roles
        });
    }
}

public interface IUserService
{
    Task<User?> ValidateUserAsync(string username, string password);
    Task<bool> UserExistsAsync(string username);
    Task<User> CreateUserAsync(string username, string email, string password);
    Task<User?> GetUserAsync(string username);
}

public class UserService : IUserService
{
    private readonly List<User> _users = new()
    {
        new User
        {
            Username = "admin",
            Email = "admin@imas.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
            Roles = new List<string> { "Admin", "User" }
        },
        new User
        {
            Username = "user",
            Email = "user@imas.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("user123"),
            Roles = new List<string> { "User" }
        }
    };

    public Task<User?> ValidateUserAsync(string username, string password)
    {
        var user = _users.FirstOrDefault(u => 
            u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        
        if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            return Task.FromResult<User?>(user);
        }
        
        return Task.FromResult<User?>(null);
    }

    public Task<bool> UserExistsAsync(string username)
    {
        return Task.FromResult(_users.Any(u => 
            u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)));
    }

    public Task<User> CreateUserAsync(string username, string email, string password)
    {
        var user = new User
        {
            Username = username,
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            Roles = new List<string> { "User" }
        };
        
        _users.Add(user);
        return Task.FromResult(user);
    }

    public Task<User?> GetUserAsync(string username)
    {
        var user = _users.FirstOrDefault(u => 
            u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(user);
    }
}
