using SoulArenasAPI.Util;

namespace SoulArenas.Tests;

public class CryptoHelperTests
{
    [Fact]
    public async Task HashPasswordAsync_ReturnsNonEmptyHash()
    {
        // Arrange
        var password = "TestPassword123";

        // Act
        var hash = await CryptoHelper.HashPasswordAsync(password);

        // Assert
        Assert.NotNull(hash);
        Assert.NotEmpty(hash);
    }

    [Fact]
    public async Task HashPasswordAsync_ReturnsHashWithCorrectFormat()
    {
        // Arrange
        var password = "TestPassword123";

        // Act
        var hash = await CryptoHelper.HashPasswordAsync(password);

        // Assert
        Assert.Contains(":", hash);
        var parts = hash.Split(':');
        Assert.Equal(2, parts.Length);
        Assert.NotEmpty(parts[0]); // Salt
        Assert.NotEmpty(parts[1]); // Hash
    }

    [Fact]
    public async Task HashPasswordAsync_GeneratesDifferentHashesForSamePassword()
    {
        // Arrange
        var password = "TestPassword123";

        // Act
        var hash1 = await CryptoHelper.HashPasswordAsync(password);
        var hash2 = await CryptoHelper.HashPasswordAsync(password);

        // Assert
        Assert.NotEqual(hash1, hash2); // Different salts should produce different hashes
    }

    [Fact]
    public async Task VerifyPasswordAsync_WithCorrectPassword_ReturnsTrue()
    {
        // Arrange
        var password = "TestPassword123";
        var hash = await CryptoHelper.HashPasswordAsync(password);

        // Act
        var result = await CryptoHelper.VerifyPasswordAsync(password, hash);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task VerifyPasswordAsync_WithIncorrectPassword_ReturnsFalse()
    {
        // Arrange
        var password = "TestPassword123";
        var wrongPassword = "WrongPassword456";
        var hash = await CryptoHelper.HashPasswordAsync(password);

        // Act
        var result = await CryptoHelper.VerifyPasswordAsync(wrongPassword, hash);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task VerifyPasswordAsync_WithMalformedHash_ReturnsFalse()
    {
        // Arrange
        var password = "TestPassword123";
        var malformedHash = "invalid_hash_format";

        // Act
        var result = await CryptoHelper.VerifyPasswordAsync(password, malformedHash);

        // Assert
        Assert.False(result);
    }
}
