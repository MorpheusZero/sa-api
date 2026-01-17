namespace SoulArenasAPI.Models.Auth;
 
 public class AuthPolicyConfiguration
 {
    public static readonly Dictionary<string, string> GetAuthPolicyConfiguration = new()
    {
        // Require SUPER_ADMIN permission for SUPER_ADMIN_POLICY
        { AuthPolicyConstants.SUPER_ADMIN_POLICY, PermissionsConstants.SUPER_ADMIN }
    };
 }