using System;
using System.Threading.Tasks;
using StarkSharp.Connectors.Components;
using StarkSharp.Core.Interfaces;

namespace StarkSharp.Core.Services
{
    /// <summary>
    /// Contract service implementation
    /// </summary>
    public class ContractService : IContractService
    {
        private readonly IPlatform _platform;
        private readonly ILoggingService _loggingService;

        public ContractService(IPlatform platform, ILoggingService loggingService)
        {
            _platform = platform;
            _loggingService = loggingService;
        }

        public async Task<string> CallContractAsync(ContractInteraction contractInteraction)
        {
            var tcs = new TaskCompletionSource<string>();

            _platform.CallContract(
                contractInteraction,
                result =>
                {
                    _loggingService?.LogSuccess("Contract call succeeded");
                    tcs.SetResult(result);
                },
                error =>
                {
                    _loggingService?.LogError($"Contract call failed: {error}");
                    tcs.SetException(new Exception($"Contract call failed: {error}"));
                }
            );

            return await tcs.Task;
        }

        public async Task<T> CallContractAsync<T>(ContractInteraction contractInteraction)
        {
            var result = await CallContractAsync(contractInteraction);
            
            try
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(result);
            }
            catch (Exception ex)
            {
                _loggingService?.LogError($"Failed to deserialize contract call result: {ex.Message}");
                throw;
            }
        }
    }
}

