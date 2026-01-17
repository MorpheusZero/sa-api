using SoulArenasAPI.Services;
using SoulArenasAPI.Models.Auth;

namespace SoulArenas.Tests.UnitTests;

public class RBACServiceTests
{
    [Fact]
    public void GetPermissionsForRole_WithSuperAdminRole_ReturnsCorrectPermissions()
    {
        // Arrange
        var rbacService = new RBACService();

        // Act
        var permissions = rbacService.GetPermissionsForRole(RoleConstants.SUPER_ADMIN);

        // Assert
        Assert.NotNull(permissions);
        Assert.Single(permissions);
        Assert.Contains(PermissionsConstants.SUPER_ADMIN, permissions);
    }

    [Fact]
    public void GetPermissionsForRole_WithUnknownRole_ReturnsEmptyList()
    {
        // Arrange
        var rbacService = new RBACService();

        // Act
        var permissions = rbacService.GetPermissionsForRole("UNKNOWN_ROLE");

        // Assert
        Assert.NotNull(permissions);
        Assert.Empty(permissions);
    }

    [Fact]
    public void GetPermissionsForRole_ReturnsNonNullList()
    {
        // Arrange
        var rbacService = new RBACService();

        // Act
        var permissions1 = rbacService.GetPermissionsForRole(RoleConstants.SUPER_ADMIN);
        var permissions2 = rbacService.GetPermissionsForRole("NONEXISTENT_ROLE");
        var permissions3 = rbacService.GetPermissionsForRole("");

        // Assert
        Assert.NotNull(permissions1);
        Assert.NotNull(permissions2);
        Assert.NotNull(permissions3);
    }
}
