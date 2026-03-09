using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using MudBlazor.Services;
using TodoApp.Web;
using TodoApp.Web.Auth;
using TodoApp.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// --- 1. KONFIGURASI PORT (WAJIB UNTUK RAILWAY) ---
var port = Environment.GetEnvironmentVariable("PORT") ?? "8081"; // Default port berbeda dari backend
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents(options =>
    {
        options.DetailedErrors = true;
    });

// MudBlazor (Sesuai kode asli Anda)
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
    config.SnackbarConfiguration.PreventDuplicates = false;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 3000;
});

// --- 2. HTTPCLIENT DINAMIS ---
builder.Services.AddScoped(sp =>
{
    // 1. Coba ambil dari Environment Variable "BACKEND_URL" (Prioritas Railway)
    // 2. Jika kosong, coba ambil dari appsettings.json "ApiBaseUrl"
    // 3. Jika masih kosong, gunakan localhost sebagai pertahanan terakhir
    var backendUrl = builder.Configuration["BACKEND_URL"]
                     ?? builder.Configuration["ApiBaseUrl"]
                     ?? "https://localhost:7000/";

    // Pastikan URL diakhiri dengan garis miring (/) untuk HttpClient
    if (!backendUrl.EndsWith("/")) backendUrl += "/";

    return new HttpClient
    {
        BaseAddress = new Uri(backendUrl)
    };
});

// Autentikasi (Sesuai kode asli Anda)
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "BlazorAuth";
})
.AddCookie("BlazorAuth", options =>
{
    options.LoginPath = "/login";
    options.LogoutPath = "/logout";
    options.AccessDeniedPath = "/login";
});

builder.Services.AddAuthorization();

// Auth Blazor
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<CustomAuthStateProvider>());

// Services
builder.Services.AddScoped<IApiService, ApiService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

// --- 3. HANDLING HTTPS REDIRECT DI RAILWAY ---
// Sama seperti backend, matikan redirect jika di Railway untuk menghindari loop
if (Environment.GetEnvironmentVariable("RAILWAY_ENVIRONMENT") == null)
{
    app.UseHttpsRedirection();
}

app.UseStaticFiles();
app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();