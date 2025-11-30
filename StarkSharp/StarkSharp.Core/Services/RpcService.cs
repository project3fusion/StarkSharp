using System;
using System.Threading.Tasks;
using StarkSharp.Core.Interfaces;
using StarkSharp.Rpc;

namespace StarkSharp.Core.Services
{
    /// <summary>
    /// RPC service implementation
    /// </summary>
    public class RpcService : IRpcService
    {
        private readonly IPlatform _platform;
        private readonly ILoggingService _loggingService;

        public RpcService(IPlatform platform, ILoggingService loggingService)
        {
            _platform = platform;
            _loggingService = loggingService;
        }

        public async Task<JsonRpcResponse> SendRequestAsync(JsonRpc request)
        {
            var tcs = new TaskCompletionSource<JsonRpcResponse>();

            _platform.PlatformRequest(request, response =>
            {
                tcs.SetResult(response);
            });

            return await tcs.Task;
        }

        public async Task<T> SendRequestAsync<T>(JsonRpc request)
        {
            var response = await SendRequestAsync(request);
            if (response.error != null)
            {
                _loggingService?.LogError($"RPC Error: {response.error.message}");
                throw new Exception($"RPC Error: {response.error.message}");
            }

            if (response.result is T result)
            {
                return result;
            }

            throw new InvalidCastException($"Cannot convert result to {typeof(T).Name}");
        }

        public JsonRpc CreateRequest(string method, object[] parameters)
        {
            return JsonRpcHandler.GenerateRequestData(method, parameters);
        }

        public JsonRpc CreateContractRequest(string contractAddress, string entryPointSelector, string serializedData)
        {
            return JsonRpcHandler.GenerateContractRequestData(contractAddress, entryPointSelector, serializedData);
        }

        public JsonRpc CreateTransactionRequest(string senderAddress, string serializedData, string maxFee, string nonce, string[] signature, string type, string version)
        {
            return JsonRpcHandler.GenerateTransactionRequestData(senderAddress, serializedData, maxFee, nonce, signature, type, version);
        }
    }
}

