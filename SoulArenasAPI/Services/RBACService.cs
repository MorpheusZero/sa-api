namespace SoulArenasAPI.Services;

using SoulArenasAPI.Models.Auth;

public class RBACService
{
    private readonly Dictionary<string, List<string>> _rolePermissionsConfig;

    public RBACService()
    {
        _rolePermissionsConfig = RoleConfiguration.GetRoleConfiguration;
    }

    public List<string> GetPermissionsForRole(string role)
    {
        if (_rolePermissionsConfig.TryGetValue(role, out var permissions))
        {
            return permissions;
        }

        return new List<string>();
    }
}
