using Posseth.UlidFactory;
using Xunit;

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
    public void GetTimestampFromUlid_ShouldReturnCorrectTimestamp()
    {
        // Arrange
        var ulid = Ulid.Parse("066C4MFM00EHYBTA0C9CEEBJMW");

        // Act
        var timestamp = Ulid.GetTimestampFromUlid(ulid);

        // Assert
        Assert.Equal(new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), timestamp);
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
}


