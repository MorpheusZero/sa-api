namespace SoulArenasAPI.Controllers;

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

                var accessTokenCookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.Lax,
                    Secure = false,
                    MaxAge = TimeSpan.FromMinutes(15),
                    Domain = "localhost",
                    Path = "/"
                };

                var accessToken = JWTHelper.GenerateAccessTokenForUser(configuration.GetValue<string>("JWTSecretKey")!, user.Id.ToString());

                context.Response.Cookies.Append("x-access-token", accessToken, accessTokenCookieOptions);

                var refreshToken = await authService.GenerateRefreshTokenForUser(
                    user.Id,
                    StringHelper.GetDeviceInfoFromUserAgent(context.Request.Headers["User-Agent"].ToString() ?? "unknown"),
                    context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    context.Request.Headers["User-Agent"].ToString() ?? "unknown"
                );

                var refreshTokenCookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.Lax,
                    Secure = false,
                    MaxAge = TimeSpan.FromDays(7),
                    Domain = "localhost",
                    Path = "/auth/refresh"
                };

                context.Response.Cookies.Append("x-refresh-token", refreshToken, refreshTokenCookieOptions);

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

        app.MapPost("/auth/refresh", static async (HttpContext context, IConfiguration configuration, AuthService authService) =>
        {
            try
            {
                // Get refresh token from cookies
                if (!context.Request.Cookies.TryGetValue("x-refresh-token", out var refreshToken))
                {
                    return Results.Unauthorized();
                }

                if (string.IsNullOrWhiteSpace(refreshToken))
                {
                    return Results.Unauthorized();
                }

                var parts = refreshToken.Split('.', 2);
                if (parts.Length != 2)
                {
                    return Results.Unauthorized();
                }

                var refreshTokenId = int.Parse(parts[0]);
                var refreshTokenString = parts[1];

                var user = await authService.ValidateRefreshToken(refreshTokenId, refreshTokenString);
                if (user == null)
                {
                    return Results.Unauthorized();
                }

                var newAccessToken = JWTHelper.GenerateAccessTokenForUser(configuration.GetValue<string>("JWTSecretKey")!, user.Id.ToString());

                var accessTokenCookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.Lax,
                    Secure = false,
                    MaxAge = TimeSpan.FromMinutes(15),
                    Domain = "localhost",
                    Path = "/"
                };

                context.Response.Cookies.Append("x-access-token", newAccessToken, accessTokenCookieOptions);

                var newRefreshToken = await authService.GenerateRefreshTokenForUser(
                    user.Id,
                    StringHelper.GetDeviceInfoFromUserAgent(context.Request.Headers["User-Agent"].ToString() ?? "unknown"),
                    context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    context.Request.Headers["User-Agent"].ToString() ?? "unknown"
                );

                var refreshTokenCookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.Lax,
                    Secure = false,
                    MaxAge = TimeSpan.FromDays(7),
                    Domain = "localhost",
                    Path = "/auth/refresh"
                };

                context.Response.Cookies.Append("x-refresh-token", newRefreshToken, refreshTokenCookieOptions);
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

            return Results.Ok(authorizedUser);
        })
        .RequireAuthorization();
    }
}
