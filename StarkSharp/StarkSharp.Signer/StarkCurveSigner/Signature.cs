using System;
using System.IO;
using System.Numerics;
using System.Globalization;
using System.Diagnostics;
using Newtonsoft.Json;
using StarkSharp.StarkCurve.Extensions;
using StarkSharp.StarkCurve.Utils;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Math;
using BouncyBigInt = Org.BouncyCastle.Math.BigInteger;
using BigInt = System.Numerics.BigInteger;
using System.Collections.Generic;
using System.Linq;

namespace StarkSharp.StarkCurve.Signature
{
    public class StarkCurveParameters
    {
        // Map the properties to the JSON fields. Property names must match the JSON field names.
        [JsonProperty("FIELD_PRIME")]
        public BigInt FieldPrime { get; set; }

        [JsonProperty("FIELD_GEN")]
        public BigInt FieldGen { get; set; }

        [JsonProperty("ALPHA")]
        public BigInt Alpha { get; set; }

        [JsonProperty("BETA")]
        public BigInt Beta { get; set; }

        [JsonProperty("EC_ORDER")]
        public BigInt EcOrder { get; set; }

        [JsonProperty("CONSTANT_POINTS")]
        public List<List<BigInt>> ConstantPoints { get; set; }
    }

    public static class ECDSA
    {
        private const string PedersenHashPointFilename = "StarkSharp.Signer/StarkCurveSigner/pedersen_params.json";

        // Load the parameters from pedersen_params.json
        private static readonly StarkCurveParameters PedersenParams = JsonConvert.DeserializeObject<StarkCurveParameters>(
            File.ReadAllText(PedersenHashPointFilename));

        // Field parameters.
        public static readonly BigInt FieldPrime = PedersenParams.FieldPrime;
        public static readonly BigInt FieldGen = PedersenParams.FieldGen;
        public static readonly BigInt Alpha = PedersenParams.Alpha;
        public static readonly BigInt Beta = PedersenParams.Beta;
        public static readonly BigInt EcOrder = PedersenParams.EcOrder;
        public static readonly List<List<BigInt>> ConstantPoints = PedersenParams.ConstantPoints;
        public static MathUtils.ECPoint ShiftPoint => new MathUtils.ECPoint(
            PedersenParams.ConstantPoints[0][0],
            PedersenParams.ConstantPoints[0][1]
        );

        public static MathUtils.ECPoint EcGen => new MathUtils.ECPoint(
            PedersenParams.ConstantPoints[1][0],
            PedersenParams.ConstantPoints[1][1]
        );
        public static MathUtils.ECPoint MinusShiftPoint => new MathUtils.ECPoint(
            ShiftPoint.X,
            FieldPrime - ShiftPoint.Y
        );
        public static readonly int NElementBitsEcdsa = 251;
        public static readonly int NElementBitsHash = (int)FieldPrime.GetBitLength();

        static ECDSA()
        {
            VerifyParameters();
        }

        public static void VerifyParameters()
        {
            // Calculate the number of bits in FIELD_PRIME.
            int nElementBitsEcdsa = (int)Math.Floor(BigInt.Log(FieldPrime, 2));
            Debug.Assert(nElementBitsEcdsa == 251, "nElementBitsEcdsa must be 251 bits.");

            // Calculate the bit length of FIELD_PRIME for hash operations.
            int nElementBitsHash = (int)FieldPrime.GetBitLength();
            Debug.Assert(nElementBitsHash == 252, "nElementBitsHash must be 252 bits.");

            // Assert the EC order conditions.
            Debug.Assert(BigInt.Pow(2, nElementBitsEcdsa) < EcOrder && EcOrder < FieldPrime,
                         "EC order must be greater than 2^nElementBitsEcdsa and less than FIELD_PRIME.");

            var expectedShiftPoint = new MathUtils.ECPoint(
                BigInt.Parse("49EE3EBA8C1600700EE1B87EB599F16716B0B1022947733551FDE4050CA6804", NumberStyles.AllowHexSpecifier),
                BigInt.Parse("3CA0CFE4B3BC6DDF346D49D06EA0ED34E621062C0E056C1D0405D266E10268A", NumberStyles.AllowHexSpecifier));

            var expectedEcGen = new MathUtils.ECPoint(
                BigInt.Parse("1EF15C18599971B7BECED415A40F0C7DEACFD9B0D1819E03D723D8BC943CFCA", NumberStyles.AllowHexSpecifier),
                BigInt.Parse("5668060AA49730B7BE4801DF46EC62DE53ECD11ABE43A32873000C36E8DC1F", NumberStyles.AllowHexSpecifier));

            // Assertions to ensure the points are what we expect.
            Debug.Assert(ShiftPoint.X == expectedShiftPoint.X && ShiftPoint.Y == expectedShiftPoint.Y,
                         "SHIFT_POINT is not as expected.");
            Debug.Assert(EcGen.X == expectedEcGen.X && EcGen.Y == expectedEcGen.Y,
                         "EC_GEN is not as expected.");
        }

