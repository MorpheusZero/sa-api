using SoulArenasAPI.Util;

namespace SoulArenas.Tests;

public class UsernameGeneratorTests
{
    [Fact]
    public void GenerateUsername_ReturnsNonEmptyString()
    {
        // Act
        var username = UsernameGenerator.GenerateUsername();

        // Assert
        Assert.NotNull(username);
        Assert.NotEmpty(username);
    }

    [Fact]
    public void GenerateUsername_MatchesExpectedFormat()
    {
        // Act
        var username = UsernameGenerator.GenerateUsername();

        // Assert
        // Format should be: AdjectiveNoun#123
        Assert.Matches(@"^[A-Za-z]+[A-Za-z]+#\d{3}$", username);
        Assert.Contains("#", username);
    }

    [Fact]
    public void GenerateUsername_ContainsNumberSuffix()
    {
        // Act
        var username = UsernameGenerator.GenerateUsername();

        // Assert
        var parts = username.Split('#');
        Assert.Equal(2, parts.Length);
        Assert.True(int.TryParse(parts[1], out var number));
        Assert.InRange(number, 100, 999);
    }

    [Fact]
    public void GenerateUsername_GeneratesMultipleUniqueUsernames()
    {
        // Act
        var usernames = new HashSet<string>();
        for (int i = 0; i < 100; i++)
        {
            usernames.Add(UsernameGenerator.GenerateUsername());
        }

        // Assert
        // With randomization, we should get mostly unique usernames
        Assert.True(usernames.Count > 89); // At least 90% unique
    }

    [Fact]
    public void GenerateUsername_HasValidLength()
    {
        // Act
        var username = UsernameGenerator.GenerateUsername();

        // Assert
        // Username should be reasonably sized (e.g., between 8 and 30 characters)
        Assert.InRange(username.Length, 8, 30);
    }
}
