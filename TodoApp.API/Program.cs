using TodoApp.API.Extensions;
using TodoApp.API.Middleware;
using TodoApp.Application;
using TodoApp.Infrastructure;
using TodoApp.Infrastructure.Data;
using TodoApp.Infrastructure.Data.Seeders;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ??? Services ????????????????????????????????????????????????????
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger dengan JWT support
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TodoApp API",
        Version = "v1",
        Description = "Enterprise Todo Application API"
    });

    // JWT di Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Masukkan: Bearer {token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// JWT
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddHttpContextAccessor();

// CORS untuk Blazor
builder.Services.AddCors(options =>
{
    options.AddPolicy("BlazorPolicy", policy =>
    {
        policy.WithOrigins(
                "https://localhost:7001",  // sesuaikan port Blazor
                "http://localhost:5001")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// ??? Pipeline ????????????????????????????????????????????????????
var app = builder.Build();

// Auto migrate & seed
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await context.Database.EnsureCreatedAsync();
    await DataSeeder.SeedAsync(context);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TodoApp API v1");
        c.RoutePrefix = string.Empty; // Swagger di root URL
    });
}

app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseHttpsRedirection();
app.UseCors("BlazorPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();