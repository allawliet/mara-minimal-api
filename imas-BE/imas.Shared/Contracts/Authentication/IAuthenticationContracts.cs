namespace imas.Shared.Contracts.Authentication;

/// <summary>
/// Simple authentication contracts for independent services
/// </summary>
public interface IAuthenticationService
{
    Task<AuthResponse<UserDto>> AuthenticateAsync(LoginRequest request);
    Task<AuthResponse<UserDto>> RegisterAsync(RegisterRequest request);
    Task<AuthResponse<bool>> ValidateTokenAsync(string token);
}

/// <summary>
/// Simple authentication response wrapper
/// </summary>
public record AuthResponse<T>(
    bool IsSuccess,
    T? Data = default,
    string? ErrorMessage = null,
    DateTime ProcessedAt = default)
{
    public AuthResponse() : this(false, default, null, DateTime.UtcNow) { }
    
    public static AuthResponse<T> Success(T data) => 
        new(true, data, null, DateTime.UtcNow);
        
    public static AuthResponse<T> Failure(string errorMessage) => 
        new(false, default, errorMessage, DateTime.UtcNow);
}

/// <summary>
/// User data transfer object
/// </summary>
public record UserDto(
    int Id,
    string Username,
    string Email,
    DateTime CreatedAt);

/// <summary>
/// Login request model
/// </summary>
public record LoginRequest(
    string Username,
    string Password);

/// <summary>
/// Registration request model
/// </summary>
public record RegisterRequest(
    string Username,
    string Email,
    string Password);