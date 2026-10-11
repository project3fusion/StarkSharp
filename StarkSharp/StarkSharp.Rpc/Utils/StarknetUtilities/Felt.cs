using System;
using System.Globalization;
using System.Numerics;
using System.Text;

namespace StarkSharp.Rpc.Utils
{
    /// <summary>
    /// Utility methods for working with Starknet field elements (felt252).
    /// </summary>
    public static class Felt
    {
        /// <summary>
        /// Parses a felt string (either hex "0x..." or decimal) into a <see cref="BigInteger"/>.
        /// </summary>
        public static BigInteger ToBigInteger(string? feltValue)
        {
            if (string.IsNullOrWhiteSpace(feltValue))
                return BigInteger.Zero;

            feltValue = feltValue.Trim();

            if (feltValue.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                string hex = feltValue.Substring(2);
                if (string.IsNullOrEmpty(hex))
                    return BigInteger.Zero;

                return BigInteger.Parse("0" + hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }

            if (BigInteger.TryParse(feltValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result))
            {
                return result;
            }

            return BigInteger.Zero;
        }

        /// <summary>
        /// Converts a <see cref="BigInteger"/> to a normalized 0x-prefixed hex string.
        /// </summary>
        public static string ToHex(BigInteger value)
        {
            if (value < BigInteger.Zero)
                throw new ArgumentOutOfRangeException(nameof(value), "Felt value cannot be negative.");

            if (value == BigInteger.Zero)
                return "0x0";

            string hex = value.ToString("x").TrimStart('0');
            return "0x" + (hex.Length == 0 ? "0" : hex);
        }

        /// <summary>
        /// Decodes a Cairo short string (ASCII packed in a felt) into a readable string.
        /// </summary>
        public static string ToShortString(BigInteger feltValue)
        {
            if (feltValue == BigInteger.Zero)
                return string.Empty;

            byte[] bytes = feltValue.ToByteArray();
            // BigInteger.ToByteArray() is little-endian with optional sign byte
            Array.Reverse(bytes);

            // Skip leading zero bytes
            int start = 0;
            while (start < bytes.Length && bytes[start] == 0)
                start++;

            if (start >= bytes.Length)
                return string.Empty;

            return Encoding.UTF8.GetString(bytes, start, bytes.Length - start).Trim('\0');
        }

        /// <summary>
        /// Decodes a Cairo short string from a felt hex/dec string into a readable string.
        /// </summary>
        public static string ToShortString(string? feltValue)
        {
            var bigInt = ToBigInteger(feltValue);
            return ToShortString(bigInt);
        }

        /// <summary>
        /// Encodes an ASCII short string (up to 31 characters) into a felt BigInteger.
        /// </summary>
        public static BigInteger FromShortString(string text)
        {
            if (string.IsNullOrEmpty(text))
                return BigInteger.Zero;

            byte[] bytes = Encoding.UTF8.GetBytes(text);
            if (bytes.Length > 31)
                throw new ArgumentException("Short string exceeds max felt length of 31 bytes.", nameof(text));

            // Convert big-endian byte array to BigInteger
            byte[] leBytes = new byte[bytes.Length + 1];
            for (int i = 0; i < bytes.Length; i++)
            {
                leBytes[i] = bytes[bytes.Length - 1 - i];
            }
            leBytes[bytes.Length] = 0; // ensure positive

            return new BigInteger(leBytes);
        }

        /// <summary>
        /// Decodes a Cairo string response (supporting both Cairo 0 short strings and Cairo 1 ByteArray).
        /// </summary>
        public static string DecodeCairoString(string[]? felts)
        {
            if (felts == null || felts.Length == 0)
                return string.Empty;

            // Cairo 0: single felt short string
            if (felts.Length == 1)
            {
                return ToShortString(felts[0]);
            }

            // Cairo 1 ByteArray: [num_full_words, full_words..., pending_word, pending_word_len]
            try
            {
                int numFullWords = (int)ToBigInteger(felts[0]);
                if (felts.Length == numFullWords + 3)
                {
                    var sb = new StringBuilder();
                    for (int i = 0; i < numFullWords; i++)
                    {
                        var wordInt = ToBigInteger(felts[1 + i]);
                        sb.Append(ToShortString(wordInt));
                    }

                    int pendingLen = (int)ToBigInteger(felts[felts.Length - 1]);
                    if (pendingLen > 0)
                    {
                        var pendingInt = ToBigInteger(felts[felts.Length - 2]);
                        string pendingStr = ToShortString(pendingInt);
                        if (pendingStr.Length > pendingLen)
                            pendingStr = pendingStr.Substring(pendingStr.Length - pendingLen);
                        sb.Append(pendingStr);
                    }

                    return sb.ToString();
                }
            }
            catch
            {
                // Fallback to concatenating short strings if ByteArray format doesn't match
            }

            // Fallback: decode each felt as a short string
            var fallback = new StringBuilder();
            foreach (var felt in felts)
            {
                string part = ToShortString(felt);
                if (!string.IsNullOrEmpty(part))
                    fallback.Append(part);
            }
            return fallback.ToString();
        }
    }
}
