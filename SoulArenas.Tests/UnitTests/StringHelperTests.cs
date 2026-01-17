using SoulArenasAPI.Util;

namespace SoulArenas.Tests.UnitTests;

public class StringHelperTests
{
    [Fact]
    public void GenerateRandomStringWithSize_ReturnsNonEmptyString()
    {
        // Act
        var result = StringHelper.GenerateRandomStringWithSize(32);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public void GenerateRandomStringWithSize_ReturnsBase64String()
    {
        // Act
        var result = StringHelper.GenerateRandomStringWithSize(64);

        // Assert
        Assert.NotNull(result);

        // Base64 strings contain only valid Base64 characters
        Assert.Matches(@"^[A-Za-z0-9+/=]+$", result);
    }

    [Fact]
    public void GenerateRandomStringWithSize_GeneratesUniqueStrings()
    {
        // Act
        var results = new HashSet<string>();
        for (int i = 0; i < 100; i++)
        {
            results.Add(StringHelper.GenerateRandomStringWithSize(128));
        }

        // Assert
        // All 100 generated strings should be unique
        Assert.Equal(100, results.Count);
    }

    [Fact]
    public void GenerateRandomStringWithSize_WithSize16_GeneratesApproximatelyCorrectLength()
    {
        // Act
        var result = StringHelper.GenerateRandomStringWithSize(16);

        // Assert
        // Base64 encoding of 16 bytes = ceil(16 * 4/3) = 24 characters
        Assert.True(result.Length >= 20 && result.Length <= 28);
    }

    [Fact]
    public void GenerateRandomStringWithSize_WithSize128_GeneratesApproximatelyCorrectLength()
    {
        // Act
        var result = StringHelper.GenerateRandomStringWithSize(128);

        // Assert
        // Base64 encoding of 128 bytes = ceil(128 * 4/3) ≈ 172 characters
        Assert.True(result.Length >= 160 && result.Length <= 180);
    }

    [Fact]
    public void GetDeviceInfoFromUserAgent_WithWindows_ReturnsWindowsPC()
    {
        // Arrange
        var userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36";

        // Act
        var result = StringHelper.GetDeviceInfoFromUserAgent(userAgent);

        // Assert
        Assert.Equal("Windows PC", result);
    }

    [Fact]
    public void GetDeviceInfoFromUserAgent_WithMac_ReturnsMacPC()
    {
        // Arrange
        var userAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36";

        // Act
        var result = StringHelper.GetDeviceInfoFromUserAgent(userAgent);

        // Assert
        Assert.Equal("Mac PC", result);
    }

    [Fact]
    public void GetDeviceInfoFromUserAgent_WithIPhone_ReturnsiOSDevice()
    {
        // Arrange
        var userAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 17_0 like Mac OS X) AppleWebKit/605.1.15";

        // Act
        var result = StringHelper.GetDeviceInfoFromUserAgent(userAgent);

        // Assert
        Assert.Equal("iOS Device", result);
    }

    [Fact]
    public void GetDeviceInfoFromUserAgent_WithIPad_ReturnsiOSDevice()
    {
        // Arrange
        var userAgent = "Mozilla/5.0 (iPad; CPU OS 16_6 like Mac OS X) AppleWebKit/605.1.15";

        // Act
        var result = StringHelper.GetDeviceInfoFromUserAgent(userAgent);

        // Assert
        Assert.Equal("iOS Device", result);
    }

    [Fact]
    public void GetDeviceInfoFromUserAgent_WithAndroid_ReturnsAndroidDevice()
    {
        // Arrange
        var userAgent = "Mozilla/5.0 (Linux; Android 13; Pixel 7) AppleWebKit/537.36";

        // Act
        var result = StringHelper.GetDeviceInfoFromUserAgent(userAgent);

        // Assert
        Assert.Equal("Android Device", result);
    }

    [Fact]
    public void GetDeviceInfoFromUserAgent_WithUnknown_ReturnsUnknownDevice()
    {
        // Arrange
        var userAgent = "SomeCustomBrowser/1.0";

        // Act
        var result = StringHelper.GetDeviceInfoFromUserAgent(userAgent);

        // Assert
        Assert.Equal("Unknown Device", result);
    }

    [Fact]
    public void GetDeviceInfoFromUserAgent_WithEmptyString_ReturnsUnknownDevice()
    {
        // Arrange
        var userAgent = "";

        // Act
        var result = StringHelper.GetDeviceInfoFromUserAgent(userAgent);

        // Assert
        Assert.Equal("Unknown Device", result);
    }

    [Fact]
    public void GetDeviceInfoFromUserAgent_PrioritizesWindowsOverOtherMatches()
    {
        // Arrange
        var userAgent = "Windows and Android somehow in same string";

        // Act
        var result = StringHelper.GetDeviceInfoFromUserAgent(userAgent);

        // Assert
        Assert.Equal("Windows PC", result);
    }

    [Fact]
    public void GetDeviceInfoFromUserAgent_PrioritizesMacOverMobileDevices()
    {
        // Arrange
        var userAgent = "Macintosh and iPhone in same string";

        // Act
        var result = StringHelper.GetDeviceInfoFromUserAgent(userAgent);

        // Assert
        Assert.Equal("Mac PC", result);
    }
}
