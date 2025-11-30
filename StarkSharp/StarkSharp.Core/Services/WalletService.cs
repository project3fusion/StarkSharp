using System;
using System.Threading.Tasks;
using StarkSharp.Accounts;
using StarkSharp.Connectors.Components;
using StarkSharp.Core.Interfaces;

namespace StarkSharp.Core.Services
{
    /// <summary>
    /// Wallet service implementation
    /// </summary>
    public class WalletService : IWalletService
    {
        private readonly IPlatform _platform;
        private readonly ILoggingService _loggingService;
        private Account _account;
        private WalletType? _connectedWalletType;
        private bool _isConnected;

        public WalletService(IPlatform platform, ILoggingService loggingService)
        {
            _platform = platform;
            _loggingService = loggingService;
            _account = new Account();
        }

        public async Task<bool> ConnectWalletAsync(WalletType walletType)
        {
            var tcs = new TaskCompletionSource<bool>();
            var taskId = StarkSharp.Connectors.Components.ConnectorTask.CreateNewTask();

            _platform.ConnectWallet(walletType.ToString(), taskId);

            _platform.WaitUntil(
                taskId,
                message =>
                {
                    if (message == walletType.ToString())
                    {
                        _connectedWalletType = walletType;
                        _account.WalletAdress = _platform.GetAccountInformation();
                        _isConnected = true;
                        _loggingService?.LogSuccess($"Wallet {walletType} connected successfully");
                        tcs.SetResult(true);
                    }
                    else
                    {
                        tcs.SetResult(false);
                    }
                },
                error =>
                {
                    _loggingService?.LogError($"Failed to connect wallet: {error}");
                    tcs.SetResult(false);
                },
                () => _platform.CheckWalletConnection(),
                (id, success, fail) => { }
            );

            return await tcs.Task;
        }

        public async Task<bool> DisconnectWalletAsync()
        {
            await Task.CompletedTask;
            _isConnected = false;
            _connectedWalletType = null;
            _account = new Account();
            _loggingService?.LogInfo("Wallet disconnected");
            return true;
        }

        public async Task<bool> IsWalletConnectedAsync()
        {
            await Task.CompletedTask;
            return _isConnected && _platform.CheckWalletConnection();
        }

        public async Task<Account> GetAccountAsync()
        {
            await Task.CompletedTask;
            if (_isConnected && _account.WalletAdress == null)
            {
                _account.WalletAdress = _platform.GetAccountInformation();
            }
            return _account;
        }

        public async Task<string> GetAccountAddressAsync()
        {
            var account = await GetAccountAsync();
            return account?.WalletAdress ?? string.Empty;
        }

        public WalletType? GetConnectedWalletType()
        {
            return _connectedWalletType;
        }
    }
}

