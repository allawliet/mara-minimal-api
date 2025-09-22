using System.Security.Claims;
using Microsoft.JSInterop;
using System.Text.Json;

namespace imas.Web3.Services
{
    public class AuthenticationService
    {
        private readonly IJSRuntime _jsRuntime;
        private bool _isAuthenticated = false;
        private ClaimsPrincipal? _currentUser;
        private bool _initialized = false;

        public event Action<ClaimsPrincipal?>? AuthenticationStateChanged;

        public bool IsAuthenticated => _isAuthenticated;
        public ClaimsPrincipal? CurrentUser => _currentUser;

        public AuthenticationService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task InitializeAsync()
        {
            if (_initialized) return;

            try
            {
                // Try to restore authentication state from localStorage
                var storedAuth = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authState");
                if (!string.IsNullOrEmpty(storedAuth))
                {
                    var authData = JsonSerializer.Deserialize<AuthStateData>(storedAuth);
                    if (authData != null && authData.IsAuthenticated)
                    {
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, authData.Username ?? ""),
                            new Claim(ClaimTypes.Email, authData.Email ?? ""),
                            new Claim("Department", authData.Department ?? ""),
                            new Claim("Role", authData.Role ?? "")
                        };

                        _currentUser = new ClaimsPrincipal(new ClaimsIdentity(claims, "custom"));
                        _isAuthenticated = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing auth state: {ex.Message}");
            }

            _initialized = true;
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            // Simulate authentication delay
            await Task.Delay(1000);

            // Simple authentication logic - replace with real implementation
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                // For demo, accept any non-empty credentials
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Email, $"{username}@mara.gov.my"),
                    new Claim("Department", "IT"),
                    new Claim("Role", "User")
                };

                _currentUser = new ClaimsPrincipal(new ClaimsIdentity(claims, "custom"));
                _isAuthenticated = true;

                // Save authentication state to localStorage
                var authData = new AuthStateData
                {
                    IsAuthenticated = true,
                    Username = username,
                    Email = $"{username}@mara.gov.my",
                    Department = "IT",
                    Role = "User"
                };

                var authJson = JsonSerializer.Serialize(authData);
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "authState", authJson);

                // Notify subscribers about the authentication state change
                AuthenticationStateChanged?.Invoke(_currentUser);

                return true;
            }

            return false;
        }

        public async Task LogoutAsync()
        {
            await Task.Delay(500);
            
            _currentUser = null;
            _isAuthenticated = false;

            // Clear localStorage
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authState");

            // Notify subscribers about the authentication state change
            AuthenticationStateChanged?.Invoke(null);
        }

        public async Task<ClaimsPrincipal?> GetCurrentUserAsync()
        {
            return await Task.FromResult(_currentUser);
        }
    }

    public class AuthStateData
    {
        public bool IsAuthenticated { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Department { get; set; }
        public string? Role { get; set; }
    }
}