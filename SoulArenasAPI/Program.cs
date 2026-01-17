using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SoulArenasAPI.Controllers;
using SoulArenasAPI.Database;
using SoulArenasAPI.Models.Auth;
using SoulArenasAPI.Services;
using SoulArenasAPI.Util;

var builder = WebApplication.CreateBuilder(args);
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
});

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = JWTHelper.Issuer,
            ValidAudience = JWTHelper.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("JWTSecretKey")!))
        };
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                await AuthMiddlewareHandler.OnTokenValidated(context);
            }
        };
    });

// Configure Authorization Policies
builder.Services.AddAuthorization(options =>
{
    foreach (var policyConfig in AuthPolicyConfiguration.GetAuthPolicyConfiguration)
    {
        options.AddPolicy(policyConfig.Key, policy =>
            policy.Requirements.Add(new PermissionRequirement(policyConfig.Value)));
    }
});

// Register the authorization handler
builder.Services.AddSingleton<Microsoft.AspNetCore.Authorization.IAuthorizationHandler, PermissionHandler>();

// Add Database Context
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(9, 0, 0))), ServiceLifetime.Singleton);

// Add services to the container.;
builder.Services.AddSingleton<HealthService>();
builder.Services.AddSingleton<UserService>();
builder.Services.AddSingleton<AuthService>();
builder.Services.AddSingleton<RBACService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// Map controller routes
HealthController.MapRoutes(app);
AuthController.MapRoutes(app);

app.Run();


