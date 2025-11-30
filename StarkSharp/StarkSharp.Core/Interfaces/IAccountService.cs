using System.Threading.Tasks;
using StarkSharp.Accounts;

namespace StarkSharp.Core.Interfaces
{
    /// <summary>
    /// Service interface for account operations
    /// </summary>
    public interface IAccountService
    {
        Task<Account> GetAccountAsync();
        Task<string> GetAccountAddressAsync();
        Task<float> GetBalanceAsync(string address);
        Task<string> GetNonceAsync(string address);
    }
}

