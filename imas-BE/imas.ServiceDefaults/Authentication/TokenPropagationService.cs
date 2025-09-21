using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;

namespace imas.ServiceDefaults.Authentication;

public interface ITokenPropagationService
{
    Task<string?> GetCurrentTokenAsync();
    void SetAuthorizationHeader(HttpClient httpClient, string? token = null);
    Task SetAuthorizationHeaderAsync(HttpClient httpClient);
}

public class TokenPropagationService : ITokenPropagationService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ICentralizedTokenService _tokenService;

    public TokenPropagationService(IHttpContextAccessor httpContextAccessor, ICentralizedTokenService tokenService)
    {
        _httpContextAccessor = httpContextAccessor;
        _tokenService = tokenService;
    }

    public Task<string?> GetCurrentTokenAsync()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context == null) return Task.FromResult<string?>(null);

        // Try to get token from Authorization header
        var authHeader = context.Request.Headers.Authorization.FirstOrDefault();
        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
        {
            var token = authHeader.Substring("Bearer ".Length).Trim();
            if (_tokenService.IsTokenValid(token))
            {
                return Task.FromResult<string?>(token);
            }
        }

        // Try to get token from user claims (if authenticated)
        var user = context.User;
        if (user?.Identity?.IsAuthenticated == true)
        {
            var username = user.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
            var email = user.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            var roles = user.FindAll(System.Security.Claims.ClaimTypes.Role).Select(c => c.Value).ToList();

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(email))
            {
                // Generate a fresh token based on current user claims
                var newToken = _tokenService.GenerateToken(username, email, roles);
                return Task.FromResult<string?>(newToken);
            }
        }

        return Task.FromResult<string?>(null);
    }

    public void SetAuthorizationHeader(HttpClient httpClient, string? token = null)
    {
        if (!string.IsNullOrEmpty(token))
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        else
        {
            httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }

    public async Task SetAuthorizationHeaderAsync(HttpClient httpClient)
    {
        var token = await GetCurrentTokenAsync();
        SetAuthorizationHeader(httpClient, token);
    }
}

// HTTP Client Delegating Handler for automatic token injection
public class TokenPropagationHandler : DelegatingHandler
{
    private readonly ITokenPropagationService _tokenPropagationService;

    public TokenPropagationHandler(ITokenPropagationService tokenPropagationService)
    {
        _tokenPropagationService = tokenPropagationService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Only add token if Authorization header is not already set
        if (request.Headers.Authorization == null)
        {
            var token = await _tokenPropagationService.GetCurrentTokenAsync();
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        return await base.SendAsync(request, cancellationToken);
    }
}