        public class ECSignature
        {
            public BigInt R { get; }
            public BigInt S { get; }

            public ECSignature(BigInt r, BigInt s)
            {
                R = r;
                S = s;
            }
        }

        /**
            Given the x coordinate of a stark_key, returns a possible y coordinate such that together the
            point (x,y) is on the curve.
            Note that the real y coordinate is either y or -y.
            If x is invalid stark_key it throws an error.
        **/
        // (ref: https://github.com/starkware-libs/cairo-lang/blob/master/src/starkware/crypto/signature/signature.py#L84)
        public static BigInt GetYCoordinate(BigInt starkKeyXCoordinate)
        {
            var x = starkKeyXCoordinate;
            var ySquared = (BigInt.ModPow(x, 3, FieldPrime) + Alpha * x + Beta) % FieldPrime;

            if (!MathUtils.IsQuadResidue(ySquared, FieldPrime))
            {
                throw new Errors.InvalidPublicKeyError();
            }
            return MathUtils.SqrtMod(ySquared, FieldPrime);
        }

        // Returns a private key in the range [1, EC_ORDER).
        // (ref: https://github.com/starkware-libs/cairo-lang/blob/master/src/starkware/crypto/signature/signature.py#L99)
        public static BigInt GetRandomPrivateKey()
        {
            SecureRandom secureRandom = new SecureRandom();
            BouncyBigInt privateKey;
            // Calculate the number of bits in EcOrder.
            int bitLength = (int)Math.Ceiling(BigInt.Log(EcOrder, 2));

            // This loop is to ensure that 0 < privateKey < EcOrder.
            // A private key of '0' or one equal to 'EcOrder' is invalid.
            do
            {
                privateKey = new BouncyBigInt(bitLength, secureRandom);
            } while (privateKey.CompareTo(BouncyBigInt.Zero) <= 0 || privateKey.CompareTo(EcOrder) >= 0);

            return new BigInt(privateKey.ToByteArrayUnsigned());
        }

        // Obtain public key coordinates from stark curve given the private key
        // (ref: https://github.com/starkware-libs/cairo-lang/blob/master/src/starkware/crypto/signature/signature.py#L104)
        public static MathUtils.ECPoint PrivateKeyToECPointOnStarkCurve(BigInt privKey)
        {
            if (privKey <= 0 || privKey >= EcOrder)
                throw new ArgumentOutOfRangeException(nameof(privKey), "Private key must be in the range (0, EC_ORDER).");

            return MathUtils.ECMult(privKey, EcGen, Alpha, FieldPrime);
        }

        // (ref: https://github.com/starkware-libs/cairo-lang/blob/master/src/starkware/crypto/signature/signature.py#L109)
        public static BigInt PrivateToStarkKey(BigInt privKey)
        {
            return PrivateKeyToECPointOnStarkCurve(privKey).X;
        }

        // (ref: https://github.com/starkware-libs/cairo-lang/blob/master/src/starkware/crypto/signature/signature.py#L113)
        public static BigInt InvModCurveSize(BigInt x)
        {
            return MathUtils.DivMod(1, x, EcOrder);
        }

        // TODO: Fix back and forth conversion of BigInt and BouncyBigInt
        // (ref: https://github.com/starkware-libs/cairo-lang/blob/master/src/starkware/crypto/signature/signature.py#L117)
        public static BigInt GenerateKRFC6979(BigInt msgHash, BigInt privKey, BigInt? seed = null)
        {
            // Convert to BouncyCastle's BigInteger type.
            var bcPrivKey = new BouncyBigInt(privKey.ToString());
            var bcEcOrder = new BouncyBigInt(EcOrder.ToString());
            var bcMsgHash = new BouncyBigInt(msgHash.ToString());

            // Prepare message hash
            byte[] preparedMsgHash = PrepareMessageHash(bcMsgHash);

            // Prepare the signer with the new SeededHMacDsaKCalculator
            var signer = new SeededHMacDsaKCalculator(new Sha256Digest());

            // If a seed is provided, convert it to a byte array and set it as extra entropy
            if (seed.HasValue)
            {
                if (seed.Value < 0)
                {
                    throw new ArgumentOutOfRangeException("seed must be non-negative");
                }

                var seedBytes = seed.Value.ToByteArray();

                // Ensure big-endian byte order
                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(seedBytes);
                }

                // Remove potential extra zero byte
                if (seedBytes.Length > 1 && seedBytes[0] == 0)
                {
                    seedBytes = seedBytes.Skip(1).ToArray();
                }

                signer.SetExtraEntropy(seedBytes); // Set the seed bytes as extra entropy
            }

