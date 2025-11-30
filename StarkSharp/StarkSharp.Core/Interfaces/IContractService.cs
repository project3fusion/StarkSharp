using System;
using System.Threading.Tasks;
using StarkSharp.Connectors.Components;

namespace StarkSharp.Core.Interfaces
{
    /// <summary>
    /// Service interface for contract interaction operations
    /// </summary>
    public interface IContractService
    {
        Task<string> CallContractAsync(ContractInteraction contractInteraction);
        Task<T> CallContractAsync<T>(ContractInteraction contractInteraction);
    }
}

