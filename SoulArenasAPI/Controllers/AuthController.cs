namespace SoulArenasAPI.Controllers;

using SoulArenasAPI.Models.DTOs;
using SoulArenasAPI.Services;

public static class AuthController
{
    public static void MapRoutes(this WebApplication app)
    {
        app.MapPost("/auth/register", async (UserService userService, UserCreateRequestDTO userCreateRequest) => 
            Results.Ok(await userService.CreateUser(userCreateRequest)));
    }
}
