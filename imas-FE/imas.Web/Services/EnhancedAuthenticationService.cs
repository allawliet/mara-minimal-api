using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.JSInterop;

namespace imas.Web.Services;

public interface IEnhancedAuthenticationService : IAuthenticationService
{
    Task<AuthResult> LoginAsync(string username, string password);
    Task<AuthResult> RegisterAsync(string username, string email, string password);
    new Task LogoutAsync();
    Task<bool> IsAuthenticatedAsync();
    Task<string?> GetTokenAsync();
    Task<UserInfo?> GetUserInfoAsync();
    Task InitializeAsync();
}

public record AuthResult(bool Success, string? Token = null, string? ErrorMessage = null);
public record UserInfo(string Username, string Email, List<string> Roles);

public class EnhancedAuthenticationService : IEnhancedAuthenticationService
{
    private readonly HttpClient _httpClient;
    private readonly IJSRuntime _jsRuntime;
    private readonly AuthenticationStateProvider _authStateProvider;
    private const string TokenKey = "authToken";
    private const string UserInfoKey = "userInfo";

    // In-memory fallback for when JavaScript is not available (during prerendering)
    private string? _memoryToken;
    private UserInfo? _memoryUserInfo;
    private bool _jsAvailable = false;

    public event Action<AuthenticationState>? AuthenticationStateChanged;

    public EnhancedAuthenticationService(
        HttpClient httpClient,
        IJSRuntime jsRuntime,
        AuthenticationStateProvider authStateProvider)
    {
        _httpClient = httpClient;
        _jsRuntime = jsRuntime;
        _authStateProvider = authStateProvider;
    }

    private async Task<bool> IsJavaScriptAvailableAsync()
    {
        if (_jsAvailable) return true;
        
        try
        {
            await _jsRuntime.InvokeAsync<string>("eval", "window");
            _jsAvailable = true;
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<AuthResult> LoginAsync(string username, string password)
    {
        try
        {
            var loginRequest = new
            {
                Username = username,
                Password = password
            };

            var response = await _httpClient.PostAsJsonAsync("/auth/login", loginRequest);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var loginResponse = JsonSerializer.Deserialize<LoginResponse>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (loginResponse?.Token != null)
                {
                    // Store in memory first
                    _memoryToken = loginResponse.Token;
                    var userInfo = new UserInfo(loginResponse.Username, loginResponse.Email, new List<string>());
                    _memoryUserInfo = userInfo;

                    // Try to store in localStorage if JavaScript is available
                    if (await IsJavaScriptAvailableAsync())
                    {
                        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", TokenKey, loginResponse.Token);
                        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", UserInfoKey, JsonSerializer.Serialize(userInfo));
                    }

                    // Set authorization header for future requests
                    _httpClient.DefaultRequestHeaders.Authorization = 
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResponse.Token);

                    // Notify authentication state change
                    AuthenticationStateChanged?.Invoke(GetAuthenticationState());

                    return new AuthResult(true, loginResponse.Token);
                }
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            return new AuthResult(false, ErrorMessage: errorContent);
        }
        catch (Exception ex)
        {
            return new AuthResult(false, ErrorMessage: ex.Message);
        }
    }

    public async Task<AuthResult> RegisterAsync(string username, string email, string password)
    {
        try
        {
            var registerRequest = new
            {
                Username = username,
                Email = email,
                Password = password
            };

            var response = await _httpClient.PostAsJsonAsync("/auth/register", registerRequest);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var loginResponse = JsonSerializer.Deserialize<LoginResponse>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (loginResponse?.Token != null)
                {
                    // Store in memory first
                    _memoryToken = loginResponse.Token;
                    var userInfo = new UserInfo(loginResponse.Username, loginResponse.Email, new List<string>());
                    _memoryUserInfo = userInfo;

                    // Try to store in localStorage if JavaScript is available
                    if (await IsJavaScriptAvailableAsync())
                    {
                        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", TokenKey, loginResponse.Token);
                        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", UserInfoKey, JsonSerializer.Serialize(userInfo));
                    }

                    // Set authorization header
                    _httpClient.DefaultRequestHeaders.Authorization = 
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResponse.Token);

                    // Notify authentication state change
                    AuthenticationStateChanged?.Invoke(GetAuthenticationState());

                    return new AuthResult(true, loginResponse.Token);
                }
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            return new AuthResult(false, ErrorMessage: errorContent);
        }
        catch (Exception ex)
        {
            return new AuthResult(false, ErrorMessage: ex.Message);
        }
    }

