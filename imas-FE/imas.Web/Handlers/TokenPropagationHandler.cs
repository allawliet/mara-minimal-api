using System.Net.Http.Headers;
using imas.Web.Services;

namespace imas.Web.Handlers;

public class TokenPropagationHandler : DelegatingHandler
{
    private readonly IEnhancedAuthenticationService _authService;

    public TokenPropagationHandler(IEnhancedAuthenticationService authService)
    {
        _authService = authService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Only add token if Authorization header is not already set
        if (request.Headers.Authorization == null)
        {
            var token = await _authService.GetTokenAsync();
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        return await base.SendAsync(request, cancellationToken);
    }
}