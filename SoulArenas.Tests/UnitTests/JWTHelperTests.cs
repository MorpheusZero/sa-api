using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using SoulArenasAPI.Util;

namespace SoulArenas.Tests.UnitTests;

public class JWTHelperTests
{
    private const string TestSecretKey = "ThisIsATestSecretKeyForJWTTesting12345678901234567890";

    [Fact]
    public void GenerateAccessTokenForUser_ReturnsNonEmptyToken()
    {
        // Arrange
        var userId = "123";

        // Act
        var token = JWTHelper.GenerateAccessTokenForUser(TestSecretKey, userId);

        // Assert
        Assert.NotNull(token);
        Assert.NotEmpty(token);
    }

    [Fact]
    public void GenerateAccessTokenForUser_TokenIsValidJWT()
    {
        // Arrange
        var userId = "456";

        // Act
        var token = JWTHelper.GenerateAccessTokenForUser(TestSecretKey, userId);

        // Assert
        var handler = new JwtSecurityTokenHandler();
        Assert.True(handler.CanReadToken(token));
    }

    [Fact]
    public void GenerateAccessTokenForUser_TokenContainsCorrectIssuer()
    {
        // Arrange
        var userId = "789";

        // Act
        var token = JWTHelper.GenerateAccessTokenForUser(TestSecretKey, userId);

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        Assert.Equal(JWTHelper.Issuer, jwtToken.Issuer);
        Assert.Equal("SoulArenasAPI", jwtToken.Issuer);
    }

    [Fact]
    public void GenerateAccessTokenForUser_TokenContainsCorrectAudience()
    {
        // Arrange
        var userId = "101112";

        // Act
        var token = JWTHelper.GenerateAccessTokenForUser(TestSecretKey, userId);

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        Assert.Contains(JWTHelper.Audience, jwtToken.Audiences);
        Assert.Contains("SoulArenasAPIClientUser", jwtToken.Audiences);
    }

    [Fact]
    public void GenerateAccessTokenForUser_TokenContainsUserId()
    {
        // Arrange
        var userId = "42";

        // Act
        var token = JWTHelper.GenerateAccessTokenForUser(TestSecretKey, userId);

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        var subClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub);

        Assert.NotNull(subClaim);
        Assert.Equal(userId, subClaim.Value);
    }

    [Fact]
    public void GenerateAccessTokenForUser_TokenExpiresIn15Minutes()
    {
        // Arrange
        var userId = "999";
        var beforeGeneration = DateTime.UtcNow;

        // Act
        var token = JWTHelper.GenerateAccessTokenForUser(TestSecretKey, userId);
        var afterGeneration = DateTime.UtcNow;

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        var expectedExpiry = beforeGeneration.AddMinutes(15);
        var actualExpiry = jwtToken.ValidTo;

        // Allow 1 minute tolerance for test execution time
        Assert.True(Math.Abs((actualExpiry - expectedExpiry).TotalMinutes) < 1);

        // Verify it's approximately 15 minutes from now
        var minutesUntilExpiry = (actualExpiry - afterGeneration).TotalMinutes;
        Assert.InRange(minutesUntilExpiry, 14.9, 15.1);
    }

    [Fact]
    public void GenerateAccessTokenForUser_DifferentUsersGetDifferentTokens()
    {
        // Arrange
        var userId1 = "100";
        var userId2 = "200";

        // Act
        var token1 = JWTHelper.GenerateAccessTokenForUser(TestSecretKey, userId1);
        var token2 = JWTHelper.GenerateAccessTokenForUser(TestSecretKey, userId2);

        // Assert
        Assert.NotEqual(token1, token2);

        var handler = new JwtSecurityTokenHandler();
        var jwtToken1 = handler.ReadJwtToken(token1);
        var jwtToken2 = handler.ReadJwtToken(token2);

        var sub1 = jwtToken1.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value;
        var sub2 = jwtToken2.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value;

        Assert.Equal("100", sub1);
        Assert.Equal("200", sub2);
    }

    [Fact]
    public void GenerateAccessTokenForUser_TokenCanBeValidated()
    {
        // Arrange
        var userId = "777";
        var token = JWTHelper.GenerateAccessTokenForUser(TestSecretKey, userId);

        // Act - Read the token to verify its structure and claims
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);

        // Assert
        Assert.NotNull(jwtToken);
        Assert.Equal(JWTHelper.Issuer, jwtToken.Issuer);
        Assert.Contains(JWTHelper.Audience, jwtToken.Audiences);

        var subClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub");
        Assert.NotNull(subClaim);
        Assert.Equal(userId, subClaim.Value);

        // Verify token expiration is approximately 15 minutes from now
        var expirationTime = jwtToken.ValidTo;
        var minutesUntilExpiry = (expirationTime - DateTime.UtcNow).TotalMinutes;
        Assert.InRange(minutesUntilExpiry, 14.5, 15.5);
    }

    [Fact]
    public void GenerateAccessTokenForUser_TokenWithInvalidSecretKeyFailsValidation()
    {
        // Arrange
        var userId = "888";
        var token = JWTHelper.GenerateAccessTokenForUser(TestSecretKey, userId);
        var wrongSecretKey = "WrongSecretKeyThatDoesNotMatchTheOriginal12345678901234567890";

        // Act & Assert
        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(wrongSecretKey)),
            ValidateIssuer = true,
            ValidIssuer = JWTHelper.Issuer,
            ValidateAudience = true,
            ValidAudience = JWTHelper.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        Assert.Throws<SecurityTokenSignatureKeyNotFoundException>(() =>
            tokenHandler.ValidateToken(token, validationParameters, out _)
        );
    }
}
