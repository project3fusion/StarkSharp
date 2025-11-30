using System;
using System.Threading.Tasks;
using StarkSharp.Connectors.Components;
using StarkSharp.Core.Interfaces;
using StarkSharp.Rpc;

namespace StarkSharp.Core.Services
{
    /// <summary>
    /// Blockchain service implementation
    /// </summary>
    public class BlockchainService : IBlockchainService
    {
        private readonly IRpcService _rpcService;
        private readonly ILoggingService _loggingService;

        public BlockchainService(IRpcService rpcService, ILoggingService loggingService)
        {
            _rpcService = rpcService;
            _loggingService = loggingService;
        }

        public async Task<string> QueryAsync(QueryInteraction queryInteraction)
        {
            try
            {
                var request = JsonRpcHandler.GenerateQueryRequestData(queryInteraction);
                var response = await _rpcService.SendRequestAsync(request);

                if (response.error != null)
                {
                    _loggingService?.LogError($"Query error: {response.error.message}");
                    throw new Exception($"Query error: {response.error.message}");
                }

                return response.result?.ToString() ?? string.Empty;
            }
            catch (Exception ex)
            {
                _loggingService?.LogError($"Query failed: {ex.Message}");
                throw;
            }
        }

        public async Task<T> QueryAsync<T>(QueryInteraction queryInteraction)
        {
            var result = await QueryAsync(queryInteraction);
            
            try
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(result);
            }
            catch (Exception ex)
            {
                _loggingService?.LogError($"Failed to deserialize query result: {ex.Message}");
                throw;
            }
        }
    }
}

