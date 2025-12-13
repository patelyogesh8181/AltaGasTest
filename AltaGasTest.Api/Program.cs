using AltaGasTest.Api.Services;
using AltaGasTest.Data;
using AltaGasTest.Data.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Configure logging
builder.Services.AddLogging(config =>
{
    config.ClearProviders();
    config.AddConsole();
    config.AddDebug();
    
    if (!builder.Environment.IsProduction())
    {
        config.SetMinimumLevel(LogLevel.Debug);
    }
});

// CORS configuration
const string corsPolicyName = "AllowBlazorClient";
builder.Services.AddCors(options =>
{
    options.AddPolicy(corsPolicyName, policy =>
    {
        policy.WithOrigins(
                "https://localhost:5101", // Blazor WASM https
                "http://localhost:5100"   // Blazor WASM http
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// Database configuration
builder.Services.AddDbContext<AltaGasTestDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found."),
        x => x.MigrationsAssembly("AltaGasTest.Data")
    )
);

// Repository and service registration
builder.Services.AddScoped<ICanadianCityRepository, CanadianCityRepository>();
builder.Services.AddScoped<ITripRepository, TripRepository>();
builder.Services.AddScoped<IEquipmentEventRepository, EquipmentEventRepository>();
builder.Services.AddScoped<ITripServices, TripServices>();

// API configuration
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger/OpenAPI configuration
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "AltaGasTest API",
        Version = "v1",
        Description = "API for managing and processing equipment trip events"
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "AltaGasTest API v1");
    });
}

// CORS
app.UseCors(corsPolicyName);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Log application startup
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("AltaGasTest API started in {Environment} environment", app.Environment.EnvironmentName);

app.Run();
