using StarkSharp.Accounts;
using System.Collections.Generic;

namespace StarkSharp.RPC.Token.ERC20
{
    public class ERC20Standart : ERCStandart
    {
        public static List<string> BalanceOf(string contractAddress, Account account) => 
            GenerateStandartData(contractAddress, "balance_of", new string[] { account.WalletAdress});

        public static List<string> BalanceOf(string contractAddress, string walletAddress) => 
            GenerateStandartData(contractAddress, "balance_of", new string[] { walletAddress });

        public static List<string> TransferToken(string contractAddress, string recipientAddress, string amount) => 
            GenerateStandartData(contractAddress, "transfer", new string[] { recipientAddress, amount, "0x00" });
    }
}
