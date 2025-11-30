using System;
using System.Threading.Tasks;
using StarkSharp.Accounts;
using StarkSharp.Connectors.Components;
using StarkSharp.Core.Interfaces;
using StarkSharp.Rpc;

namespace StarkSharp.Core.Connector
{
    /// <summary>
    /// Modern connector implementation using service pattern
    /// </summary>
    public class StarkSharpConnector : IConnector
    {
        private readonly IWalletService _walletService;
        private readonly ITransactionService _transactionService;
        private readonly IContractService _contractService;
        private readonly IAccountService _accountService;
        private readonly ILoggingService _loggingService;

        public Account Account { get; private set; }
        public WalletType WalletType { get; private set; }
        public PlatformConnectorType ConnectorType { get; }

        public StarkSharpConnector(
            IWalletService walletService,
            ITransactionService transactionService,
            IContractService contractService,
            IAccountService accountService,
            ILoggingService loggingService,
            IStarkSharpConfiguration config)
        {
            _walletService = walletService;
            _transactionService = transactionService;
            _contractService = contractService;
            _accountService = accountService;
            _loggingService = loggingService;
            ConnectorType = config.ConnectorType;
            Account = new Account();
        }

        public async void ConnectWallet(WalletType walletType, Action<string> successCallback, Action<string> failCallback)
        {
            try
            {
                var connected = await _walletService.ConnectWalletAsync(walletType);
                if (connected)
                {
                    Account = await _walletService.GetAccountAsync();
                    WalletType = walletType;
                    successCallback?.Invoke(walletType.ToString());
                }
                else
                {
                    failCallback?.Invoke("Failed to connect wallet");
                }
            }
            catch (Exception ex)
            {
                _loggingService?.LogError($"Error connecting wallet: {ex.Message}");
                failCallback?.Invoke(ex.Message);
            }
        }

        public async void SendTransaction(TransactionInteraction transactionInteraction, Action<JsonRpcResponse> successCallback, Action<JsonRpcResponse> failCallback)
        {
            try
            {
                var response = await _transactionService.SendTransactionAsync(transactionInteraction);
                if (response.error != null)
                {
                    failCallback?.Invoke(response);
                }
                else
                {
                    successCallback?.Invoke(response);
                }
            }
            catch (Exception ex)
            {
                _loggingService?.LogError($"Error sending transaction: {ex.Message}");
                failCallback?.Invoke(new JsonRpcResponse
                {
                    error = new JsonRpcError { code = -1, message = ex.Message }
                });
            }
        }

        public async void CallContract(ContractInteraction contractInteraction, Action<string> successCallback, Action<string> failCallback)
        {
            try
            {
                var result = await _contractService.CallContractAsync(contractInteraction);
                successCallback?.Invoke(result);
            }
            catch (Exception ex)
            {
                _loggingService?.LogError($"Error calling contract: {ex.Message}");
                failCallback?.Invoke(ex.Message);
            }
        }

        public void DebugMessage(string message)
        {
            _loggingService?.LogInfo(message);
        }
    }
}

