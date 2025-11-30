using System.Threading.Tasks;
using StarkSharp.Accounts;
using StarkSharp.Core.Interfaces;

namespace StarkSharp.Core.Services
{
    /// <summary>
    /// Account service implementation
    /// </summary>
    public class AccountService : IAccountService
    {
        private readonly IWalletService _walletService;
        private readonly IRpcService _rpcService;
        private Account _currentAccount;

        public AccountService(IWalletService walletService, IRpcService rpcService)
        {
            _walletService = walletService;
            _rpcService = rpcService;
        }

        public async Task<Account> GetAccountAsync()
        {
            if (_currentAccount == null)
            {
                _currentAccount = await _walletService.GetAccountAsync();
            }
            return _currentAccount;
        }

        public async Task<string> GetAccountAddressAsync()
        {
            var account = await GetAccountAsync();
            return account?.WalletAdress ?? string.Empty;
        }

        public async Task<float> GetBalanceAsync(string address)
        {
            // TODO: Implement balance query via RPC
            await Task.CompletedTask;
            return 0f;
        }

        public async Task<string> GetNonceAsync(string address)
        {
            var request = _rpcService.CreateRequest("starknet_getNonce", new object[] { "latest", address });
            var response = await _rpcService.SendRequestAsync(request);
            
            if (response.error != null)
            {
                throw new System.Exception($"Error getting nonce: {response.error.message}");
            }

            return response.result?.ToString() ?? "0x0";
        }
    }
}

