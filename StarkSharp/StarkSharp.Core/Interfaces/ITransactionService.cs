using System;
using System.Threading.Tasks;
using StarkSharp.Connectors.Components;
using StarkSharp.Rpc;

namespace StarkSharp.Core.Interfaces
{
    /// <summary>
    /// Service interface for transaction operations
    /// </summary>
    public interface ITransactionService
    {
        Task<JsonRpcResponse> SendTransactionAsync(TransactionInteraction transactionInteraction);
        Task<JsonRpcResponse> EstimateFeeAsync(TransactionInteraction transactionInteraction);
        Task<JsonRpcResponse> GetTransactionReceiptAsync(string transactionHash);
        Task<JsonRpcResponse> WaitForTransactionAsync(string transactionHash, int maxRetries = 500, float checkInterval = 2f);
    }
}

