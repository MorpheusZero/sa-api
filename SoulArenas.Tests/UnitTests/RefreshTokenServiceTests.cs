using NSubstitute;
using SoulArenasAPI.Database;
using SoulArenasAPI.Database.Entities;
using SoulArenasAPI.Services;
using Microsoft.EntityFrameworkCore;

namespace SoulArenas.Tests.UnitTests;

public class RefreshTokenServiceTests
{
    [Fact]
    public async Task CreateRefreshToken_WithValidData_CreatesTokenSuccessfully()
    {
        // Arrange
        var mockDbContext = Substitute.For<ApplicationDbContext>(
            new DbContextOptionsBuilder<ApplicationDbContext>().Options);
        var mockDbSet = Substitute.For<DbSet<RefreshTokenEntity>>();

        mockDbContext.RefreshTokens.Returns(mockDbSet);

        var refreshTokenService = new RefreshTokenService(mockDbContext);

        // Act
        var result = await refreshTokenService.CreateRefreshToken(
            userId: 1,
            refreshTokenHash: "hashedtoken123",
            deviceInfo: "Windows PC",
            ipAddress: "192.168.1.1",
            userAgent: "Mozilla/5.0"
        );

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.UserId);
        Assert.Equal("hashedtoken123", result.TokenHash);
        Assert.Equal("Windows PC", result.DeviceInfo);
        Assert.Equal("192.168.1.1", result.IPAddress);
        Assert.Equal("Mozilla/5.0", result.UserAgent);
        Assert.Null(result.RevokedAt);
        Assert.True(result.CreatedAt <= DateTime.UtcNow);

        mockDbSet.Received(1).Add(Arg.Any<RefreshTokenEntity>());
        await mockDbContext.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateRefreshToken_SetsCorrectExpirationDate()
    {
        // Arrange
        var mockDbContext = Substitute.For<ApplicationDbContext>(
            new DbContextOptionsBuilder<ApplicationDbContext>().Options);
        var mockDbSet = Substitute.For<DbSet<RefreshTokenEntity>>();

        mockDbContext.RefreshTokens.Returns(mockDbSet);

        var refreshTokenService = new RefreshTokenService(mockDbContext);
        var beforeCreation = DateTime.UtcNow.AddDays(7);

        // Act
        var result = await refreshTokenService.CreateRefreshToken(
            userId: 1,
            refreshTokenHash: "hashedtoken123",
            deviceInfo: "iOS Device",
            ipAddress: "10.0.0.1",
            userAgent: "Safari/17.0"
        );

        var afterCreation = DateTime.UtcNow.AddDays(7);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.ExpiresAt >= beforeCreation);
        Assert.True(result.ExpiresAt <= afterCreation);

        // Verify it's approximately 7 days from now (within 1 minute tolerance)
        var expectedExpiration = DateTime.UtcNow.AddDays(7);
        Assert.True(Math.Abs((result.ExpiresAt - expectedExpiration).TotalMinutes) < 1);
    }

    [Fact]
    public async Task CreateRefreshToken_SetsNullRevokedAt()
    {
        // Arrange
        var mockDbContext = Substitute.For<ApplicationDbContext>(
            new DbContextOptionsBuilder<ApplicationDbContext>().Options);
        var mockDbSet = Substitute.For<DbSet<RefreshTokenEntity>>();

        mockDbContext.RefreshTokens.Returns(mockDbSet);

        var refreshTokenService = new RefreshTokenService(mockDbContext);

        // Act
        var result = await refreshTokenService.CreateRefreshToken(
            userId: 5,
            refreshTokenHash: "anotherhash",
            deviceInfo: "Android Device",
            ipAddress: "172.16.0.1",
            userAgent: "Chrome/120.0"
        );

        // Assert
        Assert.Null(result.RevokedAt);
    }

    [Fact]
    public async Task GetRefreshTokenById_WithExistingId_ReturnsToken()
    {
        // Arrange
        var mockDbContext = Substitute.For<ApplicationDbContext>(
            new DbContextOptionsBuilder<ApplicationDbContext>().Options);

        var expectedToken = new RefreshTokenEntity
        {
            Id = 42,
            UserId = 1,
            TokenHash = "hashedtoken",
            DeviceInfo = "Mac PC",
            IPAddress = "192.168.1.100",
            UserAgent = "Safari/16.0",
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            RevokedAt = null
        };

        mockDbContext.RefreshTokens.FindAsync(42)
            .Returns(ValueTask.FromResult<RefreshTokenEntity?>(expectedToken));

        var refreshTokenService = new RefreshTokenService(mockDbContext);

        // Act
        var result = await refreshTokenService.GetRefreshTokenById(42);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(42, result.Id);
        Assert.Equal("hashedtoken", result.TokenHash);
        Assert.Equal("Mac PC", result.DeviceInfo);
    }

    [Fact]
    public async Task GetRefreshTokenById_WithNonExistingId_ReturnsNull()
    {
        // Arrange
        var mockDbContext = Substitute.For<ApplicationDbContext>(
            new DbContextOptionsBuilder<ApplicationDbContext>().Options);

        mockDbContext.RefreshTokens.FindAsync(999)
            .Returns(ValueTask.FromResult<RefreshTokenEntity?>(null));

        var refreshTokenService = new RefreshTokenService(mockDbContext);

        // Act
        var result = await refreshTokenService.GetRefreshTokenById(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateRefreshToken_UpdatesEntitySuccessfully()
    {
        // Arrange
        var mockDbContext = Substitute.For<ApplicationDbContext>(
            new DbContextOptionsBuilder<ApplicationDbContext>().Options);
        var mockDbSet = Substitute.For<DbSet<RefreshTokenEntity>>();

        mockDbContext.RefreshTokens.Returns(mockDbSet);

        var refreshTokenService = new RefreshTokenService(mockDbContext);

        var tokenToUpdate = new RefreshTokenEntity
        {
            Id = 1,
            UserId = 1,
            TokenHash = "hashedtoken",
            DeviceInfo = "Windows PC",
            IPAddress = "192.168.1.1",
            UserAgent = "Mozilla/5.0",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            ExpiresAt = DateTime.UtcNow.AddDays(6),
            RevokedAt = null,
            LastUsedAt = null
        };

        tokenToUpdate.RevokedAt = DateTime.UtcNow;
        tokenToUpdate.LastUsedAt = DateTime.UtcNow;

        // Act
        var result = await refreshTokenService.UpdateRefreshToken(tokenToUpdate);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.RevokedAt);
        Assert.NotNull(result.LastUsedAt);
        mockDbSet.Received(1).Update(tokenToUpdate);
        await mockDbContext.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateRefreshToken_CallsSaveChanges()
    {
        // Arrange
        var mockDbContext = Substitute.For<ApplicationDbContext>(
            new DbContextOptionsBuilder<ApplicationDbContext>().Options);
        var mockDbSet = Substitute.For<DbSet<RefreshTokenEntity>>();

        mockDbContext.RefreshTokens.Returns(mockDbSet);

        var refreshTokenService = new RefreshTokenService(mockDbContext);

        var tokenToUpdate = new RefreshTokenEntity
        {
            Id = 10,
            UserId = 5,
            TokenHash = "sometoken",
            DeviceInfo = "Linux PC",
            IPAddress = "10.0.0.50",
            UserAgent = "Firefox/121.0",
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        // Act
        await refreshTokenService.UpdateRefreshToken(tokenToUpdate);

        // Assert
        await mockDbContext.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
