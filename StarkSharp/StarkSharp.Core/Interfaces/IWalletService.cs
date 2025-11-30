using System;
using System.Threading.Tasks;
using StarkSharp.Accounts;
using StarkSharp.Connectors.Components;

namespace StarkSharp.Core.Interfaces
{
    /// <summary>
    /// Service interface for wallet operations
    /// </summary>
    public interface IWalletService
    {
        Task<bool> ConnectWalletAsync(WalletType walletType);
        Task<bool> DisconnectWalletAsync();
        Task<bool> IsWalletConnectedAsync();
        Task<Account> GetAccountAsync();
        Task<string> GetAccountAddressAsync();
        WalletType? GetConnectedWalletType();
    }
}

