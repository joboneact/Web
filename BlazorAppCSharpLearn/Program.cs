using BlazorAppCSharpLearn.Components;
using BlazorAppCSharpLearn.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add HTTP client for weather API calls
builder.Services.AddHttpClient();

// Register services
builder.Services.AddScoped<WeatherService>();
builder.Services.AddScoped<MovieService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
// Redirect HTTP requests to HTTPS for security.
app.UseHttpsRedirection();

// Enable antiforgery protection for requests (helps prevent CSRF attacks).
app.UseAntiforgery();

// Map static assets (like CSS, JS, images) to be served by the app.
app.MapStaticAssets();

// Map the root Razor component (App) and enable interactive server-side rendering.
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Start the web application and begin listening for requests.
app.Run();
