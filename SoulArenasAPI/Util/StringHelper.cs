namespace SoulArenasAPI.Util;

using System.Security.Cryptography;

public class StringHelper
{
    public static string GenerateRandomStringWithSize(int size)
    {
        var randomBytes = new byte[size];
        RandomNumberGenerator.Fill(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    public static string GetDeviceInfoFromUserAgent(string userAgent)
    {
        if (userAgent.Contains("Windows"))
        {
            return "Windows PC";
        }
        else if (userAgent.Contains("Macintosh"))
        {
            return "Mac PC";
        }
        else if (userAgent.Contains("iPhone") || userAgent.Contains("iPad"))
        {
            return "iOS Device";
        }
        else if (userAgent.Contains("Android"))
        {
            return "Android Device";
        }
        else
        {
            return "Unknown Device";
        }
    }
}