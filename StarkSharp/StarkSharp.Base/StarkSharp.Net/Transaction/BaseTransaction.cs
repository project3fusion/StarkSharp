using System;
using System.Collections.Generic;
using StarkSharp.Base.Net.Hash;

namespace StarkSharp.Base.Net.Transaction
{
    /// <summary>
    /// Base class for all Starknet transactions
    /// </summary>
    public abstract class BaseTransaction
    {
        public NetHash TransactionHash { get; set; }
        public string Type { get; set; }
        public string Version { get; set; }
        public NetHash ContractAddress { get; set; }
        public string EntryPointSelector { get; set; }
        public List<string> Calldata { get; set; }
        public string MaxFee { get; set; }
        public string Nonce { get; set; }
        public List<string> Signature { get; set; }
        public NetHash BlockHash { get; set; }
        public int? BlockNumber { get; set; }
        public string Status { get; set; }
    }
}
