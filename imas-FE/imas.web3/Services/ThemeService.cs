using imas.Web3.Models;

namespace imas.Web3.Services;

public class ThemeService
{
    private AppTheme _currentTheme = AppTheme.Light;
    
    public event Action<AppTheme>? ThemeChanged;
    
    public AppTheme CurrentTheme => _currentTheme;
    
    public void SetTheme(AppTheme theme)
    {
        _currentTheme = theme;
        ApplyThemeToDocument(theme);
        ThemeChanged?.Invoke(theme);
    }
    
    public void ToggleTheme()
    {
        var newTheme = _currentTheme.IsDark ? AppTheme.Light : AppTheme.Dark;
        SetTheme(newTheme);
    }
    
    private void ApplyThemeToDocument(AppTheme theme)
    {
        var themeAttribute = theme.IsDark ? "dark" : "light";
        
        // Update the document's data-theme attribute
        var script = $@"
            document.documentElement.setAttribute('data-theme', '{themeAttribute}');
            
            // Apply CSS custom properties for dynamic theming
            const root = document.documentElement;
            root.style.setProperty('--app-color-primary', '{theme.PrimaryColor}');
            root.style.setProperty('--app-color-secondary', '{theme.SecondaryColor}');
            root.style.setProperty('--app-color-background', '{theme.BackgroundColor}');
            root.style.setProperty('--app-color-surface', '{theme.SurfaceColor}');
            root.style.setProperty('--app-color-on-surface', '{theme.TextColor}');
            root.style.setProperty('--app-color-on-background', '{theme.TextColor}');
        ";
        
        // In a real implementation, you'd use IJSRuntime to execute this script
        // For now, we'll store the script for later execution
        _pendingScript = script;
    }
    
    private string? _pendingScript;
    
    public string? GetPendingScript()
    {
        var script = _pendingScript;
        _pendingScript = null;
        return script;
    }
    
    public async Task InitializeAsync()
    {
        // Initialize with light theme by default
        SetTheme(AppTheme.Light);
        await Task.CompletedTask;
    }
}