    public async Task LogoutAsync()
    {
        // Clear memory storage
        _memoryToken = null;
        _memoryUserInfo = null;

        // Clear localStorage if available
        if (await IsJavaScriptAvailableAsync())
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", TokenKey);
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", UserInfoKey);
        }

        // Clear authorization header
        _httpClient.DefaultRequestHeaders.Authorization = null;

        // Notify authentication state change
        AuthenticationStateChanged?.Invoke(GetAuthenticationState());
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await GetTokenAsync();
        return !string.IsNullOrEmpty(token);
    }

    public async Task<string?> GetTokenAsync()
    {
        // First check memory storage
        if (!string.IsNullOrEmpty(_memoryToken))
            return _memoryToken;

        // Then check localStorage if available
        if (await IsJavaScriptAvailableAsync())
        {
            try
            {
                var token = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", TokenKey);
                if (!string.IsNullOrEmpty(token))
                {
                    _memoryToken = token; // Cache in memory
                    return token;
                }
            }
            catch
            {
                // Handle error silently
            }
        }

        return null;
    }

    public async Task<UserInfo?> GetUserInfoAsync()
    {
        // First check memory storage
        if (_memoryUserInfo != null)
            return _memoryUserInfo;

        // Then check localStorage if available
        if (await IsJavaScriptAvailableAsync())
        {
            try
            {
                var userInfoJson = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", UserInfoKey);
                if (!string.IsNullOrEmpty(userInfoJson))
                {
                    var userInfo = JsonSerializer.Deserialize<UserInfo>(userInfoJson);
                    _memoryUserInfo = userInfo; // Cache in memory
                    return userInfo;
                }
            }
            catch
            {
                // Handle error silently
            }
        }

        return null;
    }

    public async Task InitializeAsync()
    {
        // Initialize the service by setting up the HTTP client with stored token
        var token = await GetTokenAsync();
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
    }

    // IAuthenticationService interface implementation
    public async Task<(bool Success, string Message, LoginResponse? Response)> LoginAsync(LoginRequest request)
    {
        var result = await LoginAsync(request.Username, request.Password);
        if (result.Success && !string.IsNullOrEmpty(result.Token))
        {
            var userInfo = await GetUserInfoAsync();
            var response = new LoginResponse
            {
                Token = result.Token,
                Username = userInfo?.Username ?? request.Username,
                Email = userInfo?.Email ?? ""
            };
            return (true, "Login successful", response);
        }
        return (false, result.ErrorMessage ?? "Login failed", null);
    }

    public async Task<(bool Success, string Message)> RegisterAsync(RegisterRequest request)
    {
        var result = await RegisterAsync(request.Username, request.Email, request.Password);
        return (result.Success, result.ErrorMessage ?? (result.Success ? "Registration successful" : "Registration failed"));
    }

    public async Task<(bool Success, UserProfile? Profile)> GetProfileAsync()
    {
        var userInfo = await GetUserInfoAsync();
        if (userInfo != null)
        {
            var profile = new UserProfile
            {
                Username = userInfo.Username,
                Email = userInfo.Email
            };
            return (true, profile);
        }
        return (false, null);
    }

    public AuthenticationState GetAuthenticationState()
    {
        // Use memory values during prerendering to avoid JavaScript interop
        var isAuth = !string.IsNullOrEmpty(_memoryToken);
        var token = _memoryToken;
        var userInfo = _memoryUserInfo;
        
        return new AuthenticationState
        {
            IsAuthenticated = isAuth,
            Token = token,
            Username = userInfo?.Username,
            Email = userInfo?.Email
        };
    }
}