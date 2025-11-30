using System;
using System.Threading.Tasks;
using StarkSharp.Base.Net;
using StarkSharp.Base.Net.Hash;
using StarkSharp.Base.Net.Models;
using StarkSharp.Base.Net.Transaction;
using static StarkSharp.Base.Net.Models.NetModel;
using static StarkSharp.Base.Net.Transaction.BaseTransactionComponent;

namespace StarkSharp.Base.Provider
{
    /// <summary>
    /// Main provider class for Starknet interactions
    /// Wraps NetClient and provides high-level API
    /// </summary>
    public class StarkProvider
    {
        private readonly NetClient _client;

        public StarkProvider(NetClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        /// <summary>
        /// Gets a block by hash or number
        /// </summary>
        public Task<StarknetBlock> GetBlockAsync(NetHash blockHash = null, int? blockNumber = null)
        {
            return Task.FromResult(_client.GetBlock(blockHash, blockNumber));
        }

        /// <summary>
        /// Gets transaction by hash
        /// </summary>
        public Task<BaseTransaction> GetTransactionAsync(NetHash txHash)
        {
            return Task.FromResult(_client.GetTransaction(txHash));
        }

        /// <summary>
        /// Gets transaction receipt
        /// </summary>
        public Task<TransactionReceipt> GetTransactionReceiptAsync(NetHash txHash)
        {
            return _client.GetTransactionReceipt(txHash);
        }

        /// <summary>
        /// Waits for transaction to be confirmed
        /// </summary>
        public Task<TransactionReceipt> WaitForTransactionAsync(NetHash txHash, float checkInterval = 2f, int retries = 500)
        {
            return _client.WaitForTx(txHash, null, checkInterval, retries);
        }

        /// <summary>
        /// Calls a contract method
        /// </summary>
        public Task<List<int>> CallContractAsync(NetCall call, NetHash blockHash = null, int? blockNumber = null)
        {
            return Task.FromResult(_client.CallContract(call, blockHash, blockNumber));
        }

        /// <summary>
        /// Sends an invoke transaction
        /// </summary>
        public Task<SentTransactionResponse> SendTransactionAsync(NetInvoke transaction)
        {
            return _client.SendTransaction(transaction);
        }

        /// <summary>
        /// Estimates fee for a transaction
        /// </summary>
        public Task<EstimatedFee> EstimateFeeAsync(AccountTransaction tx, NetHash blockHash = null, int? blockNumber = null)
        {
            return Task.FromResult(_client.EstimateFee(tx, blockHash, blockNumber));
        }

        /// <summary>
        /// Gets storage value at contract address
        /// </summary>
        public Task<int> GetStorageAtAsync(NetHash contractAddress, int key, NetHash blockHash = null, int? blockNumber = null)
        {
            return Task.FromResult(_client.GetStorageAt(contractAddress, key, blockHash, blockNumber));
        }

        /// <summary>
        /// Gets the network this provider is connected to
        /// </summary>
        public NetNetworks Network => _client.net;
    }
}
