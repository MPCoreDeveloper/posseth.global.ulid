// Michel Posseth - 2024-05-20 YYYY-MM-DD
// Encode and decode Base32 (Crockford) for ULIDs
// MOD: Michel Posseth - 2026-08-25 YYYY-MM-DD - corrected to be fully compliant with the ULID specification (v2.0)
using System;
using System.Text;

namespace Posseth.Global.UlidFactory.Encoding
{
    public static class Base32
    {
        private const string Base32Chars = "0123456789ABCDEFGHJKMNPQRSTVWXYZ";

        public static string Encode(byte[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (data.Length == 0)
            {
                return string.Empty;
            }

            // A ULID is a 128-bit value encoded in 26 Crockford Base32 characters.
            // Because 128 = 25 * 5 + 3, the most significant character only carries
            // 3 bits, i.e. the value is left-padded with 2 leading zero bits.
            var result = new StringBuilder(data.Length * 8 / 5 + 1);
            int hi = 0, bitsRemaining = 2, index = 0;

            while (index < data.Length)
            {
                if (bitsRemaining > 0)
                {
                    hi = hi << 8 | data[index++];
                    bitsRemaining += 8;
                }
                else
                {
                    hi = data[index++];
                    bitsRemaining = 8;
                }

                while (bitsRemaining >= 5)
                {
                    result.Append(Base32Chars[hi >> (bitsRemaining - 5) & 0x1F]);
                    bitsRemaining -= 5;
                }
            }

            if (bitsRemaining > 0)
            {
                result.Append(Base32Chars[hi << (5 - bitsRemaining) & 0x1F]);
            }

            return result.ToString();
        }

        public static byte[] Decode(string input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (input.Length == 0)
            {
                return Array.Empty<byte>();
            }

            input = input.ToUpperInvariant();

            // The first character only carries 3 significant bits (2 leading zero bits),
            // because a ULID is a 128-bit value encoded in 26 Base32 characters.
            var output = new byte[(input.Length * 5 - 2) / 8];
            int bits = 0;
            int bitsRemaining = 0;
            int outputIndex = 0;

            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];

                if (c < '0' || c > 'Z' || c == 'I' || c == 'L' || c == 'O')
                {
                    throw new ArgumentException("Invalid character in the input string.", nameof(input));
                }

                int value = Base32Chars.IndexOf(c);
                if (value < 0)
                {
                    throw new ArgumentException("Invalid character in the input string.", nameof(input));
                }

                if (i == 0)
                {
                    // The first Base32 character holds the top 3 bits of the 128-bit value,
                    // so any value above 7 would exceed the valid ULID range (2^128 - 1).
                    if (value > 7)
                    {
                        throw new ArgumentException("Invalid ULID: the encoded value exceeds the 128-bit range.", nameof(input));
                    }

                    bits = value;
                    bitsRemaining = 3;
                }
                else
                {
                    bits = bits << 5 | value;
                    bitsRemaining += 5;
                }

                if (bitsRemaining >= 8)
                {
                    output[outputIndex++] = (byte)(bits >> (bitsRemaining - 8));
                    bitsRemaining -= 8;
                }
            }

            return output;
        }
    }
}
