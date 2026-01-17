namespace SoulArenasAPI.Models.Auth;

using Microsoft.AspNetCore.Authorization;
using SoulArenasAPI.Database.Entities;
using SoulArenasAPI.Util;
using System.Threading.Tasks;

public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        if (context.Resource is HttpContext httpContext)
        {
            var authorizedUser = httpContext.Items[Constants.AuthorizedUserContextString] as UserEntity;

            if (authorizedUser != null &&
                authorizedUser.Permissions.Contains(requirement.Permission))
            {
                context.Succeed(requirement);
            }
        }

        return Task.CompletedTask;
    }
}
