// Michel Posseth - 2024-05-20 YYYY-MM-DD
// Handy ULID factory
using System.Security.Cryptography;
using Posseth.Global.UlidFactory.Encoding;
namespace Posseth.Global.UlidFactory
{
    public static class Ulid
    {
        public static string NewUlid()
        {
            byte[] ulidBytes = new byte[16];

            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            for (int i = 5; i >= 0; i--)
            {
                ulidBytes[i] = (byte)(timestamp & 0xFF);
                timestamp >>= 8;
            }

            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
                rng.GetBytes(ulidBytes, 6, 10);

            return Base32.Encode(ulidBytes);

        }
        public static DateTime GetTimestampFromUlid(string ulid)
        {
            if (ulid.Length != 26)
            {
                throw new ArgumentException("Invalid ULID length. ULID should be 26 characters long.");
            }

            byte[] bytes = Base32.Decode(ulid[..10]);

            long timestamp = ExtractTimestamp(bytes);

            DateTime epoch = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddMilliseconds(timestamp);
        }
        private static long ExtractTimestamp(byte[] bytes)
        {
            long timestamp = 0;
            for (int i = 0; i < 6; i++)
            {
                timestamp = timestamp << 8 | bytes[i];
            }
            return timestamp;
        }
    }
}
