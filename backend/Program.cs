using Microsoft.EntityFrameworkCore;
using MovieActorApi.Data;
using MovieActorApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://localhost:5080");

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "Connection string 'DefaultConnection' is missing. For local development, set it in appsettings.Development.json.");
}

builder.Services.AddControllers();

// Dependency injection: one AppDbContext per HTTP request, shared with the services below.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<MovieService>();
builder.Services.AddScoped<ActorService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Vite", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors("Vite");
app.MapControllers();

app.Run();
