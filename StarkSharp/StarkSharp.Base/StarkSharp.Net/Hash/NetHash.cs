using System;
using System.Numerics;

namespace StarkSharp.Base.Net.Hash
{
    /// <summary>
    /// Represents a Starknet hash value
    /// </summary>
    public class NetHash
    {
        private readonly string _value;

        public NetHash(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Hash value cannot be null or empty", nameof(value));

            // Remove 0x prefix if present for normalization
            _value = value.StartsWith("0x", StringComparison.OrdinalIgnoreCase) 
                ? value 
                : $"0x{value}";
        }

        public NetHash(BigInteger value)
        {
            _value = $"0x{value:X}";
        }

        public string Value => _value;
        public string Hex => _value;

        public override string ToString() => _value;

        public override bool Equals(object obj)
        {
            if (obj is NetHash other)
                return _value.Equals(other._value, StringComparison.OrdinalIgnoreCase);
            
            if (obj is string str)
                return _value.Equals(str, StringComparison.OrdinalIgnoreCase);
            
            return false;
        }

        public override int GetHashCode() => _value.GetHashCode();

        public static implicit operator string(NetHash hash) => hash?._value;
        public static implicit operator NetHash(string value) => value == null ? null : new NetHash(value);
        public static implicit operator NetHash(BigInteger value) => new NetHash(value);

        public static bool operator ==(NetHash left, NetHash right)
        {
            if (ReferenceEquals(left, right)) return true;
            if (left is null || right is null) return false;
            return left.Equals(right);
        }

        public static bool operator !=(NetHash left, NetHash right) => !(left == right);
    }
}
