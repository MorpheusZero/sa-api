using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SoulArenasAPI.Controllers;
using SoulArenasAPI.Database;
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
                // 1. Get the UserID from the verified claims
                var userId = context.Principal?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    context.Fail("Unauthorized: No sub claim found.");
                    return;
                }

                // 2. Get your DB context from the request services
                var userService = context.HttpContext.RequestServices.GetRequiredService<UserService>();
                
                // 3. Look up the user
                var userEntity = await userService.GetUserById(int.Parse(userId));
                if (userEntity == null)
                {
                    context.Fail("Unauthorized: User no longer exists in DB.");
                    return;
                }

                // 4. Inject the User Entity into HttpContext for easy access in handlers
                context.HttpContext.Items[Constants.AuthorizedUserContextString] = userEntity;

                // 5. Optionally add extra claims dynamically (e.g., from DB permissions)
                // var appIdentity = new ClaimsIdentity(new[] {
                //     new Claim("user_status", userEntity.Status)
                // });
                // context.Principal?.AddIdentity(appIdentity);
            }
        };
    });

builder.Services.AddAuthorization();

// Add Database Context
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(9, 0, 0))), ServiceLifetime.Singleton);

// Add services to the container.;
builder.Services.AddSingleton<HealthService>();
builder.Services.AddSingleton<UserService>();
builder.Services.AddSingleton<AuthService>();

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

                   
