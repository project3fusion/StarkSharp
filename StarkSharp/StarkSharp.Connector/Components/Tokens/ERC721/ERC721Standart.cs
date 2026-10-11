using System.Numerics;
using StarkSharp.Accounts;
using StarkSharp.Connectors.Components;
using StarkSharp.Rpc.Utils;

namespace StarkSharp.Components.Token.ERC721
{
    public class ERC721Standart : ERCStandart
    {
        public static ContractInteraction BalanceOf(string contractAddress, Account account) =>
            GenerateStandartData(contractAddress, ERC721Function.BalanceOf, new string[] { account.WalletAdress });

        public static ContractInteraction BalanceOf(string contractAddress, string walletAddress) =>
            GenerateStandartData(contractAddress, ERC721Function.BalanceOf, new string[] { walletAddress });

        public static ContractInteraction OwnerOf(string contractAddress, string tokenId) =>
            GenerateStandartData(contractAddress, ERC721Function.OwnerOf, new string[] { tokenId, "0x00" });

        public static ContractInteraction OwnerOf(string contractAddress, BigInteger tokenId)
        {
            var (low, high) = Uint256.ToFelts(tokenId);
            return GenerateStandartData(contractAddress, ERC721Function.OwnerOf, new string[] { low, high });
        }

        public static ContractInteraction TransferToken(string contractAddress, string recipientAddress, string tokenId) =>
            GenerateStandartData(contractAddress, ERC721Function.TransferFrom, new string[] { recipientAddress, tokenId, "0x00" });

        public static ContractInteraction TransferToken(string contractAddress, string recipientAddress, BigInteger tokenId)
        {
            var (low, high) = Uint256.ToFelts(tokenId);
            return GenerateStandartData(contractAddress, ERC721Function.TransferFrom, new string[] { recipientAddress, low, high });
        }

        public static ContractInteraction SafeTransferFrom(string contractAddress, string from, string to, string tokenId) =>
            GenerateStandartData(contractAddress, ERC721Function.SafeTransferFrom, new string[] { from, to, tokenId, "0x00" });

        public static ContractInteraction SafeTransferFrom(string contractAddress, string from, string to, BigInteger tokenId)
        {
            var (low, high) = Uint256.ToFelts(tokenId);
            return GenerateStandartData(contractAddress, ERC721Function.SafeTransferFrom, new string[] { from, to, low, high, "0x00" });
        }

        public static ContractInteraction Approve(string contractAddress, string approvedAddress, string tokenId) =>
            GenerateStandartData(contractAddress, ERC721Function.Approve, new string[] { approvedAddress, tokenId, "0x00" });

        public static ContractInteraction Approve(string contractAddress, string approvedAddress, BigInteger tokenId)
        {
            var (low, high) = Uint256.ToFelts(tokenId);
            return GenerateStandartData(contractAddress, ERC721Function.Approve, new string[] { approvedAddress, low, high });
        }

        public static ContractInteraction SetApprovalForAll(string contractAddress, string @operator, bool approved) =>
            GenerateStandartData(contractAddress, ERC721Function.SetApprovalForAll, new string[] { @operator, approved ? "0x1" : "0x0" });

        public static ContractInteraction GetApproved(string contractAddress, string tokenId) =>
            GenerateStandartData(contractAddress, ERC721Function.GetApproved, new string[] { tokenId, "0x00" });

        public static ContractInteraction IsApprovedForAll(string contractAddress, string owner, string @operator) =>
            GenerateStandartData(contractAddress, ERC721Function.IsApprovedForAll, new string[] { owner, @operator });

        public static ContractInteraction TokenUri(string contractAddress, string tokenId) =>
            GenerateStandartData(contractAddress, ERC721Function.TokenUri, new string[] { tokenId, "0x00" });
    }
}
