using StarkSharp.Base.Net.Exception;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarkSharp.Base.Net.Transaction
{
    public class BaseTransactionComponent
    {
        public class TransactionReceipt
        {
            public int Id { get; set; }
            public string Jsonrpc { get; set; }
            public string Method { get; set; }
            public List<string> Params { get; set; }
            public TransactionFinalityStatus? finalityStatus { get; set; }
            public TransactionExecutionStatus? executionStatus { get; set; }
            public BaseTransactionComponent.StatusType status { get; set; }
            public string rejectionReason { get; set; }
            public string revertError { get; set; }
        }

        public enum TransactionFinalityStatus
        {
            ACCEPTED_ON_L1,
            ACCEPTED_ON_L2,
            Unknown
        }

        public enum TransactionExecutionStatus
        {
            SUCCEEDED,
            REJECTED,
            REVERTED,
            Status1Execution,
            Status2Execution,
            Unknown
        }

        /// <summary>
        /// Response after sending a transaction
        /// </summary>
        public class SentTransactionResponse
        {
            public NetHash TransactionHash { get; set; }
            public string Status { get; set; }
        }

        /// <summary>
        /// Estimated fee for a transaction
        /// </summary>
        public class EstimatedFee
        {
            public string GasConsumed { get; set; }
            public string GasPrice { get; set; }
            public string OverallFee { get; set; }
            public string Unit { get; set; }
        }

        /// <summary>
        /// Represents a contract call
        /// </summary>
        public class NetCall
        {
            public NetHash ContractAddress { get; set; }
            public string EntryPointSelector { get; set; }
            public List<string> Calldata { get; set; }
        }

        /// <summary>
        /// Represents an invoke transaction
        /// </summary>
        public class NetInvoke
        {
            public NetHash SenderAddress { get; set; }
            public List<Call> Calls { get; set; }
            public string MaxFee { get; set; }
            public string Nonce { get; set; }
            public List<string> Signature { get; set; }
            public string Version { get; set; }
        }

        public class Call
        {
            public NetHash To { get; set; }
            public string Selector { get; set; }
            public List<string> Data { get; set; }
        }

        public class TransactionRejectedError : NetException
        {
            public TransactionRejectedError(string message) : base(message) { }
        }

        public class TransactionRevertedError : NetException
        {
            public TransactionRevertedError(string message) : base(message) { }
        }

        public class TransactionNotReceivedError : NetException
        {
            public TransactionNotReceivedError() : base("Transaction not received within the given retries.") { }
        }

        public static (TransactionFinalityStatus, TransactionExecutionStatus) StatusToFinalityExecution(StatusType status)
        {
            TransactionFinalityStatus finalityStatus;
            TransactionExecutionStatus executionStatus;

            switch (status.ExecutionStatus)
            {
                case TransactionExecutionStatus.SUCCEEDED:
                    executionStatus = TransactionExecutionStatus.SUCCEEDED;
                    break;
                case TransactionExecutionStatus.REJECTED:
                    executionStatus = TransactionExecutionStatus.REJECTED;
                    break;
                case TransactionExecutionStatus.REVERTED:
                    executionStatus = TransactionExecutionStatus.REVERTED;
                    break;
                default:
                    executionStatus = TransactionExecutionStatus.Unknown;
                    break;
            }

            switch (status.FinalityStatus)
            {
                case TransactionFinalityStatus.ACCEPTED_ON_L1:
                    finalityStatus = TransactionFinalityStatus.ACCEPTED_ON_L1;
                    break;
                case TransactionFinalityStatus.ACCEPTED_ON_L2:
                    finalityStatus = TransactionFinalityStatus.ACCEPTED_ON_L2;
                    break;
                default:
                    finalityStatus = TransactionFinalityStatus.Unknown;
                    break;
            }

            return (finalityStatus, executionStatus);
        }

        public enum FinalityStatus { Status1Finality, Status2Finality, Unknown }
        public enum ExecutionStatus { Status1Execution, Status2Execution, Unknown }
        public enum StatusEnumType
        {
            Status1,
            Status2
        }
        public class StatusType
        {
            public TransactionExecutionStatus ExecutionStatus { get; set; }
            public TransactionFinalityStatus FinalityStatus { get; set; }
        }
    }
}
