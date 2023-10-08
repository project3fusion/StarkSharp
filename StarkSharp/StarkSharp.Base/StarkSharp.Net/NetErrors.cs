﻿using System;
using static StarkSharp.StarkSharp.Base.StarkSharp.Net.NetEnum;

namespace StarkSharp.StarkSharp.Base.StarkSharp.Net
{
    public static class NetErrors
    {
        public const string CLIENT_ERROR_CODE = "SomeErrorCode"; // Örnek hata kodu. İhtiyacınıza göre değiştirebilirsiniz.
    }

    public class ClientError : Exception
    {
        public string Code { get; }

        public ClientError(string message, string code = null)
            : base($"Client failed{(string.IsNullOrEmpty(code) ? "" : $" with code {code}")}: {message}.")
        {
            Code = code;
        }
    }

    public class ContractNotFoundError : ClientError
    {
        public ContractNotFoundError(string address, string blockHash = null, object blockNumber = null)
            : base(GenerateMessage(address, blockHash, blockNumber))
        {
        }

        private static string GenerateMessage(string address, string blockHash, object blockNumber)
        {
            bool isIdentifier = blockHash != null || blockNumber != null;
            object identifier = blockHash ?? blockNumber;
            string identifierName = blockHash != null ? "block_hash" : "block_number";

            string message = $"No contract with address {address} found";
            string blockInfo = isIdentifier ? $" for block with {identifierName}: {identifier}" : "";

            return message + blockInfo;
        }
    }
}
