using SoulArenasAPI.Services;

namespace SoulArenas.Tests;

public class HealthServiceTests
{
    [Fact]
    public void GetHealthStatus_ReturnsOk()
    {
        // Arrange
        var healthService = new HealthService();

        // Act
        var result = healthService.GetHealthStatus();

        // Assert
        Assert.Equal("ok", result);
    }

    [Fact]
    public void GetHealthStatus_ReturnsNonEmptyString()
    {
        // Arrange
        var healthService = new HealthService();

        // Act
        var result = healthService.GetHealthStatus();

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }
}
