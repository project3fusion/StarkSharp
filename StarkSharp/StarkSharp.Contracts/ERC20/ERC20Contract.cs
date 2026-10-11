using System;
using System.Numerics;
using System.Threading.Tasks;
using StarkSharp.Connectors.Components;
using StarkSharp.Rpc.Utils;

namespace StarkSharp.Contracts
{
    /// <summary>
    /// High-level client and interaction builder for standard ERC-20 token contracts on Starknet.
    /// Combines read (view) calls and write (invoke) call builders into a single, cohesive API.
    /// </summary>
    public class ERC20Contract : StarknetContract
    {
        public ERC20Contract(string address, string? rpcUrl = null) : base(address, rpcUrl) { }

        #region Read (View) Operations

        /// <summary>
        /// Queries the token balance for the specified owner address.
        /// Returns a <see cref="BigInteger"/> representing the 256-bit token balance.
        /// </summary>
        public async Task<BigInteger> GetBalanceAsync(string ownerAddress)
        {
            var res = await CallAsync(ERC20Function.BalanceOf, ownerAddress);
            return Uint256.FromResult(res).Value;
        }

        /// <summary>
        /// Queries the total supply of the token.
        /// </summary>
        public async Task<BigInteger> GetTotalSupplyAsync()
        {
            var res = await CallAsync(ERC20Function.TotalSupply);
            return Uint256.FromResult(res).Value;
        }

        /// <summary>
        /// Queries the remaining token allowance granted by an owner to a spender.
        /// </summary>
        public async Task<BigInteger> GetAllowanceAsync(string ownerAddress, string spenderAddress)
        {
            var res = await CallAsync(ERC20Function.Allowance, ownerAddress, spenderAddress);
            return Uint256.FromResult(res).Value;
        }

        /// <summary>
        /// Queries the human-readable token name (e.g. "Ethereum", "USD Coin").
        /// Automatically decodes Cairo 0 short strings and Cairo 1 ByteArray.
        /// </summary>
        public async Task<string> GetNameAsync()
        {
            var res = await CallAsync(ERC20Function.Name);
            return Felt.DecodeCairoString(res);
        }

        /// <summary>
        /// Queries the token symbol (e.g. "ETH", "USDC", "STRK").
        /// </summary>
        public async Task<string> GetSymbolAsync()
        {
            var res = await CallAsync(ERC20Function.Symbol);
            return Felt.DecodeCairoString(res);
        }

        /// <summary>
        /// Queries the number of decimals used by the token (e.g. 18).
        /// </summary>
        public async Task<int> GetDecimalsAsync()
        {
            var res = await CallAsync(ERC20Function.Decimals);
            return res.Length > 0 ? (int)Felt.ToBigInteger(res[0]) : 18;
        }

        #endregion

        #region Write (Invoke) Call Builders

        /// <summary>
        /// Creates a <see cref="ContractInteraction"/> to transfer tokens to a recipient.
        /// </summary>
        public ContractInteraction CreateTransfer(string recipientAddress, BigInteger amount)
        {
            var (low, high) = Uint256.ToFelts(amount);
            return new ContractInteraction(Address, ERC20Function.Transfer, new string[] { recipientAddress, low, high });
        }

        /// <summary>
        /// Creates a <see cref="ContractInteraction"/> to transfer tokens on behalf of a sender to a recipient.
        /// </summary>
        public ContractInteraction CreateTransferFrom(string senderAddress, string recipientAddress, BigInteger amount)
        {
            var (low, high) = Uint256.ToFelts(amount);
            return new ContractInteraction(Address, ERC20Function.TransferFrom, new string[] { senderAddress, recipientAddress, low, high });
        }

        /// <summary>
        /// Creates a <see cref="ContractInteraction"/> to approve a spender to transfer up to the specified amount.
        /// </summary>
        public ContractInteraction CreateApprove(string spenderAddress, BigInteger amount)
        {
            var (low, high) = Uint256.ToFelts(amount);
            return new ContractInteraction(Address, ERC20Function.Approve, new string[] { spenderAddress, low, high });
        }

        #endregion

        #region Static Convenience Methods

        /// <summary>
        /// Queries the token balance in a single line.
        /// </summary>
        public static Task<BigInteger> QueryBalanceAsync(string contractAddress, string ownerAddress, string? rpcUrl = null) =>
            new ERC20Contract(contractAddress, rpcUrl).GetBalanceAsync(ownerAddress);

        /// <summary>
        /// Queries the token symbol in a single line.
        /// </summary>
        public static Task<string> QuerySymbolAsync(string contractAddress, string? rpcUrl = null) =>
            new ERC20Contract(contractAddress, rpcUrl).GetSymbolAsync();

        /// <summary>
        /// Queries the token name in a single line.
        /// </summary>
        public static Task<string> QueryNameAsync(string contractAddress, string? rpcUrl = null) =>
            new ERC20Contract(contractAddress, rpcUrl).GetNameAsync();

        /// <summary>
        /// Queries the token decimals in a single line.
        /// </summary>
        public static Task<int> QueryDecimalsAsync(string contractAddress, string? rpcUrl = null) =>
            new ERC20Contract(contractAddress, rpcUrl).GetDecimalsAsync();

        /// <summary>
        /// Queries the total supply in a single line.
        /// </summary>
        public static Task<BigInteger> QueryTotalSupplyAsync(string contractAddress, string? rpcUrl = null) =>
            new ERC20Contract(contractAddress, rpcUrl).GetTotalSupplyAsync();

        #endregion
    }
}
