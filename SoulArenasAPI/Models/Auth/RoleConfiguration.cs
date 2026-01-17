namespace SoulArenasAPI.Models.Auth;

// This class maps roles to their associated permissions in a static way so the service can be minimal.
public class RoleConfiguration
{
    public static readonly Dictionary<string, List<string>> GetRoleConfiguration = new()
    {
        // SUPER_ADMIN gives access to everything--overrides anything else.
        { RoleConstants.SUPER_ADMIN, new List<string> { PermissionsConstants.SUPER_ADMIN } }
    };
}
