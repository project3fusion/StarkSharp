using System;
using System.Threading.Tasks;
using StarkSharp.Rpc;

namespace StarkSharp.Core.Interfaces
{
    /// <summary>
    /// Service interface for RPC operations
    /// </summary>
    public interface IRpcService
    {
        Task<JsonRpcResponse> SendRequestAsync(JsonRpc request);
        Task<T> SendRequestAsync<T>(JsonRpc request);
        JsonRpc CreateRequest(string method, object[] parameters);
        JsonRpc CreateContractRequest(string contractAddress, string entryPointSelector, string serializedData);
        JsonRpc CreateTransactionRequest(string senderAddress, string serializedData, string maxFee, string nonce, string[] signature, string type, string version);
    }
}

