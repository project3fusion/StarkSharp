using StarkSharp.Rpc.Modules.Transactions.Prefix;
using StarkSharp.StarkCurve.Signature;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using UnityEngine;

namespace StarkSharp.Rpc.Modules.Transactions.Hash
{
    public class TransactionHash
    {
        public class Call
        {
            public string To { get; set; } // contract address
            public string Selector { get; set; }
            public string[] Data { get; set; }
        }
        public static class Hash
        {
            public static string[] FormatCalldataOther(Call[] callArray)
            {
                return new[] { "0x" + new BigInteger(callArray.Length).ToString("x") }.Concat(callArray.SelectMany(call => new[] { call.To, call.Selector, "0x" + new BigInteger(call.Data.Length).ToString("x") }.Concat(call.Data))).ToArray();
            }

            public static string[] FormatCalldataCairo0(Call[] callArray)
            {
                List<string> calldata = new List<string>();
                List<string> calls = new List<string>();

                calldata.Add("0x" + new BigInteger(callArray.Length).ToString("x"));

                int offset = 0;
                foreach (Call call in callArray)
                {
                    calldata.Add(call.To);
                    calldata.Add(call.Selector);
                    calldata.Add("0x" + offset.ToString("x")); // data offset
                    calldata.Add("0x" + new BigInteger(call.Data.Length).ToString("x")); // data length

                    offset += call.Data.Length;

                    foreach (string data in call.Data)
                    {
                        calls.Add(data);
                    }
                }
                calldata.Add("0x" + offset.ToString("x")); // calldata length
                calldata.AddRange(calls);

                return calldata.ToArray();
            }

            public static string[] FormatCalldata(Call[] callArray, int cairoVersion)
            {
                return cairoVersion == 0 ? FormatCalldataCairo0(callArray) : FormatCalldataOther(callArray);
            }
            public static string ComputeCalldataHash(Call[] callArray, int cairoVersion)
            {
                return cairoVersion == 0 ? ComputeCalldataHashCairo0(callArray) : ComputeCalldataHashOther(callArray);
            }
            public static string ComputeCalldataHashOther(Call[] callArray)
            {
                return ECDSA.PedersenArrayHash(FormatCalldataOther(callArray).Select(HexToBigInteger).ToArray()).ToString("x");
            }

            public static string ComputeCalldataHashCairo0(Call[] callArray)
            {
                return ECDSA.PedersenArrayHash(FormatCalldataCairo0(callArray).Select(HexToBigInteger).ToArray()).ToString("x");
            }

            public static ECDSA.ECSignature SignInvokeTransaction(string version, string senderAddress, string calldataHash, string maxFee, string chainId, string nonce, BigInteger privateKey)
            {
                try
                {
                    BigInteger txHash = TransactionHashInvoke(version, senderAddress, calldataHash, maxFee, chainId, nonce);
                    return ECDSA.Sign(txHash, privateKey);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error signing transaction: {ex.Message}");
                    throw;
                }
            }

            public static BigInteger TransactionHashInvoke(string version, string senderAddress, string calldataHash, string maxFee, string chainId, string nonce)
            {
                string txHashPrefix = TransactionPrefixes.Invoke;
                const string zero = "0x0";

                return CalculateTransactionHash(txHashPrefix, version, senderAddress, zero, calldataHash, maxFee, chainId, nonce);
            }

            public static BigInteger CalculateTransactionHash(string txHashPrefix, string version, string contractAddress, string entryPointSelector, string calldata, string maxFee, string chainId, params string[] additionalData)
            {
                List<string> data = new List<string>
                    {
                        txHashPrefix,
                        version,
                        contractAddress,
                        entryPointSelector,
                        calldata,
                        maxFee,
                        chainId
                    };
                data.AddRange(additionalData);
                return ComputeHashOnElements(data.ToArray());
            }

            private static Dictionary<string, BigInteger> specialCases = new Dictionary<string, BigInteger>
            {
                { "0x289d4c5d81", new BigInteger(11151000167265) }
            };

            private static BigInteger ComputeHashOnElements(string[] data)
            {
                // Convert data strings to big integers using LINQ
                BigInteger[] dataAsBigIntegers = data.Select(d =>
                    specialCases.ContainsKey(d) ? specialCases[d] : HexToBigInteger(d)
                ).ToArray();

                // Compute the hash
                return ECDSA.PedersenArrayHash(dataAsBigIntegers);
            }

            public static BigInteger HexToBigInteger(string hex)
            {
                BigInteger X = BigInteger.Pow(2, 251) + 17 * BigInteger.Pow(2, 192) + 1;
                BigInteger fieldSize = X * 2;  // The field size is 2X because the range is -X to X - 1.
                try
                {
                    var hexNumber = hex.StartsWith("0x") ? hex.Substring(2) : hex; // check if it starts with '0x' and remove it
                    // Try to parse the hex string
                    if (!BigInteger.TryParse(hexNumber, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out BigInteger result))
                    {
                        Console.WriteLine("Error converting hex to BigInteger: Invalid hex string.");
                    }
                    // Make the BigInteger positive if it's interpreted as negative
                    if (result.Sign < 0)
                    {
                        result = new BigInteger(result.ToByteArray().Concat(new byte[] { 0 }).ToArray());
                    }
                    // Ensure the result is within the range -X < result < X
                    if (result >= X)
                    {
                        result = (result + X) % fieldSize - X;  // Mapping the value to the range -X to X - 1.
                    }
                    return result;
                }
                catch (System.Exception e)
                {
                    Console.WriteLine("Error converting hex to BigInteger: " + e.Message);
                    throw;
                }
            }

            public static string BigIntegerToHex(BigInteger bigInteger)
            {
                // Similar to HexToBigInteger, create BigIntegerToHex in the field of order X.
                BigInteger X = BigInteger.Pow(2, 251) + 17 * BigInteger.Pow(2, 192) + 1;
                BigInteger fieldSize = X * 2;  // The field size is 2X because the range is -X to X - 1.
                // Ensure the result is within the range -X < result < X
                if (bigInteger >= X)
                {
                    bigInteger = (bigInteger + X) % fieldSize - X;  // Mapping the value to the range -X to X - 1.
                }
                // Make the BigInteger negative if it's interpreted as positive
                if (bigInteger.Sign > 0)
                {
                    bigInteger = new BigInteger(bigInteger.ToByteArray().Concat(new byte[] { 0 }).ToArray());
                }
                // Convert the BigInteger to a hex string
                string hex = bigInteger.ToString("x");
                // Add the '0x' prefix
                return "0x" + hex;
            }
        }
    }
}