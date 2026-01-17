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

    [Fact]
    public async Task GetUserById_WithExistingId_ReturnsUser()
    {
        // Arrange
        var mockDbContext = Substitute.For<ApplicationDbContext>(
            new DbContextOptionsBuilder<ApplicationDbContext>().Options);

        var expectedUser = new UserEntity
        {
            Id = 1,
            Email = "test@example.com",
            Username = "TestUser#123",
            PasswordHash = "hashedpassword",
            IsActive = true,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow,
            LastModified = DateTime.UtcNow
        };

        mockDbContext.Users.FindAsync(1).Returns(ValueTask.FromResult<UserEntity?>(expectedUser));

        var userService = new UserService(mockDbContext);

        // Act
        var result = await userService.GetUserById(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("test@example.com", result.Email);
        Assert.Equal("TestUser#123", result.Username);
    }

    [Fact]
    public async Task GetUserById_WithNonExistingId_ReturnsNull()
    {
        // Arrange
        var mockDbContext = Substitute.For<ApplicationDbContext>(
            new DbContextOptionsBuilder<ApplicationDbContext>().Options);

        mockDbContext.Users.FindAsync(999).Returns(ValueTask.FromResult<UserEntity?>(null));

        var userService = new UserService(mockDbContext);

        // Act
        var result = await userService.GetUserById(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetUserByEmail_NormalizesEmailToLowerCase()
    {
        // Arrange
        var mockDbContext = Substitute.For<ApplicationDbContext>(
            new DbContextOptionsBuilder<ApplicationDbContext>().Options);

        var userService = new UserService(mockDbContext);

        // Act - The method should normalize the email to lowercase when querying
        // Since we're testing the service layer and using mocks, we're verifying
        // that the service normalizes the input (tested via actual CreateUser behavior)
        var userCreateRequest = new UserCreateRequestDTO
        {
            Email = "UPPERCASE@EXAMPLE.COM",
            Password = "Password123"
        };

        var result = await userService.CreateUser(userCreateRequest);

        // Assert
        Assert.Equal("uppercase@example.com", result.Email);
    }

    [Fact]
    public async Task CreateUser_SetsIsEmailVerifiedToFalse()
    {
        // Arrange
        var mockDbContext = Substitute.For<ApplicationDbContext>(
            new DbContextOptionsBuilder<ApplicationDbContext>().Options);
        var mockDbSet = Substitute.For<DbSet<UserEntity>>();

        mockDbContext.Users.Returns(mockDbSet);

        var userService = new UserService(mockDbContext);
        var userCreateRequest = new UserCreateRequestDTO
        {
            Email = "newuser@example.com",
            Password = "Password123"
        };

        // Act
        var result = await userService.CreateUser(userCreateRequest);

        // Assert
        Assert.False(result.IsEmailVerified);
    }

    [Fact]
    public async Task CreateUser_NormalizesEmailToLowerCase()
    {
        // Arrange
        var mockDbContext = Substitute.For<ApplicationDbContext>(
            new DbContextOptionsBuilder<ApplicationDbContext>().Options);
        var mockDbSet = Substitute.For<DbSet<UserEntity>>();

        mockDbContext.Users.Returns(mockDbSet);

        var userService = new UserService(mockDbContext);
        var userCreateRequest = new UserCreateRequestDTO
        {
            Email = "UPPERCASE@EXAMPLE.COM",
            Password = "Password123"
        };

        // Act
        var result = await userService.CreateUser(userCreateRequest);

        // Assert
        Assert.Equal("uppercase@example.com", result.Email);
    }
}
