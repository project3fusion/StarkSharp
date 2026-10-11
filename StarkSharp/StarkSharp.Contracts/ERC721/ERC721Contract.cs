using System;
using System.Numerics;
using System.Threading.Tasks;
using StarkSharp.Connectors.Components;
using StarkSharp.Rpc.Utils;

namespace StarkSharp.Contracts
{
    /// <summary>
    /// High-level client and interaction builder for standard ERC-721 (NFT) contracts on Starknet.
    /// Combines read (view) calls and write (invoke) call builders into a single, cohesive API.
    /// </summary>
    public class ERC721Contract : StarknetContract
    {
        public ERC721Contract(string address, string? rpcUrl = null) : base(address, rpcUrl) { }

        #region Read (View) Operations

        /// <summary>
        /// Queries the number of NFTs owned by the specified address.
        /// </summary>
        public async Task<BigInteger> GetBalanceAsync(string ownerAddress)
        {
            var res = await CallAsync(ERC721Function.BalanceOf, ownerAddress);
            return Uint256.FromResult(res).Value;
        }

        /// <summary>
        /// Queries the owner address of a specific NFT token ID.
        /// </summary>
        public async Task<string> GetOwnerOfAsync(BigInteger tokenId)
        {
            var (low, high) = Uint256.ToFelts(tokenId);
            var res = await CallAsync(ERC721Function.OwnerOf, low, high);
            return res.Length > 0 ? res[0] : string.Empty;
        }

        /// <summary>
        /// Queries the uniform resource identifier (URI) for a token's metadata.
        /// </summary>
        public async Task<string> GetTokenUriAsync(BigInteger tokenId)
        {
            var (low, high) = Uint256.ToFelts(tokenId);
            var res = await CallAsync(ERC721Function.TokenUri, low, high);
            return Felt.DecodeCairoString(res);
        }

        /// <summary>
        /// Queries the collection name.
        /// </summary>
        public async Task<string> GetNameAsync()
        {
            var res = await CallAsync(ERC721Function.Name);
            return Felt.DecodeCairoString(res);
        }

        /// <summary>
        /// Queries the collection symbol.
        /// </summary>
        public async Task<string> GetSymbolAsync()
        {
            var res = await CallAsync(ERC721Function.Symbol);
            return Felt.DecodeCairoString(res);
        }

        /// <summary>
        /// Queries the approved address for a specific token ID (or zero if none).
        /// </summary>
        public async Task<string> GetApprovedAsync(BigInteger tokenId)
        {
            var (low, high) = Uint256.ToFelts(tokenId);
            var res = await CallAsync(ERC721Function.GetApproved, low, high);
            return res.Length > 0 ? res[0] : string.Empty;
        }

        /// <summary>
        /// Queries whether an operator is approved to manage all assets of an owner.
        /// </summary>
        public async Task<bool> IsApprovedForAllAsync(string ownerAddress, string operatorAddress)
        {
            var res = await CallAsync(ERC721Function.IsApprovedForAll, ownerAddress, operatorAddress);
            return res.Length > 0 && Felt.ToBigInteger(res[0]) != BigInteger.Zero;
        }

        #endregion

        #region Write (Invoke) Call Builders

        /// <summary>
        /// Creates a <see cref="ContractInteraction"/> to transfer an NFT from one address to another.
        /// </summary>
        public ContractInteraction CreateTransferFrom(string from, string to, BigInteger tokenId)
        {
            var (low, high) = Uint256.ToFelts(tokenId);
            return new ContractInteraction(Address, ERC721Function.TransferFrom, new string[] { from, to, low, high });
        }

        /// <summary>
        /// Creates a <see cref="ContractInteraction"/> to safely transfer an NFT from one address to another.
        /// </summary>
        public ContractInteraction CreateSafeTransferFrom(string from, string to, BigInteger tokenId)
        {
            var (low, high) = Uint256.ToFelts(tokenId);
            return new ContractInteraction(Address, ERC721Function.SafeTransferFrom, new string[] { from, to, low, high, "0x0" });
        }

        /// <summary>
        /// Creates a <see cref="ContractInteraction"/> to approve an address to operate on the given token ID.
        /// </summary>
        public ContractInteraction CreateApprove(string approvedAddress, BigInteger tokenId)
        {
            var (low, high) = Uint256.ToFelts(tokenId);
            return new ContractInteraction(Address, ERC721Function.Approve, new string[] { approvedAddress, low, high });
        }

        /// <summary>
        /// Creates a <see cref="ContractInteraction"/> to approve or revoke an operator to manage all tokens.
        /// </summary>
        public ContractInteraction CreateSetApprovalForAll(string operatorAddress, bool approved)
        {
            return new ContractInteraction(Address, ERC721Function.SetApprovalForAll, new string[] { operatorAddress, approved ? "0x1" : "0x0" });
        }

        #endregion
    }
}
