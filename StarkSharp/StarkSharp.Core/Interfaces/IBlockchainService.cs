using System.Threading.Tasks;
using StarkSharp.Connectors.Components;

namespace StarkSharp.Core.Interfaces
{
    /// <summary>
    /// Service interface for blockchain query operations
    /// </summary>
    public interface IBlockchainService
    {
        Task<string> QueryAsync(QueryInteraction queryInteraction);
        Task<T> QueryAsync<T>(QueryInteraction queryInteraction);
    }
}