            // Initialize with private key's value modulo EcOrder.
            signer.Init(bcEcOrder, bcPrivKey, preparedMsgHash);

            // Generate and return the 'k' value
            var nextK = signer.NextK();
            return new BigInt(nextK.ToByteArrayUnsigned());
        }

        private static byte[] PrepareMessageHash(BouncyBigInt msgHash)
        {
            if (1 <= msgHash.BitLength % 8 && msgHash.BitLength % 8 <= 4 && msgHash.BitLength >= 248)
            {
                msgHash = msgHash.ShiftLeft(4);
            }

            // Convert the message hash to a byte array
            byte[] msgHashBytes = msgHash.ToByteArray();
            return msgHashBytes;
        }

        // (ref: https://github.com/starkware-libs/cairo-lang/blob/master/src/starkware/crypto/signature/signature.py#L137)
        public static ECSignature Sign(BigInt msgHash, BigInt privKey, BigInt? seed = null)
        {
            /**
                msg_hash must be smaller than 2**N_ELEMENT_BITS_ECDSA.
                Message whose hash is >= 2**N_ELEMENT_BITS_ECDSA cannot be signed.
                This happens with a very small probability.
            **/
            if (msgHash < 0 || msgHash >= BigInt.Pow(2, NElementBitsEcdsa))
                throw new ArgumentException("Message not signable.");

            /**
                Choose a valid k. In our version of ECDSA not every k value is valid,
                and there is a negligible probability a drawn k cannot be used for signing.
                This is why we have this loop.
            **/
            while (true)
            {
                var k = GenerateKRFC6979(msgHash, privKey, seed);
                seed = seed.HasValue ? seed + 1 : new BigInt(1); // Update seed for next iteration in case the value of k is bad.

                var x = (MathUtils.ECMult(k, EcGen, Alpha, FieldPrime)).X;

                var r = new BigInt(x.ToByteArray());

                if (r <= 0 || r >= BigInt.Pow(2, NElementBitsEcdsa))
                    // Bad value. This fails with negligible probability.
                    continue;

                var temp = (msgHash + r * privKey) % EcOrder;

                if (temp == 0)
                    // Bad value. This fails with negligible probability.
                    continue;

                var w = MathUtils.DivMod(k, temp, EcOrder);

                if (w <= 0 || w >= BigInt.Pow(2, NElementBitsEcdsa))
                    // Bad value. This fails with negligible probability.
                    continue;

                var s = InvModCurveSize(w);

                return new ECSignature(r, s);
            }
        }

        public static MathUtils.ECPoint MimicEcMultAir(BigInt m, MathUtils.ECPoint point, MathUtils.ECPoint shiftPoint)
        {
            if (m <= 0 || m >= BigInt.Pow(2, NElementBitsEcdsa))
                throw new ArgumentException("Invalid 'm' value");

            MathUtils.ECPoint partialSum = shiftPoint;
            for (int i = 0; i < NElementBitsEcdsa; i++)
            {
                if (partialSum.X == point.X)
                    throw new InvalidOperationException("Invalid operation");

                if ((m & 1) == 1)
                    partialSum = MathUtils.ECAdd(partialSum, point, FieldPrime);

                point = MathUtils.ECDouble(point, Alpha, FieldPrime);
                m >>= 1;
            }

            if (m != 0)
                throw new InvalidOperationException("Invalid operation");

            return partialSum;
        }

        public static bool IsPointOnCurve(BigInt x, BigInt y)
        {
            BigInt leftSide = BigInt.ModPow(y, 2, FieldPrime);
            BigInt rightSide = (BigInt.ModPow(x, 3, FieldPrime) + Alpha * x + Beta) % FieldPrime;

            return leftSide == rightSide;
        }

        public static bool IsValidStarkPrivateKey(BigInt privateKey)
        {
            return privateKey > 0 && privateKey < EcOrder;
        }

        public static bool IsValidStarkKey(BigInt starkKey)
        {
            try
            {
                GetYCoordinate(starkKey);
            }
            catch (InvalidOperationException)
            {
                return false;
            }
            return true;
        }

