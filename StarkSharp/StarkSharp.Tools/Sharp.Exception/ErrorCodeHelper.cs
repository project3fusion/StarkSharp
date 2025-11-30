using System;
using System.Collections.Generic;

namespace StarkSharp.Tools.Exception
{
    /// <summary>
    /// Helper class for error code operations
    /// </summary>
    public static class ErrorCodeHelper
    {
        private static readonly Dictionary<StarkSharpErrorCode, string> ErrorCategories = new Dictionary<StarkSharpErrorCode, string>
        {
            // General
            { StarkSharpErrorCode.UnknownError, "General" },
            { StarkSharpErrorCode.InvalidParameter, "General" },
            { StarkSharpErrorCode.NullReference, "General" },
            { StarkSharpErrorCode.InvalidOperation, "General" },
            { StarkSharpErrorCode.Timeout, "General" },
            { StarkSharpErrorCode.Cancelled, "General" },

            // Network
            { StarkSharpErrorCode.NetworkError, "Network" },
            { StarkSharpErrorCode.ConnectionFailed, "Network" },
            { StarkSharpErrorCode.ConnectionTimeout, "Network" },
            { StarkSharpErrorCode.RequestFailed, "Network" },
            { StarkSharpErrorCode.InvalidResponse, "Network" },
            { StarkSharpErrorCode.ServerError, "Network" },
            { StarkSharpErrorCode.ClientError, "Network" },
            { StarkSharpErrorCode.HttpError, "Network" },

            // RPC
            { StarkSharpErrorCode.RpcError, "RPC" },
            { StarkSharpErrorCode.RpcInvalidRequest, "RPC" },
            { StarkSharpErrorCode.RpcMethodNotFound, "RPC" },
            { StarkSharpErrorCode.RpcInvalidParams, "RPC" },
            { StarkSharpErrorCode.RpcInternalError, "RPC" },
            { StarkSharpErrorCode.RpcParseError, "RPC" },
            { StarkSharpErrorCode.RpcServerError, "RPC" },
            { StarkSharpErrorCode.RpcTimeout, "RPC" },
            { StarkSharpErrorCode.RpcConnectionFailed, "RPC" },

            // Wallet
            { StarkSharpErrorCode.WalletError, "Wallet" },
            { StarkSharpErrorCode.WalletNotConnected, "Wallet" },
            { StarkSharpErrorCode.WalletConnectionFailed, "Wallet" },
            { StarkSharpErrorCode.WalletDisconnected, "Wallet" },
            { StarkSharpErrorCode.WalletNotFound, "Wallet" },
            { StarkSharpErrorCode.WalletRejected, "Wallet" },
            { StarkSharpErrorCode.WalletTimeout, "Wallet" },
            { StarkSharpErrorCode.InvalidWalletType, "Wallet" },

            // Transaction
            { StarkSharpErrorCode.TransactionError, "Transaction" },
            { StarkSharpErrorCode.TransactionFailed, "Transaction" },
            { StarkSharpErrorCode.TransactionRejected, "Transaction" },
            { StarkSharpErrorCode.TransactionReverted, "Transaction" },
            { StarkSharpErrorCode.TransactionNotFound, "Transaction" },
            { StarkSharpErrorCode.TransactionTimeout, "Transaction" },
            { StarkSharpErrorCode.InvalidTransaction, "Transaction" },
            { StarkSharpErrorCode.InsufficientFee, "Transaction" },
            { StarkSharpErrorCode.InvalidNonce, "Transaction" },

            // Contract
            { StarkSharpErrorCode.ContractError, "Contract" },
            { StarkSharpErrorCode.ContractNotFound, "Contract" },
            { StarkSharpErrorCode.ContractCallFailed, "Contract" },
            { StarkSharpErrorCode.InvalidContractAddress, "Contract" },
            { StarkSharpErrorCode.InvalidEntryPoint, "Contract" },
            { StarkSharpErrorCode.ContractExecutionFailed, "Contract" },
            { StarkSharpErrorCode.InvalidCallData, "Contract" },

            // Account
            { StarkSharpErrorCode.AccountError, "Account" },
            { StarkSharpErrorCode.AccountNotFound, "Account" },
            { StarkSharpErrorCode.InvalidAccountAddress, "Account" },
            { StarkSharpErrorCode.InvalidPrivateKey, "Account" },
            { StarkSharpErrorCode.AccountNotInitialized, "Account" },
            { StarkSharpErrorCode.InsufficientBalance, "Account" },

            // Cryptographic
            { StarkSharpErrorCode.CryptographicError, "Cryptographic" },
            { StarkSharpErrorCode.SignatureGenerationFailed, "Cryptographic" },
            { StarkSharpErrorCode.InvalidPublicKey, "Cryptographic" },
            { StarkSharpErrorCode.HashCalculationFailed, "Cryptographic" },
            { StarkSharpErrorCode.PedersenHashError, "Cryptographic" },

            // Platform
            { StarkSharpErrorCode.PlatformError, "Platform" },
            { StarkSharpErrorCode.PlatformNotSupported, "Platform" },
            { StarkSharpErrorCode.PlatformInitializationFailed, "Platform" },
            { StarkSharpErrorCode.PlatformConnectionFailed, "Platform" },
            { StarkSharpErrorCode.InvalidPlatformType, "Platform" },

            // Configuration
            { StarkSharpErrorCode.ConfigurationError, "Configuration" },
            { StarkSharpErrorCode.InvalidConfiguration, "Configuration" },
            { StarkSharpErrorCode.MissingConfiguration, "Configuration" },
            { StarkSharpErrorCode.InvalidRpcUrl, "Configuration" },
            { StarkSharpErrorCode.InvalidChainId, "Configuration" },

            // Validation
            { StarkSharpErrorCode.ValidationError, "Validation" },
            { StarkSharpErrorCode.InvalidAddress, "Validation" },
            { StarkSharpErrorCode.InvalidHexString, "Validation" },
            { StarkSharpErrorCode.InvalidBigInteger, "Validation" },
            { StarkSharpErrorCode.InvalidCairoVersion, "Validation" },
            { StarkSharpErrorCode.InvalidFunctionSelector, "Validation" },

            // Serialization
            { StarkSharpErrorCode.SerializationError, "Serialization" },
            { StarkSharpErrorCode.DeserializationError, "Serialization" },
            { StarkSharpErrorCode.InvalidJson, "Serialization" },
            { StarkSharpErrorCode.JsonParseError, "Serialization" }
        };

