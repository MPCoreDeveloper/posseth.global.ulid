// Start: Michel Posseth - 2024-05-20 YYYY-MM-DD
// MOD : Michel Posseth - 2024-05-26 YYYY-MM-DD
// made type methods static / more robust 
// Handy ULID factory
using System.Security.Cryptography;
using Posseth.UlidFactory.Encoding;

namespace Posseth.UlidFactory;

public record Ulid(string Value)
{
    public Ulid() : this(string.Empty) { }

    public static Ulid NewUlid()
    {
        return NewUlid(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
    }

    public static Ulid NewUlid(DateTimeOffset timestamp)
    {
        return NewUlid(timestamp.ToUnixTimeMilliseconds());
    }

    public static Ulid NewUlid(DateTime timestamp)
    {
        return NewUlid(new DateTimeOffset(timestamp).ToUnixTimeMilliseconds());
    }

    public static Ulid NewUlid(long timestamp)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(timestamp, nameof(timestamp));

        Span<byte> ulidBytes = stackalloc byte[16];

        for (int i = 5; i >= 0; i--)
        {
            ulidBytes[i] = (byte)(timestamp & 0xFF);
            timestamp >>= 8;
        }

        RandomNumberGenerator.Fill(ulidBytes[6..]);

        var ulid = Base32.Encode(ulidBytes.ToArray());
        return new Ulid(ulid);
    }

    public static DateTime GetTimestampFromUlid(Ulid ulid)
    {
        ArgumentException.ThrowIfNullOrEmpty(ulid.Value, nameof(ulid));
        
        if (ulid.Value.Length != 26)
        {
            throw new ArgumentException("Invalid ULID length. ULID should be 26 characters long.", nameof(ulid));
        }

        ReadOnlySpan<byte> bytes = Base32.Decode(ulid.Value[..10]);

        var timestamp = ExtractTimestamp(bytes);

        return DateTime.UnixEpoch.AddMilliseconds(timestamp);
    }

    public DateTime ToDateTime()
    {
        return GetTimestampFromUlid(this);
    }

    public long ToEpoch()
    {
        if (string.IsNullOrEmpty(Value) || Value.Length != 26)
        {
            return 0;
        }

        ReadOnlySpan<byte> bytes = Base32.Decode(Value[..10]);

        return ExtractTimestamp(bytes);
    }
    
    public long ToUnixTime() => ToEpoch();

    private static long ExtractTimestamp(ReadOnlySpan<byte> bytes)
    {
        long timestamp = 0;
        for (int i = 0; i < 6; i++)
        {
            timestamp = timestamp << 8 | bytes[i];
        }
        return timestamp;
    }

    public override string ToString() => Value;

    public bool HasValue() => !string.IsNullOrEmpty(Value);

    public static bool TryParse(string? ulidString, out Ulid? ulid)
    {
        ulid = null;

        if (string.IsNullOrEmpty(ulidString) || ulidString.Length != 26)
        {
            return false;
        }

        try
        {
            ReadOnlySpan<byte> bytes = Base32.Decode(ulidString);
            var timestamp = ExtractTimestamp(bytes);
            if (timestamp < 0)
            {
                return false;
            }
            ulid = new Ulid(ulidString);
            return true;
        }
        catch (Exception ex) when (ex is FormatException or ArgumentException)
        {
            return false;
        }
    }
    
    public static Ulid Parse(string? ulidString)
    {
        ArgumentException.ThrowIfNullOrEmpty(ulidString, nameof(ulidString));
        
        if (ulidString.Length != 26)
        {
            throw new ArgumentException("Invalid ULID string. ULID should be 26 characters long.", nameof(ulidString));
        }

        try
        {
            ReadOnlySpan<byte> bytes = Base32.Decode(ulidString);
            var timestamp = ExtractTimestamp(bytes);
            if (timestamp < 0)
            {
                throw new ArgumentException("Invalid ULID string.", nameof(ulidString));
            }
            return new Ulid(ulidString);
        }
        catch (Exception ex) when (ex is FormatException or ArgumentException)
        {
            throw new ArgumentException("Invalid ULID string format.", nameof(ulidString), ex);
        }
    }
}
