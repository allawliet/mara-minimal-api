using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace imas.Web.Services;

public interface ISafeNavigationService
{
    Task NavigateToAsync(string uri, bool forceLoad = false);
    Task NavigateToAsync(string uri, NavigationOptions options);
}

public class SafeNavigationService : ISafeNavigationService
{
    private readonly NavigationManager _navigationManager;
    private readonly IJSRuntime _jsRuntime;
    private bool _isInteractiveRenderingComplete = false;

    public SafeNavigationService(NavigationManager navigationManager, IJSRuntime jsRuntime)
    {
        _navigationManager = navigationManager;
        _jsRuntime = jsRuntime;
    }

    public async Task NavigateToAsync(string uri, bool forceLoad = false)
    {
        await NavigateToAsync(uri, new NavigationOptions { ForceLoad = forceLoad });
    }

    public async Task NavigateToAsync(string uri, NavigationOptions options)
    {
        try
        {
            // First try the standard NavigationManager
            _navigationManager.NavigateTo(uri, options);
        }
        catch (InvalidOperationException)
        {
            // This is likely the NavigationException during prerendering
            // Fallback to JavaScript navigation
            await NavigateWithJavaScriptAsync(uri, options.ForceLoad);
        }
        catch (Exception)
        {
            // Last resort: Use JavaScript location change
            await NavigateWithJavaScriptAsync(uri, options.ForceLoad);
        }
    }

    private async Task<bool> CanNavigateSafelyAsync()
    {
        try
        {
            // Try to access the current URI - this will throw during prerendering
            var currentUri = _navigationManager.Uri;
            
            // Check if JavaScript is available (indicates interactive rendering)
            await _jsRuntime.InvokeAsync<string>("eval", "window.location.href");
            
            return true;
        }
        catch
        {
            return false;
        }
    }

    private async Task NavigateWithJavaScriptAsync(string uri, bool forceLoad)
    {
        try
        {
            if (forceLoad)
            {
                await _jsRuntime.InvokeVoidAsync("window.location.assign", uri);
            }
            else
            {
                await _jsRuntime.InvokeVoidAsync("window.location.href = arguments[0]", uri);
            }
        }
        catch
        {
            // If JavaScript also fails, we're probably in a context where navigation isn't possible
            // This is expected during static rendering and can be safely ignored
        }
    }
}