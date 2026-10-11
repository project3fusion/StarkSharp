using System.Numerics;
using StarkSharp.Accounts;
using StarkSharp.Connectors.Components;
using StarkSharp.Rpc.Utils;

namespace StarkSharp.Components.Token.ERC20
{
    public class ERC20Standart : ERCStandart
    {
        public static ContractInteraction BalanceOf(string contractAddress, Account account) => 
            GenerateStandartData(contractAddress, ERC20Function.BalanceOf, new string[] { account.WalletAdress });

        public static ContractInteraction BalanceOf(string contractAddress, string walletAddress) => 
            GenerateStandartData(contractAddress, ERC20Function.BalanceOf, new string[] { walletAddress });

        public static ContractInteraction TransferToken(string contractAddress, string recipientAddress, string amount) => 
            GenerateStandartData(contractAddress, ERC20Function.Transfer, new string[] { recipientAddress, amount, "0x00" });

        public static ContractInteraction TransferToken(string contractAddress, string recipientAddress, BigInteger amount)
        {
            var (low, high) = Uint256.ToFelts(amount);
            return GenerateStandartData(contractAddress, ERC20Function.Transfer, new string[] { recipientAddress, low, high });
        }

        public static ContractInteraction TransferFrom(string contractAddress, string senderAddress, string recipientAddress, string amount) => 
            GenerateStandartData(contractAddress, ERC20Function.TransferFrom, new string[] { senderAddress, recipientAddress, amount, "0x00" });

        public static ContractInteraction TransferFrom(string contractAddress, string senderAddress, string recipientAddress, BigInteger amount)
        {
            var (low, high) = Uint256.ToFelts(amount);
            return GenerateStandartData(contractAddress, ERC20Function.TransferFrom, new string[] { senderAddress, recipientAddress, low, high });
        }

        public static ContractInteraction Approve(string contractAddress, string spenderAddress, string amount) => 
            GenerateStandartData(contractAddress, ERC20Function.Approve, new string[] { spenderAddress, amount, "0x00" });

        public static ContractInteraction Approve(string contractAddress, string spenderAddress, BigInteger amount)
        {
            var (low, high) = Uint256.ToFelts(amount);
            return GenerateStandartData(contractAddress, ERC20Function.Approve, new string[] { spenderAddress, low, high });
        }

        public static ContractInteraction Allowance(string contractAddress, string ownerAddress, string spenderAddress) => 
            GenerateStandartData(contractAddress, ERC20Function.Allowance, new string[] { ownerAddress, spenderAddress });

        public static ContractInteraction TotalSupply(string contractAddress) => 
            GenerateStandartData(contractAddress, ERC20Function.TotalSupply, new string[0]);

        public static ContractInteraction Name(string contractAddress) => 
            GenerateStandartData(contractAddress, ERC20Function.Name, new string[0]);

        public static ContractInteraction Symbol(string contractAddress) => 
            GenerateStandartData(contractAddress, ERC20Function.Symbol, new string[0]);

        public static ContractInteraction Decimals(string contractAddress) => 
            GenerateStandartData(contractAddress, ERC20Function.Decimals, new string[0]);
    }
}
