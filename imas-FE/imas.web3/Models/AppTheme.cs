namespace imas.Web3.Models;

public class AppTheme
{
    public string Name { get; set; } = string.Empty;
    public bool IsDark { get; set; }
    public string PrimaryColor { get; set; } = "#1976d2";
    public string SecondaryColor { get; set; } = "#dc004e";
    public string BackgroundColor { get; set; } = "#fafafa";
    public string SurfaceColor { get; set; } = "#ffffff";
    public string TextColor { get; set; } = "#212121";
    
    public static AppTheme Light => new()
    {
        Name = "Light",
        IsDark = false,
        PrimaryColor = "#1976d2",
        SecondaryColor = "#dc004e",
        BackgroundColor = "#fafafa",
        SurfaceColor = "#ffffff",
        TextColor = "#212121"
    };
    
    public static AppTheme Dark => new()
    {
        Name = "Dark",
        IsDark = true,
        PrimaryColor = "#90caf9",
        SecondaryColor = "#f48fb1",
        BackgroundColor = "#000000",
        SurfaceColor = "#121212",
        TextColor = "#ffffff"
    };
}