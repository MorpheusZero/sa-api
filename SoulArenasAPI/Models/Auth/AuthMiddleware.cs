using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SoulArenasAPI.Services;
using SoulArenasAPI.Util;

namespace SoulArenasAPI.Models.Auth;

public class AuthMiddlewareHandler
{
    public static async Task OnTokenValidated(TokenValidatedContext context)
    {
        var userId = context.Principal?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            context.Fail("Unauthorized: No sub claim found.");
            return;
        }

        var userService = context.HttpContext.RequestServices.GetRequiredService<UserService>();

        var userEntity = await userService.GetUserById(int.Parse(userId));
        if (userEntity == null)
        {
            context.Fail("Unauthorized: User no longer exists in DB.");
            return;
        }

        var rbacService = context.HttpContext.RequestServices.GetRequiredService<RBACService>();

        foreach (var role in userEntity.RolesRaw.Split(',', StringSplitOptions.RemoveEmptyEntries))
        {
            var trimmedRole = role.Trim();
            var permissions = rbacService.GetPermissionsForRole(trimmedRole);
            foreach (var permission in permissions)
            {
                userEntity.Permissions.Add(permission);
            }
        }

        context.HttpContext.Items[Constants.AuthorizedUserContextString] = userEntity;
    }
}
