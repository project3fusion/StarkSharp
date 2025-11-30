using System;
using System.Collections.Generic;
using StarkSharp.Base.Net.Hash;

namespace StarkSharp.Base.Net.Models
{
    public class NetModel
    {
        /// <summary>
        /// Represents a Starknet block
        /// </summary>
        public class StarknetBlock
        {
            public NetHash BlockHash { get; set; }
            public int BlockNumber { get; set; }
            public NetHash ParentBlockHash { get; set; }
            public int Timestamp { get; set; }
            public NetHash StateRoot { get; set; }
            public List<NetHash> Transactions { get; set; }
            public string Status { get; set; }
        }

        /// <summary>
        /// Represents transaction traces for a block
        /// </summary>
        public class BlockTransactionTraces
        {
            public List<TransactionTrace> Traces { get; set; }
        }

        public class TransactionTrace
        {
            public NetHash TransactionHash { get; set; }
            public List<FunctionInvocation> FunctionInvocation { get; set; }
        }

        public class FunctionInvocation
        {
            public NetHash ContractAddress { get; set; }
            public string EntryPointSelector { get; set; }
            public List<string> Calldata { get; set; }
            public List<string> Result { get; set; }
        }

        /// <summary>
        /// Response for deploy account transaction
        /// </summary>
        public class DeployAccountTransactionResponse
        {
            public NetHash TransactionHash { get; set; }
            public NetHash ContractAddress { get; set; }
        }

        /// <summary>
        /// Response for declare transaction
        /// </summary>
        public class DeclareTransactionResponse
        {
            public NetHash TransactionHash { get; set; }
            public NetHash ClassHash { get; set; }
        }

        /// <summary>
        /// Represents state update for a block
        /// </summary>
        public class BlockStateUpdate
        {
            public NetHash BlockHash { get; set; }
            public int BlockNumber { get; set; }
            public StateDiff StateDiff { get; set; }
        }

        public class StateDiff
        {
            public List<StorageDiff> StorageDiffs { get; set; }
            public List<DeployedContract> DeployedContracts { get; set; }
        }

        public class StorageDiff
        {
            public NetHash Address { get; set; }
            public List<StorageEntry> StorageEntries { get; set; }
        }

        public class StorageEntry
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }

        public class DeployedContract
        {
            public NetHash Address { get; set; }
            public NetHash ClassHash { get; set; }
        }

        /// <summary>
        /// Represents an account transaction
        /// </summary>
        public class AccountTransaction
        {
            public string Type { get; set; }
            public NetHash SenderAddress { get; set; }
            public string MaxFee { get; set; }
            public string Nonce { get; set; }
            public List<string> Signature { get; set; }
            public string Version { get; set; }
        }

        /// <summary>
        /// Represents a deploy account transaction
        /// </summary>
        public class DeployAccount
        {
            public NetHash ClassHash { get; set; }
            public NetHash ContractAddressSalt { get; set; }
            public List<string> ConstructorCalldata { get; set; }
            public string MaxFee { get; set; }
            public string Nonce { get; set; }
            public List<string> Signature { get; set; }
            public string Version { get; set; }
        }

        /// <summary>
        /// Represents a declare transaction
        /// </summary>
        public class Declare
        {
            public NetHash SenderAddress { get; set; }
            public NetHash ClassHash { get; set; }
            public string MaxFee { get; set; }
            public string Nonce { get; set; }
            public List<string> Signature { get; set; }
            public string Version { get; set; }
        }
    }
}
