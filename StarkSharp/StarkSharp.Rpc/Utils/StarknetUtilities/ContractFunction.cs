using System;

namespace StarkSharp.Rpc.Utils
{
    /// <summary>
    /// Common standard Starknet contract function names.
    /// </summary>
    public enum ContractFunction
    {
        // ERC20 & Common Token
        BalanceOf,
        Transfer,
        TransferFrom,
        Approve,
        Allowance,
        TotalSupply,
        Name,
        Symbol,
        Decimals,

        // ERC721 (NFT)
        OwnerOf,
        SafeTransferFrom,
        SetApprovalForAll,
        GetApproved,
        IsApprovedForAll,
        TokenUri,

        // Account & Contract Introspection
        Execute,
        Validate,
        ValidateDeclare,
        ValidateDeploy,
        GetNonce,
        GetPublicKey,
        IsValidSignature,
        SupportsInterface
    }

    /// <summary>
    /// Standard ERC-20 token contract function names.
    /// </summary>
    public enum ERC20Function
    {
        Name,
        Symbol,
        Decimals,
        TotalSupply,
        BalanceOf,
        Allowance,
        Transfer,
        TransferFrom,
        Approve
    }

    /// <summary>
    /// Standard ERC-721 (NFT) contract function names.
    /// </summary>
    public enum ERC721Function
    {
        Name,
        Symbol,
        BalanceOf,
        OwnerOf,
        SafeTransferFrom,
        TransferFrom,
        Approve,
        SetApprovalForAll,
        GetApproved,
        IsApprovedForAll,
        TokenUri
    }

    /// <summary>
    /// Standard Starknet account contract function names.
    /// </summary>
    public enum AccountFunction
    {
        Execute,
        Validate,
        ValidateDeclare,
        ValidateDeploy,
        GetNonce,
        GetPublicKey,
        IsValidSignature,
        SupportsInterface
    }

    /// <summary>
    /// Extension methods for contract function enums to retrieve function names and Starknet selectors.
    /// </summary>
    public static class ContractFunctionExtensions
    {
        /// <summary>
        /// Converts <see cref="ContractFunction"/> to its exact Starknet contract function entrypoint name.
        /// </summary>
        public static string ToFunctionName(this ContractFunction function) => function switch
        {
            ContractFunction.BalanceOf => "balance_of",
            ContractFunction.Transfer => "transfer",
            ContractFunction.TransferFrom => "transfer_from",
            ContractFunction.Approve => "approve",
            ContractFunction.Allowance => "allowance",
            ContractFunction.TotalSupply => "total_supply",
            ContractFunction.Name => "name",
            ContractFunction.Symbol => "symbol",
            ContractFunction.Decimals => "decimals",
            ContractFunction.OwnerOf => "owner_of",
            ContractFunction.SafeTransferFrom => "safe_transfer_from",
            ContractFunction.SetApprovalForAll => "set_approval_for_all",
            ContractFunction.GetApproved => "get_approved",
            ContractFunction.IsApprovedForAll => "is_approved_for_all",
            ContractFunction.TokenUri => "token_uri",
            ContractFunction.Execute => "__execute__",
            ContractFunction.Validate => "__validate__",
            ContractFunction.ValidateDeclare => "__validate_declare__",
            ContractFunction.ValidateDeploy => "__validate_deploy__",
            ContractFunction.GetNonce => "get_nonce",
            ContractFunction.GetPublicKey => "get_public_key",
            ContractFunction.IsValidSignature => "is_valid_signature",
            ContractFunction.SupportsInterface => "supports_interface",
            _ => throw new ArgumentOutOfRangeException(nameof(function), function, "Unknown contract function")
        };

        /// <summary>
        /// Computes the Starknet function selector (starknet_keccak) for the given <see cref="ContractFunction"/>.
        /// </summary>
        public static string ToSelector(this ContractFunction function) =>
            StarknetOps.CalculateFunctionSelector(function.ToFunctionName());

        /// <summary>
        /// Converts <see cref="ERC20Function"/> to its exact Starknet function entrypoint name.
        /// </summary>
        public static string ToFunctionName(this ERC20Function function) => function switch
        {
            ERC20Function.Name => "name",
            ERC20Function.Symbol => "symbol",
            ERC20Function.Decimals => "decimals",
            ERC20Function.TotalSupply => "total_supply",
            ERC20Function.BalanceOf => "balance_of",
            ERC20Function.Allowance => "allowance",
            ERC20Function.Transfer => "transfer",
            ERC20Function.TransferFrom => "transfer_from",
            ERC20Function.Approve => "approve",
            _ => throw new ArgumentOutOfRangeException(nameof(function), function, "Unknown ERC20 function")
        };

        /// <summary>
        /// Computes the Starknet function selector for the given <see cref="ERC20Function"/>.
        /// </summary>
        public static string ToSelector(this ERC20Function function) =>
            StarknetOps.CalculateFunctionSelector(function.ToFunctionName());

        /// <summary>
        /// Converts <see cref="ERC721Function"/> to its exact Starknet function entrypoint name.
        /// </summary>
        public static string ToFunctionName(this ERC721Function function) => function switch
        {
            ERC721Function.Name => "name",
            ERC721Function.Symbol => "symbol",
            ERC721Function.BalanceOf => "balance_of",
            ERC721Function.OwnerOf => "owner_of",
            ERC721Function.SafeTransferFrom => "safe_transfer_from",
            ERC721Function.TransferFrom => "transfer_from",
            ERC721Function.Approve => "approve",
            ERC721Function.SetApprovalForAll => "set_approval_for_all",
            ERC721Function.GetApproved => "get_approved",
            ERC721Function.IsApprovedForAll => "is_approved_for_all",
            ERC721Function.TokenUri => "token_uri",
            _ => throw new ArgumentOutOfRangeException(nameof(function), function, "Unknown ERC721 function")
        };

        /// <summary>
        /// Computes the Starknet function selector for the given <see cref="ERC721Function"/>.
        /// </summary>
        public static string ToSelector(this ERC721Function function) =>
            StarknetOps.CalculateFunctionSelector(function.ToFunctionName());

        /// <summary>
        /// Converts <see cref="AccountFunction"/> to its exact Starknet function entrypoint name.
        /// </summary>
        public static string ToFunctionName(this AccountFunction function) => function switch
        {
            AccountFunction.Execute => "__execute__",
            AccountFunction.Validate => "__validate__",
            AccountFunction.ValidateDeclare => "__validate_declare__",
            AccountFunction.ValidateDeploy => "__validate_deploy__",
            AccountFunction.GetNonce => "get_nonce",
            AccountFunction.GetPublicKey => "get_public_key",
            AccountFunction.IsValidSignature => "is_valid_signature",
            AccountFunction.SupportsInterface => "supports_interface",
            _ => throw new ArgumentOutOfRangeException(nameof(function), function, "Unknown Account function")
        };

        /// <summary>
        /// Computes the Starknet function selector for the given <see cref="AccountFunction"/>.
        /// </summary>
        public static string ToSelector(this AccountFunction function) =>
            StarknetOps.CalculateFunctionSelector(function.ToFunctionName());
    }
}
