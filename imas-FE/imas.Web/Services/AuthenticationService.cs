using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json;

namespace imas.Web.Services;

// Models for authentication
public class LoginRequest
{
    [Required(ErrorMessage = "Username is required")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 100 characters")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
    public string Password { get; set; } = string.Empty;
}

public class RegisterRequest
{
    [Required(ErrorMessage = "Username is required")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 100 characters")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]", 
        ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please confirm your password")]
    [Compare("Password", ErrorMessage = "Passwords do not match")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public class UserProfile
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

// Authentication state management
public class AuthenticationState
{
    public bool IsAuthenticated { get; set; }
    public string? Token { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
}

public interface IAuthenticationService
{
    Task<(bool Success, string Message, LoginResponse? Response)> LoginAsync(LoginRequest request);
    Task<(bool Success, string Message)> RegisterAsync(RegisterRequest request);
    Task<(bool Success, UserProfile? Profile)> GetProfileAsync();
    Task LogoutAsync();
    event Action<AuthenticationState>? AuthenticationStateChanged;
    AuthenticationState GetAuthenticationState();
}

public class AuthenticationService : IAuthenticationService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AuthenticationService> _logger;
    private AuthenticationState _authState = new();

    public event Action<AuthenticationState>? AuthenticationStateChanged;

    public AuthenticationService(HttpClient httpClient, ILogger<AuthenticationService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        
        // Initialize authentication state (could load from localStorage in a real app)
        _authState = new AuthenticationState();
    }

    public async Task<(bool Success, string Message, LoginResponse? Response)> LoginAsync(LoginRequest request)
    {
        try
        {
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/auth/login", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var loginResponse = JsonSerializer.Deserialize<LoginResponse>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (loginResponse != null)
                {
                    // Update authentication state
                    _authState = new AuthenticationState
                    {
                        IsAuthenticated = true,
                        Token = loginResponse.Token,
                        Username = loginResponse.Username,
                        Email = loginResponse.Email
                    };

                    // Set authorization header for future requests
                    _httpClient.DefaultRequestHeaders.Authorization = 
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResponse.Token);

                    // Notify subscribers
                    AuthenticationStateChanged?.Invoke(_authState);

                    _logger.LogInformation("User {Username} logged in successfully", loginResponse.Username);
                    return (true, "Login successful", loginResponse);
                }
            }

            // Handle error response
            var errorMessage = "Login failed";
            try
            {
                var errorResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);
                if (errorResponse.TryGetProperty("message", out var message))
                {
                    errorMessage = message.GetString() ?? errorMessage;
                }
            }
            catch
            {
                // Use default error message if parsing fails
            }

            _logger.LogWarning("Login failed for user {Username}: {Error}", request.Username, errorMessage);
            return (false, errorMessage, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for user {Username}", request.Username);
            return (false, "An error occurred during login. Please try again.", null);
        }
    }

    public async Task<(bool Success, string Message)> RegisterAsync(RegisterRequest request)
    {
        try
        {
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/auth/register", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("User {Username} registered successfully", request.Username);
                return (true, "Registration successful! You can now login.");
            }

            // Handle error response
            var errorMessage = "Registration failed";
            try
            {
                var errorResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);
                if (errorResponse.TryGetProperty("message", out var message))
                {
                    errorMessage = message.GetString() ?? errorMessage;
                }
            }
            catch
            {
                // Use default error message if parsing fails
            }

            _logger.LogWarning("Registration failed for user {Username}: {Error}", request.Username, errorMessage);
            return (false, errorMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration for user {Username}", request.Username);
            return (false, "An error occurred during registration. Please try again.");
        }
    }

    public async Task<(bool Success, UserProfile? Profile)> GetProfileAsync()
    {
        try
        {
            if (!_authState.IsAuthenticated)
            {
                return (false, null);
            }

            var response = await _httpClient.GetAsync("/auth/profile");
            
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var profile = JsonSerializer.Deserialize<UserProfile>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return (true, profile);
            }

            return (false, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user profile");
            return (false, null);
        }
    }

    public Task LogoutAsync()
    {
        // Clear authentication state
        _authState = new AuthenticationState();
        
        // Clear authorization header
        _httpClient.DefaultRequestHeaders.Authorization = null;
        
        // Notify subscribers
        AuthenticationStateChanged?.Invoke(_authState);
        
        _logger.LogInformation("User logged out");
        
        return Task.CompletedTask;
    }

    public AuthenticationState GetAuthenticationState()
    {
        return _authState;
    }
}
