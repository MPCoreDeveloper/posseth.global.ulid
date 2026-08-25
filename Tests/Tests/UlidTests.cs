using Posseth.UlidFactory;

namespace Posseth.Global.UlidFactory.Tests;

public class UlidTests
{
    [Fact]
    public void NewUlid_ShouldReturnValidUlid()
    {
        // Act
        var ulid = Ulid.NewUlid();

        // Assert
        Assert.NotNull(ulid);
        Assert.True(ulid.HasValue());
    }

    [Fact]
    public void NewUlid_ShouldProduce26CrockfordCharacters()
    {
        // Act
        var ulid = Ulid.NewUlid().ToString();

        // Assert
        Assert.Equal(26, ulid.Length);
        Assert.Matches("^[0-9A-HJKMNP-TV-Z]{26}$", ulid);
    }

    [Fact]
    public void NewUlid_WithTimestamp_ShouldReturnValidUlid()
    {
        // Arrange
        var timestamp = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero);

        // Act
        var ulid = Ulid.NewUlid(timestamp);

        // Assert
        Assert.NotNull(ulid);
        Assert.True(ulid.HasValue());
    }

    [Fact]
    public void NewUlid_WithDateTime_ShouldReturnValidUlid()
    {
        // Arrange
        var dateTime = new DateTime(2024, 1, 1, 0, 0, 0);

        // Act
        var ulid = Ulid.NewUlid(dateTime);

        // Assert
        Assert.NotNull(ulid);
        Assert.True(ulid.HasValue());
    }

    [Fact]
    public void NewUlid_WithTimestampLong_ShouldReturnValidUlid()
    {
        // Arrange
        const long timestamp = 1704067200000; // January 1, 2024 00:00:00 UTC

        // Act
        var ulid = Ulid.NewUlid(timestamp);

        // Assert
        Assert.NotNull(ulid);
        Assert.True(ulid.HasValue());
    }

    [Fact]
    public void NewUlid_WithTimestampLong_ShouldProduceStandardCompliantTimestampPrefix()
    {
        // Arrange
        const long timestamp = 1704067200000; // January 1, 2024 00:00:00 UTC

        // Act
        var ulid = Ulid.NewUlid(timestamp);

        // Assert - "01HK153X00" is the standard Base32 encoding of 1704067200000
        Assert.StartsWith("01HK153X00", ulid.ToString());
    }

    [Fact]
    public void NewUlid_WithTimestampExceeding48Bits_ShouldThrow()
    {
        // Arrange
        const long timestamp = 0xFFFFFFFFFFFF + 1; // 2^48

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => Ulid.NewUlid(timestamp));
    }

    [Fact]
    public void GetTimestampFromUlid_ShouldReturnCorrectTimestamp()
    {
        // Arrange
        var ulid = Ulid.Parse("01HK153X00" + new string('0', 16));

        // Act
        var timestamp = Ulid.GetTimestampFromUlid(ulid);

        // Assert
        Assert.Equal(new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), timestamp);
    }

    [Theory]
    [InlineData("01HK153X00" + "0000000000000000", 1704067200000)] // 2024-01-01 00:00:00 UTC
    [InlineData("01AN4Z07BY79KA1307SR9X4MV3", 1465824320894)]   // official ULID spec example
    [InlineData("7ZZZZZZZZZZZZZZZZZZZZZZZZZ", 281474976710655)] // maximum ULID: 2^48 - 1
    public void ToEpoch_ShouldExtractTimestampPerStandard(string ulidString, long expectedEpoch)
    {
        // Act
        var epoch = Ulid.Parse(ulidString).ToEpoch();

        // Assert
        Assert.Equal(expectedEpoch, epoch);
    }

    [Fact]
    public void TryParse_ValidUlidString_ShouldReturnTrueAndValidUlid()
    {
        // Arrange
        const string ulidString = "01F9Z3N2F2VX4XG00000000000";

        // Act
        var result = Ulid.TryParse(ulidString, out var ulid);

        // Assert
        Assert.True(result);
        Assert.NotNull(ulid);
        Assert.True(ulid.HasValue());
    }

    [Fact]
    public void TryParse_InvalidUlidString_ShouldReturnFalseAndNullUlid()
    {
        // Arrange
        const string ulidString = "invalidulid";

        // Act
        var result = Ulid.TryParse(ulidString, out var ulid);

        // Assert
        Assert.False(result);
        Assert.Null(ulid);
    }

    [Fact]
    public void TryParse_UlidExceeding128Bits_ShouldReturnFalse()
    {
        // Arrange - a first character above '7' exceeds the valid 128-bit ULID range
        const string ulidString = "8ZZZZZZZZZZZZZZZZZZZZZZZZZ";

        // Act
        var result = Ulid.TryParse(ulidString, out var ulid);

        // Assert
        Assert.False(result);
        Assert.Null(ulid);
    }

    [Fact]
    public void NewUlid_RoundTrip_ShouldPreserveValue()
    {
        // Arrange
        var ulid = Ulid.NewUlid();

        // Act
        var parsed = Ulid.Parse(ulid.ToString());

        // Assert
        Assert.Equal(ulid, parsed);
    }
}


