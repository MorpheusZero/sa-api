using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SoulArenasAPI.Controllers;
using SoulArenasAPI.Database;
using SoulArenasAPI.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
});

// Add Database Context
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(9, 0, 0))));

// Add services to the container.;
builder.Services.AddSingleton<HealthService>();
builder.Services.AddScoped<UserService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

// Map controller routes
HealthController.MapRoutes(app);
AuthController.MapRoutes(app);

app.Run();

                   
