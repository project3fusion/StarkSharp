using System;
using System.Numerics;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto;

namespace StarkSharp.StarkCurve.Extensions
{
    public static class BigIntergerExtensions
    {
        public static int GetBitLength(this BigInteger integer)
        {
            return (int)Math.Ceiling(BigInteger.Log(integer + 1, 2)); // Plus one handles the case when integer is a power of 2.
        }
    }

    public class SeededHMacDsaKCalculator : HMacDsaKCalculator
    {
        private byte[] _seed;

        public SeededHMacDsaKCalculator(IDigest digest) : base(digest)
        {
        }

        public void SetExtraEntropy(byte[] seed)
        {
            this._seed = seed;
        }
        protected override void InitAdditionalInput0(HMac hmac0)
        {
            if (_seed != null && _seed.Length > 0)
            {
                // The seed is added to the HMAC process here.
                hmac0.BlockUpdate(_seed, 0, _seed.Length);
            }
        }
    }

}