using System;
using System.Globalization;
using System.Numerics;
using System.Text.RegularExpressions;

namespace StarkSharp.Rpc.Utils
{
    public static class NumericOps
    {
        public static BigInteger StringToBigInt(string inputValue)
        {
            if (string.IsNullOrWhiteSpace(inputValue))
                throw new ArgumentException("The provided string cannot be null or empty.", nameof(inputValue));

            inputValue = inputValue.Trim();

            try
            {
                if (inputValue.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                {
                    string hex = inputValue.Substring(2);
                    if (string.IsNullOrEmpty(hex)) return BigInteger.Zero;
                    return BigInteger.Parse("0" + hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                }
                return BigInteger.Parse(inputValue, NumberStyles.Integer, CultureInfo.InvariantCulture);
            }
            catch (FormatException fe)
            {
                throw new ArgumentException($"The provided string '{inputValue}' cannot be converted to BigInteger.", fe);
            }
        }

        public static bool IsOfBigIntegerType(object? testObject)
        {
            return testObject is BigInteger;
        }

        public static string BigIntToHexFormat(BigInteger value)
        {
            return ComposeHex(value.ToString("x"));
        }

        public static string StringNumberToHexRepresentation(string numStr)
        {
            BigInteger value = StringToBigInt(numStr);
            return BigIntToHexFormat(value);
        }

        public static string ByteArrayToHexPresentation(byte[] dataArray)
        {
            if (dataArray == null || dataArray.Length == 0) return "0x0";
            return ComposeHex(BitConverter.ToString(dataArray).Replace("-", "").ToLowerInvariant());
        }

        public static bool IsPureNumber(string checkStr)
        {
            if (string.IsNullOrWhiteSpace(checkStr)) return false;
            return Regex.IsMatch(checkStr.Trim(), @"^\d+$");
        }

        public static bool IsHexFormatValid(string potentialHex)
        {
            if (string.IsNullOrWhiteSpace(potentialHex)) return false;
            return Regex.IsMatch(potentialHex.Trim(), @"^0x[0-9a-fA-F]+$");
        }

        public static string TransformHexToDecimalStr(string hexVal)
        {
            VerifyHexCorrectness(hexVal);
            return StringToBigInt(hexVal).ToString(CultureInfo.InvariantCulture);
        }

        private static string ComposeHex(string baseHex)
        {
            if (string.IsNullOrEmpty(baseHex)) return "0x0";
            return baseHex.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? baseHex : $"0x{baseHex}";
        }

        private static void VerifyHexCorrectness(string potentialHex)
        {
            if (!IsHexFormatValid(potentialHex))
            {
                throw new InvalidOperationException($"The provided string '{potentialHex}' doesn't represent a valid hexadecimal format.");
            }
        }
    }
}
