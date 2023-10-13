using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace StarkSharp.Base.Net
{
    public class NetClient
    {
        public abstract NetNetworks net { get; }

        public abstract StarknetBlock GetBlock(Hash? blockHash = null, int? blockNumber = null);

        public abstract BlockTransactionTraces TraceBlockTransactions(Hash? blockHash = null, int? blockNumber = null);

        public abstract BlockStateUpdate GetStateUpdate(Hash? blockHash = null, int? blockNumber = null);

        public abstract int GetStorageAt(Hash contractAddress, int key, Hash? blockHash = null, int? blockNumber = null);

        public abstract Transaction GetTransaction(Hash txHash);

        public abstract TransactionReceipt GetTransactionReceipt(Hash txHash);

        public async Task<TransactionReceipt> WaitForTx(Hash txHash, bool? waitForAccept = null, float checkInterval = 2, int retries = 500)
        {
            if (checkInterval <= 0)
                throw new ArgumentException("Argument check_interval has to be greater than 0.");
            if (retries <= 0)
                throw new ArgumentException("Argument retries has to be greater than 0.");
            if (waitForAccept is not null)
                warnings.warn("Parameter `wait_for_accept` has been deprecated - since Starknet 0.12.0, transactions in a PENDING"
    
                             " block have status ACCEPTED_ON_L2.");

            while (true)
            {
                try
                {
                    TransactionReceipt txReceipt = await this.GetTransactionReceipt(txHash);

                    var deprecatedStatus = _statusToFinalityExecution(txReceipt.status);
                    var finalityStatus = txReceipt.finalityStatus ?? deprecatedStatus[0];
                    var executionStatus = txReceipt.executionStatus ?? deprecatedStatus[1];

                    if (executionStatus == TransactionExecutionStatus.REJECTED)
                        throw new TransactionRejectedError(message: txReceipt.rejectionReason);

                    if (executionStatus == TransactionExecutionStatus.REVERTED)
                        throw new TransactionRevertedError(message: txReceipt.revertError);

                    if (executionStatus == TransactionExecutionStatus.SUCCEEDED)
                        return txReceipt;

                    if (finalityStatus in (TransactionFinalityStatus.ACCEPTED_ON_L2, TransactionFinalityStatus.ACCEPTED_ON_L1))
                    return txReceipt;

                    retries -= 1;
                    if (retries == 0)
                        throw new TransactionNotReceivedError();

                    await Task.Delay(checkInterval);
                }
                catch (TaskCanceledException ex)
                {
                    throw new TransactionNotReceivedError();
                }
                catch (ClientError ex)
                {
                    if ("Transaction hash not found" != ex.message)
                        throw ex;
                    retries -= 1;
                    if (retries == 0)
                        throw new TransactionNotReceivedError();

                    await Task.Delay(checkInterval);
                }
            }
        }

        public abstract EstimatedFee EstimateFee(AccountTransaction tx, Hash? blockHash = null, int? blockNumber = null);

        public abstract List<int> CallContract(Call call, Hash? blockHash = null, int? blockNumber = null);

        public async Task<SentTransactionResponse> SendTransaction(Invoke transaction)
        {
            return await this._sendTransaction(transaction);
        }

        public abstract DeployAccountTransactionResponse DeployAccount(DeployAccount transaction);

        public abstract DeclareTransactionResponse Declare(Declare transaction);

        public abstract int GetClassHashAt(Hash contractAddress, Hash? blockHash = null, int? blockNumber = null);

        public abstract Task<int> GetClassHashAt(Hash contractAddress, Hash? blockHash = null, int? blockNumber = null);

        public abstract Task<ContractClass> GetContractClassAt(Hash contractAddress, Hash? blockHash = null, int? blockNumber = null);

        public abstract Task<ContractClass> GetContractClassByHash(Hash classHash);

        public abstract Task<SierraContractClass> GetSierraContractClassAt(Hash contractAddress, Hash? blockHash = null, int? blockNumber = null);

        public abstract Task<SierraContractClass> GetSierraContractClassByHash(Hash classHash);

        protected abstract Task<SentTransactionResponse> _sendTransaction(Invoke transaction);


    }
}
