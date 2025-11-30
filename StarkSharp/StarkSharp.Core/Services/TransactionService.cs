using System;
using System.Threading;
using System.Threading.Tasks;
using StarkSharp.Connectors.Components;
using StarkSharp.Core.Interfaces;
using StarkSharp.Rpc;

namespace StarkSharp.Core.Services
{
    /// <summary>
    /// Transaction service implementation
    /// </summary>
    public class TransactionService : ITransactionService
    {
        private readonly IPlatform _platform;
        private readonly IRpcService _rpcService;
        private readonly ILoggingService _loggingService;
        private readonly IStarkSharpConfiguration _config;

        public TransactionService(
            IPlatform platform,
            IRpcService rpcService,
            ILoggingService loggingService,
            IStarkSharpConfiguration config)
        {
            _platform = platform;
            _rpcService = rpcService;
            _loggingService = loggingService;
            _config = config;
        }

        public async Task<JsonRpcResponse> SendTransactionAsync(TransactionInteraction transactionInteraction)
        {
            var tcs = new TaskCompletionSource<JsonRpcResponse>();

            _platform.SendTransaction(
                transactionInteraction,
                response =>
                {
                    _loggingService?.LogSuccess("Transaction sent successfully");
                    tcs.SetResult(response);
                },
                error =>
                {
                    _loggingService?.LogError($"Transaction failed: {error.error?.message ?? "Unknown error"}");
                    tcs.SetResult(error);
                }
            );

            return await tcs.Task;
        }

        public async Task<JsonRpcResponse> EstimateFeeAsync(TransactionInteraction transactionInteraction)
        {
            // TODO: Implement fee estimation
            await Task.CompletedTask;
            throw new NotImplementedException("Fee estimation not yet implemented");
        }

        public async Task<JsonRpcResponse> GetTransactionReceiptAsync(string transactionHash)
        {
            var request = _rpcService.CreateRequest("starknet_getTransactionReceipt", new object[] { transactionHash });
            return await _rpcService.SendRequestAsync(request);
        }

        public async Task<JsonRpcResponse> WaitForTransactionAsync(string transactionHash, int maxRetries = 500, float checkInterval = 2f)
        {
            maxRetries = maxRetries > 0 ? maxRetries : _config.DefaultMaxRetries;
            checkInterval = checkInterval > 0 ? checkInterval : _config.DefaultCheckInterval;

            for (int i = 0; i < maxRetries; i++)
            {
                var receipt = await GetTransactionReceiptAsync(transactionHash);

                if (receipt.error == null && receipt.result != null)
                {
                    _loggingService?.LogSuccess($"Transaction confirmed: {transactionHash}");
                    return receipt;
                }

                if (receipt.error != null && receipt.error.message != "Transaction hash not found")
                {
                    _loggingService?.LogError($"Transaction error: {receipt.error.message}");
                    return receipt;
                }

                await Task.Delay((int)(checkInterval * 1000));
            }

            _loggingService?.LogError($"Transaction not received within {maxRetries} retries");
            throw new TimeoutException($"Transaction {transactionHash} not received within the given retries.");
        }
    }
}

