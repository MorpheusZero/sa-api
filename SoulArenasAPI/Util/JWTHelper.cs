using System.Text;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace SoulArenasAPI.Util;

public static class JWTHelper
{
    public const string Issuer = "SoulArenasAPI";
    public const string Audience = "SoulArenasAPIClientUser";

    public static string GenerateAccessTokenForUser(string secretKey, string userId)
    {
        var handler = new JsonWebTokenHandler();
        var key = Encoding.UTF8.GetBytes(secretKey);

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = Issuer,
            Audience = Audience,
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
            }),
            Expires = DateTime.UtcNow.AddMinutes(15),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        return handler.CreateToken(descriptor);
    }
}
