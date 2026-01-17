using NSubstitute;
using SoulArenasAPI.Database;
using SoulArenasAPI.Database.Entities;
using SoulArenasAPI.Models.DTOs;
using SoulArenasAPI.Services;
using Microsoft.EntityFrameworkCore;

namespace SoulArenas.Tests;

public class UserServiceTests
{
    [Fact]
    public async Task CreateUser_WithValidData_CreatesUserSuccessfully()
    {
        // Arrange
        var mockDbContext = Substitute.For<ApplicationDbContext>(
            new DbContextOptionsBuilder<ApplicationDbContext>().Options);
        var mockDbSet = Substitute.For<DbSet<UserEntity>>();

        mockDbContext.Users.Returns(mockDbSet);

        var userService = new UserService(mockDbContext);
        var userCreateRequest = new UserCreateRequestDTO
        {
            Email = "test@example.com",
            Password = "SecurePassword123"
        };

        // Act
        var result = await userService.CreateUser(userCreateRequest);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test@example.com", result.Email);
        Assert.NotEmpty(result.Username);
        Assert.NotEmpty(result.PasswordHash);
        Assert.True(result.IsActive);
        Assert.False(result.IsDeleted);
        Assert.True(result.CreatedAt <= DateTime.UtcNow);
        Assert.True(result.LastModified <= DateTime.UtcNow);

        mockDbSet.Received(1).Add(Arg.Any<UserEntity>());
        await mockDbContext.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateUser_GeneratesUniqueUsername()
    {
        // Arrange
        var mockDbContext = Substitute.For<ApplicationDbContext>(
            new DbContextOptionsBuilder<ApplicationDbContext>().Options);
        var mockDbSet = Substitute.For<DbSet<UserEntity>>();

        mockDbContext.Users.Returns(mockDbSet);

        var userService = new UserService(mockDbContext);
        var userCreateRequest = new UserCreateRequestDTO
        {
            Email = "test@example.com",
            Password = "SecurePassword123"
        };

        // Act
        var result = await userService.CreateUser(userCreateRequest);

        // Assert
        Assert.Matches(@"^[A-Za-z]+[A-Za-z]+#\d{3}$", result.Username);
    }

    [Fact]
    public async Task CreateUser_HashesPassword()
    {
        // Arrange
        var mockDbContext = Substitute.For<ApplicationDbContext>(
            new DbContextOptionsBuilder<ApplicationDbContext>().Options);
        var mockDbSet = Substitute.For<DbSet<UserEntity>>();

        mockDbContext.Users.Returns(mockDbSet);

        var userService = new UserService(mockDbContext);
        var userCreateRequest = new UserCreateRequestDTO
        {
            Email = "test@example.com",
            Password = "SecurePassword123"
        };

        // Act
        var result = await userService.CreateUser(userCreateRequest);

        // Assert
        Assert.NotEqual("SecurePassword123", result.PasswordHash);
        Assert.Contains(":", result.PasswordHash); // Hash format is salt:hash
    }
}