        public static string GetCategory(StarkSharpErrorCode errorCode)
        {
            return ErrorCategories.TryGetValue(errorCode, out var category) 
                ? category 
                : "Unknown";
        }

        public static string GetErrorMessage(StarkSharpErrorCode errorCode)
        {
            return errorCode switch
            {
                StarkSharpErrorCode.UnknownError => "An unknown error occurred",
                StarkSharpErrorCode.InvalidParameter => "Invalid parameter provided",
                StarkSharpErrorCode.NullReference => "Null reference encountered",
                StarkSharpErrorCode.InvalidOperation => "Invalid operation attempted",
                StarkSharpErrorCode.Timeout => "Operation timed out",
                StarkSharpErrorCode.Cancelled => "Operation was cancelled",
                StarkSharpErrorCode.NetworkError => "Network error occurred",
                StarkSharpErrorCode.ConnectionFailed => "Failed to establish connection",
                StarkSharpErrorCode.ConnectionTimeout => "Connection timed out",
                StarkSharpErrorCode.RequestFailed => "Request failed",
                StarkSharpErrorCode.InvalidResponse => "Invalid response received",
                StarkSharpErrorCode.ServerError => "Server error occurred",
                StarkSharpErrorCode.ClientError => "Client error occurred",
                StarkSharpErrorCode.HttpError => "HTTP error occurred",
                StarkSharpErrorCode.RpcError => "RPC error occurred",
                StarkSharpErrorCode.RpcInvalidRequest => "Invalid RPC request",
                StarkSharpErrorCode.RpcMethodNotFound => "RPC method not found",
                StarkSharpErrorCode.RpcInvalidParams => "Invalid RPC parameters",
                StarkSharpErrorCode.RpcInternalError => "RPC internal error",
                StarkSharpErrorCode.RpcParseError => "RPC parse error",
                StarkSharpErrorCode.RpcServerError => "RPC server error",
                StarkSharpErrorCode.RpcTimeout => "RPC request timed out",
                StarkSharpErrorCode.RpcConnectionFailed => "RPC connection failed",
                StarkSharpErrorCode.WalletError => "Wallet error occurred",
                StarkSharpErrorCode.WalletNotConnected => "Wallet is not connected",
                StarkSharpErrorCode.WalletConnectionFailed => "Failed to connect wallet",
                StarkSharpErrorCode.WalletDisconnected => "Wallet disconnected",
                StarkSharpErrorCode.WalletNotFound => "Wallet not found",
                StarkSharpErrorCode.WalletRejected => "Wallet operation rejected",
                StarkSharpErrorCode.WalletTimeout => "Wallet operation timed out",
                StarkSharpErrorCode.InvalidWalletType => "Invalid wallet type",
                StarkSharpErrorCode.TransactionError => "Transaction error occurred",
                StarkSharpErrorCode.TransactionFailed => "Transaction failed",
                StarkSharpErrorCode.TransactionRejected => "Transaction rejected",
                StarkSharpErrorCode.TransactionReverted => "Transaction reverted",
                StarkSharpErrorCode.TransactionNotFound => "Transaction not found",
                StarkSharpErrorCode.TransactionTimeout => "Transaction timed out",
                StarkSharpErrorCode.InvalidTransaction => "Invalid transaction",
                StarkSharpErrorCode.InsufficientFee => "Insufficient fee",
                StarkSharpErrorCode.InvalidNonce => "Invalid nonce",
                StarkSharpErrorCode.ContractError => "Contract error occurred",
                StarkSharpErrorCode.ContractNotFound => "Contract not found",
                StarkSharpErrorCode.ContractCallFailed => "Contract call failed",
                StarkSharpErrorCode.InvalidContractAddress => "Invalid contract address",
                StarkSharpErrorCode.InvalidEntryPoint => "Invalid entry point",
                StarkSharpErrorCode.ContractExecutionFailed => "Contract execution failed",
                StarkSharpErrorCode.InvalidCallData => "Invalid call data",
                StarkSharpErrorCode.AccountError => "Account error occurred",
                StarkSharpErrorCode.AccountNotFound => "Account not found",
                StarkSharpErrorCode.InvalidAccountAddress => "Invalid account address",
                StarkSharpErrorCode.InvalidPrivateKey => "Invalid private key",
                StarkSharpErrorCode.AccountNotInitialized => "Account not initialized",
                StarkSharpErrorCode.InsufficientBalance => "Insufficient balance",
                StarkSharpErrorCode.CryptographicError => "Cryptographic error occurred",
                StarkSharpErrorCode.SignatureGenerationFailed => "Signature generation failed",
                StarkSharpErrorCode.InvalidPublicKey => "Invalid public key",
                StarkSharpErrorCode.HashCalculationFailed => "Hash calculation failed",
                StarkSharpErrorCode.PedersenHashError => "Pedersen hash error",
                StarkSharpErrorCode.PlatformError => "Platform error occurred",
                StarkSharpErrorCode.PlatformNotSupported => "Platform not supported",
                StarkSharpErrorCode.PlatformInitializationFailed => "Platform initialization failed",
                StarkSharpErrorCode.PlatformConnectionFailed => "Platform connection failed",
                StarkSharpErrorCode.InvalidPlatformType => "Invalid platform type",
                StarkSharpErrorCode.ConfigurationError => "Configuration error occurred",
                StarkSharpErrorCode.InvalidConfiguration => "Invalid configuration",
                StarkSharpErrorCode.MissingConfiguration => "Missing configuration",
                StarkSharpErrorCode.InvalidRpcUrl => "Invalid RPC URL",
                StarkSharpErrorCode.InvalidChainId => "Invalid chain ID",
                StarkSharpErrorCode.ValidationError => "Validation error occurred",
                StarkSharpErrorCode.InvalidAddress => "Invalid address",
                StarkSharpErrorCode.InvalidHexString => "Invalid hex string",
                StarkSharpErrorCode.InvalidBigInteger => "Invalid big integer",
                StarkSharpErrorCode.InvalidCairoVersion => "Invalid Cairo version",
                StarkSharpErrorCode.InvalidFunctionSelector => "Invalid function selector",
                StarkSharpErrorCode.SerializationError => "Serialization error occurred",
                StarkSharpErrorCode.DeserializationError => "Deserialization error occurred",
                StarkSharpErrorCode.InvalidJson => "Invalid JSON",
                StarkSharpErrorCode.JsonParseError => "JSON parse error",
                _ => "Unknown error"
            };
        }
    }
}

