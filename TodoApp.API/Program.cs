using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TodoApp.API.Extensions;
using TodoApp.API.Middleware;
using TodoApp.Application;
using TodoApp.Infrastructure;
using TodoApp.Infrastructure.Data;
using TodoApp.Infrastructure.Data.Seeders;

var builder = WebApplication.CreateBuilder(args);

// --- 1. KONFIGURASI PORT (WAJIB UNTUK RAILWAY) ---
// Railway akan menyuntikkan nomor port ke environment variable "PORT"
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger (Sesuai kode asli Anda)
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TodoApp API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        In = ParameterLocation.Header
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        { new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }, Array.Empty<string>() }
    });
});

// Layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// JWT & Context
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddHttpContextAccessor();

// --- 2. CORS DINAMIS (PENTING AGAR BISA DIAKSES FRONTEND) ---
builder.Services.AddCors(options =>
{
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("BlazorPolicy", policy =>
        {
            policy.WithOrigins(
                    "https://simpletaskapp.up.railway.app",  
                    "https://localhost:7001",
                    "http://localhost:5001")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
    });
});

var app = builder.Build();

// Auto migrate & seed
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        
        // Cek pending migrations dulu sebelum migrate
        var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
        if (pendingMigrations.Any())
        {
            await context.Database.MigrateAsync();
        }
        
        await DataSeeder.SeedAsync(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Error saat migration database");
    }
}

// --- 3. SWAGGER DI DEVELOPMENT & RAILWAY ---
// Kita aktifkan Swagger di Railway agar Anda mudah melakukan testing API awal
if (app.Environment.IsDevelopment() || Environment.GetEnvironmentVariable("RAILWAY_ENVIRONMENT") != null)
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TodoApp API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

// --- 4. HTTPS REDIRECTION (CATATAN) ---
// Di Railway, HTTPS dihandle oleh mereka. Jika muncul error "Too many redirects", 
// matikan (comment) baris UseHttpsRedirection di bawah ini.
if (Environment.GetEnvironmentVariable("RAILWAY_ENVIRONMENT") == null)
{
    app.UseHttpsRedirection();
}

app.UseCors("BlazorPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();