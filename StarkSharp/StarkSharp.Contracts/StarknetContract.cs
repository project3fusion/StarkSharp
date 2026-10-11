using System;
using System.Threading.Tasks;
using StarkSharp.Api.RPC;
using StarkSharp.Rpc;
using StarkSharp.Rpc.Utils;

namespace StarkSharp.Contracts
{
    /// <summary>
    /// Base class representing a deployed Starknet smart contract.
    /// Provides low-level read (view) calls and selector routing.
    /// </summary>
    public class StarknetContract
    {
        /// <summary>
        /// The deployed Starknet contract address (0x...).
        /// </summary>
        public string Address { get; }

        /// <summary>
        /// Optional specific RPC URL for this contract. If null, the global default RPC URL is used.
        /// </summary>
        public string? RpcUrl { get; }

        public StarknetContract(string address, string? rpcUrl = null)
        {
            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentException("Contract address cannot be empty.", nameof(address));

            Address = address;
            RpcUrl = rpcUrl;
        }

        /// <summary>
        /// Performs a low-level starknet_call (view call) and returns the raw <see cref="JsonRpcResponse"/>.
        /// </summary>
        public Task<JsonRpcResponse> CallRawAsync(string entryPointSelector, params string[] calldata)
        {
            return JsonRpcRequestHandler.Call(RpcUrl, Address, entryPointSelector, calldata);
        }

        /// <summary>
        /// Performs a starknet_call (view call) and returns the resulting felt array.
        /// Throws an exception if the RPC returns an error.
        /// </summary>
        public async Task<string[]> CallAsync(string entryPointSelector, params string[] calldata)
        {
            var response = await CallRawAsync(entryPointSelector, calldata);
            if (response.HasError)
            {
                throw new InvalidOperationException(
                    $"Starknet call to {Address} ({entryPointSelector}) failed: {response.error?.message} (code {response.error?.code})");
            }

            return response.ResultAsArray ?? Array.Empty<string>();
        }

        public Task<string[]> CallAsync(ContractFunction function, params string[] calldata) =>
            CallAsync(function.ToSelector(), calldata);

        public Task<string[]> CallAsync(ERC20Function function, params string[] calldata) =>
            CallAsync(function.ToSelector(), calldata);

        public Task<string[]> CallAsync(ERC721Function function, params string[] calldata) =>
            CallAsync(function.ToSelector(), calldata);

        public Task<string[]> CallAsync(AccountFunction function, params string[] calldata) =>
            CallAsync(function.ToSelector(), calldata);
    }
}