        // (ref: https://github.com/starkware-libs/cairo-lang/blob/master/src/starkware/crypto/signature/signature.py#L217)
        public static bool Verify(BigInt msgHash, BigInt r, BigInt s, object publicKey) // publicKey can be int or ECPoint
        {
            // Check the bounds for 's'
            if (s <= 1 || s >= EcOrder)
                throw new ArgumentException($"Invalid 's' value: {s}");

            BigInt w = InvModCurveSize(s);

            // Perform preassumptions checks
            if (r <= 1 || r >= BigInt.Pow(2, NElementBitsEcdsa))
                throw new ArgumentException($"Invalid 'r' value: {r}");

            if (w <= 1 || w >= BigInt.Pow(2, NElementBitsEcdsa))
                throw new ArgumentException($"Invalid 'w' value: {w}");

            if (msgHash < 0 || msgHash >= BigInt.Pow(2, NElementBitsEcdsa))
                throw new ArgumentException($"Invalid 'msg_hash' value: {msgHash}");

            MathUtils.ECPoint publicKeyPoint;
            if (publicKey is BigInt publicKeyInt)
            {
                // Only the x coordinate of the point is given, check the two possibilities for the y coordinate.
                try
                {
                    BigInt y = GetYCoordinate(publicKeyInt);
                    publicKeyPoint = new MathUtils.ECPoint(publicKeyInt, y);
                }
                catch (Exception) // Catch the specific exception that GetYCoordinate throws for an invalid key
                {
                    return false;
                }

                // Attempt verification with both possible y coordinates
                return Verify(msgHash, r, s, publicKeyPoint) ||
                       Verify(msgHash, r, s, new MathUtils.ECPoint(publicKeyPoint.X, (-publicKeyPoint.Y) % (FieldPrime)));
            }
            else if (publicKey is MathUtils.ECPoint point)
            {
                publicKeyPoint = point;
            }
            else
            {
                throw new ArgumentException("Invalid public key type");
            }

            // Ensure the public key point is on the curve
            if (!IsPointOnCurve(publicKeyPoint.X, publicKeyPoint.Y))
                throw new ArgumentException("Public key is not on the curve");

            // Signature validation
            try
            {
                MathUtils.ECPoint zG = MimicEcMultAir(msgHash, EcGen, MinusShiftPoint);
                MathUtils.ECPoint rQ = MimicEcMultAir(r, publicKeyPoint, ShiftPoint);
                MathUtils.ECPoint wB = MimicEcMultAir(w, MathUtils.ECAdd(zG, rQ, FieldPrime), ShiftPoint);
                BigInt x = MathUtils.ECAdd(wB, MinusShiftPoint, FieldPrime).X;

                // Comparison without mod n, differing from classic ECDSA
                return r == x;
            }
            catch (Exception)
            {
                // Catch any assertion errors from the computations
                return false;
            }
        }

        public static BigInt PedersenHash(params BigInt[] elements)
        {
            return PedersenHashAsPoint(elements).X;
        }

        public static MathUtils.ECPoint PedersenHashAsPoint(params BigInt[] elements)
        {
            MathUtils.ECPoint point = ShiftPoint;

            for (int i = 0; i < elements.Length; i++)
            {
                BigInt x = elements[i];
                if (x < 0 || x >= FieldPrime)
                    throw new ArgumentException("Element out of bounds", nameof(elements));

                List<List<BigInt>> pointList = ConstantPoints.GetRange(2 + i * NElementBitsHash, NElementBitsHash);

                if (pointList.Count != NElementBitsHash)
                    throw new InvalidOperationException("Invalid point list size");

                foreach (List<BigInt> pt in pointList)
                {
                    if (point.X == pt[0])
                        throw new InvalidOperationException("Unhashable input due to point collision");

                    MathUtils.ECPoint ecPt = new MathUtils.ECPoint(pt[0], pt[1]);
                    if ((x & 1) != 0)
                    {
                        point = MathUtils.ECAdd(point, ecPt, FieldPrime);
                    }


                    x >>= 1;
                }

                if (x != 0)
                    throw new InvalidOperationException("Non-zero value after processing bits");
            }

            return point;
        }

        public static BigInt PedersenArrayHash(params BigInt[] elements)
        {
            if (elements.Length == 0)
            {
                return PedersenHashAsPoint().X;
            }

            MathUtils.ECPoint point = new MathUtils.ECPoint();
            for (int i = 0; i < elements.Length; i++)
            {
                point = PedersenHashAsPoint(point.X, elements[i]);
                Console.WriteLine(point.X);
            }

            return point.X;
        }
    }
}

