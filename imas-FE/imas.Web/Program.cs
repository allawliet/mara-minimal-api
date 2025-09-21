using imas.Web;
using imas.Web.Components;
using imas.Web.Services;
using imas.Web.Handlers;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddOutputCache();

// Register services first
builder.Services.AddScoped<EnhancedAuthenticationService>();
builder.Services.AddTransient<TokenPropagationHandler>();

// Add HTTP clients with proper configuration
builder.Services.AddHttpClient<WeatherApiClient>(client =>
    {
        // This URL uses "https+http://" to indicate HTTPS is preferred over HTTP.
        // Learn more about service discovery scheme resolution at https://aka.ms/dotnet/sdschemes.
        client.BaseAddress = new("https+http://api-gateway");
    })
    .AddHttpMessageHandler<TokenPropagationHandler>();

// Add HTTP client for enhanced authentication service
builder.Services.AddHttpClient<EnhancedAuthenticationService>(client =>
    {
        client.BaseAddress = new("https+http://api-gateway");
    });

// Keep original authentication service for backward compatibility
builder.Services.AddHttpClient<IAuthenticationService, AuthenticationService>(client =>
    {
        client.BaseAddress = new("https+http://api-gateway");
    });

// Register the enhanced authentication service interfaces
builder.Services.AddScoped<IEnhancedAuthenticationService>(provider => 
    provider.GetRequiredService<EnhancedAuthenticationService>());
builder.Services.AddScoped<IAuthenticationService>(provider => 
    provider.GetRequiredService<EnhancedAuthenticationService>());

// Register safe navigation service
builder.Services.AddScoped<ISafeNavigationService, SafeNavigationService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseOutputCache();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapDefaultEndpoints();

app.Run();
