using System;
using System.Numerics;
using Newtonsoft.Json.Linq;
using StarkSharp.Api.RPC;

namespace StarkSharp.Rpc.Utils
{
    /// <summary>
    /// Represents a 256-bit unsigned integer (u256) in Starknet, composed of two 128-bit felts: { low, high }.
    /// Value = low + (high * 2^128).
    /// </summary>
    public readonly struct Uint256 : IEquatable<Uint256>, IComparable<Uint256>
    {
        private static readonly BigInteger TwoPow128 = BigInteger.One << 128;
        private static readonly BigInteger Mask128 = TwoPow128 - BigInteger.One;

        public static readonly Uint256 Zero = new Uint256(BigInteger.Zero);
        public static readonly Uint256 One = new Uint256(BigInteger.One);

        public BigInteger Value { get; }
        public BigInteger Low => Value & Mask128;
        public BigInteger High => Value >> 128;

        public string LowHex => Felt.ToHex(Low);
        public string HighHex => Felt.ToHex(High);

        public Uint256(BigInteger value)
        {
            if (value < BigInteger.Zero)
                throw new ArgumentOutOfRangeException(nameof(value), "Uint256 cannot be negative.");

            Value = value;
        }

        public Uint256(BigInteger low, BigInteger high)
        {
            if (low < BigInteger.Zero || high < BigInteger.Zero)
                throw new ArgumentOutOfRangeException("Low and high parts cannot be negative.");

            Value = (low & Mask128) + (high << 128);
        }

        public Uint256(long value) : this(new BigInteger(value)) { }
        public Uint256(ulong value) : this(new BigInteger(value)) { }

        /// <summary>
        /// Creates a <see cref="Uint256"/> from low and high felt strings (hex 0x... or decimal).
        /// </summary>
        public static Uint256 FromFelts(string? lowFelt, string? highFelt)
        {
            var low = Felt.ToBigInteger(lowFelt);
            var high = Felt.ToBigInteger(highFelt);
            return new Uint256(low, high);
        }

        /// <summary>
        /// Reads a <see cref="Uint256"/> from a felt array at the specified offset (reading 2 felts: low, high).
        /// If only 1 felt is available, it is treated as the value directly.
        /// </summary>
        public static Uint256 FromResult(string[]? felts, int offset = 0)
        {
            if (felts == null || felts.Length <= offset)
                return Zero;

            if (felts.Length == offset + 1)
                return new Uint256(Felt.ToBigInteger(felts[offset]));

            var low = Felt.ToBigInteger(felts[offset]);
            var high = Felt.ToBigInteger(felts[offset + 1]);
            return new Uint256(low, high);
        }

        /// <summary>
        /// Converts the <see cref="Uint256"/> to (low, high) hex string tuple.
        /// </summary>
        public (string Low, string High) ToFelts() => (LowHex, HighHex);

        /// <summary>
        /// Converts the <see cref="Uint256"/> to calldata array [lowHex, highHex].
        /// </summary>
        public string[] ToCalldata() => new[] { LowHex, HighHex };

        /// <summary>
        /// Converts a <see cref="BigInteger"/> into (low, high) hex string tuple.
        /// </summary>
        public static (string Low, string High) ToFelts(BigInteger value) =>
            new Uint256(value).ToFelts();

        /// <summary>
        /// Converts a <see cref="BigInteger"/> into calldata array [lowHex, highHex].
        /// </summary>
        public static string[] ToCalldata(BigInteger value) =>
            new Uint256(value).ToCalldata();

        /// <summary>
        /// Parses a JsonRpcResponse or JSON array or felt into a BigInteger.
        /// </summary>
        public static BigInteger ParseResult(JsonRpcResponse? response)
        {
            if (response == null || response.HasError || response.result == null)
                return BigInteger.Zero;

            return FromResult(response.ResultAsArray).Value;
        }

        /// <summary>
        /// Parses a JSON string (array or single felt) into a BigInteger.
        /// </summary>
        public static BigInteger ParseResult(string? resultJson)
        {
            if (string.IsNullOrWhiteSpace(resultJson))
                return BigInteger.Zero;

            try
            {
                var token = JToken.Parse(resultJson);
                if (token is JArray jArray)
                {
                    return FromResult(jArray.ToObject<string[]>()).Value;
                }
            }
            catch
            {
                // Not valid JSON array, fallback to direct felt parse
            }

            return Felt.ToBigInteger(resultJson);
        }

        // Implicit conversions
        public static implicit operator BigInteger(Uint256 u) => u.Value;
        public static implicit operator Uint256(BigInteger b) => new Uint256(b);
        public static implicit operator Uint256(long l) => new Uint256(l);
        public static implicit operator Uint256(ulong l) => new Uint256(l);
        public static implicit operator Uint256(int i) => new Uint256(i);

        // Arithmetic
        public static Uint256 operator +(Uint256 a, Uint256 b) => new Uint256(a.Value + b.Value);
        public static Uint256 operator -(Uint256 a, Uint256 b) => new Uint256(a.Value - b.Value);
        public static Uint256 operator *(Uint256 a, Uint256 b) => new Uint256(a.Value * b.Value);
        public static Uint256 operator /(Uint256 a, Uint256 b) => new Uint256(a.Value / b.Value);

        // Comparison & Equality
        public bool Equals(Uint256 other) => Value.Equals(other.Value);
        public override bool Equals(object? obj) => obj is Uint256 other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();
        public int CompareTo(Uint256 other) => Value.CompareTo(other.Value);

        public static bool operator ==(Uint256 left, Uint256 right) => left.Equals(right);
        public static bool operator !=(Uint256 left, Uint256 right) => !left.Equals(right);
        public static bool operator <(Uint256 left, Uint256 right) => left.Value < right.Value;
        public static bool operator <=(Uint256 left, Uint256 right) => left.Value <= right.Value;
        public static bool operator >(Uint256 left, Uint256 right) => left.Value > right.Value;
        public static bool operator >=(Uint256 left, Uint256 right) => left.Value >= right.Value;

        public override string ToString() => Value.ToString();
        public string ToString(string? format) => Value.ToString(format);
    }
}
