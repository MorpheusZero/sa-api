namespace SoulArenasAPI.Controllers;

using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;
using SoulArenasAPI.Models.DTOs;
using SoulArenasAPI.Services;
using SoulArenasAPI.Util;

public static class AuthController
{
    public static void MapRoutes(this WebApplication app)
    {
        app.MapPost("/auth/register", async (AuthService authService, UserCreateRequestDTO userCreateRequest) =>
        {
            try
            {
            if (string.IsNullOrWhiteSpace(userCreateRequest.Email) || string.IsNullOrWhiteSpace(userCreateRequest.Password))
            {
                return Results.BadRequest("email and password are required fields.");
            }

            return Results.Ok(await authService.RegisterNewUser(userCreateRequest));
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }).AllowAnonymous();

        app.MapPost("/auth/login", static async (HttpContext context, IConfiguration configuration, AuthService authService) =>
        {
            try
            {
                // Read Authorization header
                if (!context.Request.Headers.TryGetValue("Authorization", out var authHeader))
                {
                    return Results.Unauthorized();
                }

                var authHeaderValue = authHeader.ToString();

                // Check if it's Basic auth
                if (!authHeaderValue.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("Invalid authorization scheme. Expected Basic auth.");
                }

                // Extract and decode the base64 credentials
                var encodedCredentials = authHeaderValue.Substring("Basic ".Length).Trim();
                var decodedCredentials = System.Text.Encoding.UTF8.GetString(
                    Convert.FromBase64String(encodedCredentials));

                // Split email:password
                var credentials = decodedCredentials.Split(':', 2);
                if (credentials.Length != 2)
                {
                    return Results.BadRequest("Invalid credentials format.");
                }

                var userLoginRequest = new UserLoginRequestDTO
                {
                    Email = credentials[0],
                    Password = credentials[1]
                };

                if (string.IsNullOrWhiteSpace(userLoginRequest.Email) ||
                    string.IsNullOrWhiteSpace(userLoginRequest.Password))
                {
                    return Results.BadRequest("Email and password are required.");
                }

                var user = await authService.LoginUser(userLoginRequest);

                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.Lax,
                    Secure = false,
                    MaxAge = TimeSpan.FromMinutes(15),
                    Domain = "localhost",
                    Path = "/"
                };

                var accessToken = JWTHelper.GenerateAccessTokenForUser(configuration.GetValue<string>("JWTSecretKey")!, user.Id.ToString());

                context.Response.Cookies.Append("x-access-token", accessToken, cookieOptions);

                return Results.Ok();
            }
            catch (FormatException)
            {
                return Results.BadRequest("Invalid authorization header format.");
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }).AllowAnonymous();     

        app.MapGet("/auth/token", (HttpContext context) => 
        {
            var authorizedUser = context.Items[Constants.AuthorizedUserContextString] as Database.Entities.UserEntity;
            
            return Results.Ok(new { Message = $"Hello user {authorizedUser?.Username}, this is private data." });
        })
        .RequireAuthorization(); // Requires a valid JWT   
    }
}
