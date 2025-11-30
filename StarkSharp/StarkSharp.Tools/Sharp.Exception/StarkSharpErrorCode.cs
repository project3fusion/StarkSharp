namespace StarkSharp.Tools.Exception
{
    /// <summary>
    /// Custom error codes for StarkSharp
    /// </summary>
    public enum StarkSharpErrorCode
    {
        // General Errors (1000-1999)
        UnknownError = 1000,
        InvalidParameter = 1001,
        NullReference = 1002,
        InvalidOperation = 1003,
        Timeout = 1004,
        Cancelled = 1005,

        // Network Errors (2000-2999)
        NetworkError = 2000,
        ConnectionFailed = 2001,
        ConnectionTimeout = 2002,
        RequestFailed = 2003,
        InvalidResponse = 2004,
        ServerError = 2005,
        ClientError = 2006,
        HttpError = 2007,

        // RPC Errors (3000-3999)
        RpcError = 3000,
        RpcInvalidRequest = 3001,
        RpcMethodNotFound = 3002,
        RpcInvalidParams = 3003,
        RpcInternalError = 3004,
        RpcParseError = 3005,
        RpcServerError = 3006,
        RpcTimeout = 3007,
        RpcConnectionFailed = 3008,

        // Wallet Errors (4000-4999)
        WalletError = 4000,
        WalletNotConnected = 4001,
        WalletConnectionFailed = 4002,
        WalletDisconnected = 4003,
        WalletNotFound = 4004,
        WalletRejected = 4005,
        WalletTimeout = 4006,
        InvalidWalletType = 4007,

        // Transaction Errors (5000-5999)
        TransactionError = 5000,
        TransactionFailed = 5001,
        TransactionRejected = 5002,
        TransactionReverted = 5003,
        TransactionNotFound = 5004,
        TransactionTimeout = 5005,
        InvalidTransaction = 5006,
        InsufficientFee = 5007,
        InvalidNonce = 5008,
        InvalidSignature = 5009,

        // Contract Errors (6000-6999)
        ContractError = 6000,
        ContractNotFound = 6001,
        ContractCallFailed = 6002,
        InvalidContractAddress = 6003,
        InvalidEntryPoint = 6004,
        ContractExecutionFailed = 6005,
        InvalidCallData = 6006,

        // Account Errors (7000-7999)
        AccountError = 7000,
        AccountNotFound = 7001,
        InvalidAccountAddress = 7002,
        InvalidPrivateKey = 7003,
        AccountNotInitialized = 7004,
        InsufficientBalance = 7005,

        // Cryptographic Errors (8000-8999)
        CryptographicError = 8000,
        InvalidSignature = 8001,
        SignatureGenerationFailed = 8002,
        InvalidPublicKey = 8003,
        InvalidPrivateKey = 8004,
        HashCalculationFailed = 8005,
        PedersenHashError = 8006,

        // Platform Errors (9000-9999)
        PlatformError = 9000,
        PlatformNotSupported = 9001,
        PlatformInitializationFailed = 9002,
        PlatformConnectionFailed = 9003,
        InvalidPlatformType = 9004,

        // Configuration Errors (10000-10999)
        ConfigurationError = 10000,
        InvalidConfiguration = 10001,
        MissingConfiguration = 10002,
        InvalidRpcUrl = 10003,
        InvalidChainId = 10004,

        // Validation Errors (11000-11999)
        ValidationError = 11000,
        InvalidAddress = 11001,
        InvalidHexString = 11002,
        InvalidBigInteger = 11003,
        InvalidCairoVersion = 11004,
        InvalidFunctionSelector = 11005,

        // Serialization Errors (12000-12999)
        SerializationError = 12000,
        DeserializationError = 12001,
        InvalidJson = 12002,
        JsonParseError = 12003
    }
}

