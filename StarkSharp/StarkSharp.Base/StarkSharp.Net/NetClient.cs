using System;
using System.Collections.Generic;
using static StarkSharp.StarkSharp.Base.StarkSharp.Net.NetEnum;


namespace StarkSharp.StarkSharp.Base.StarkSharp.Net
{
    public class NetClient
    {
       

        public class Call
        {
            public int ToAddr { get; set; }
            public int Selector { get; set; }
            public List<int> Calldata { get; set; }
            public CallType Type { get; set; }
            public int Block { get; set; }
            public Tag Tag { get; set; }
        }

      

        public class Event
        {
            public EventType Type { get; set; }
            public int FromAddress { get; set; }
            public List<int> Keys { get; set; }
            public List<int> Data { get; set; }
        }

        public class EventsChunk
        {
            public List<Event> Events { get; set; }
            public string ContinuationToken { get; set; }
        }

     

        public class L1toL2Message
        {
            public List<int> Payload { get; set; }
            public int Nonce { get; set; }
            public int Selector { get; set; }
            public int L1Address { get; set; }
            public int L2Address { get; set; }
            public L1toL2Type Type { get; set; }
        }

       

        public abstract class Transaction
        {
            public int? Hash { get; set; }
            public List<int> Signature { get; set; }
            public int MaxFee { get; set; }
            public int Version { get; set; }
            public TransactionType Type { get; set; }
        }

        public class InvokeTransaction : Transaction
        {
            public int SenderAddress { get; set; }
            public List<int> Calldata { get; set; }
            public int? EntryPointSelector { get; set; }
            public int? Nonce { get; set; }
        }

        public class DeclareTransaction : Transaction
        {
            // Ek özellikler buraya eklenmelidir
        }

        public class DeployAccountTransaction : Transaction
        {
            // Ek özellikler buraya eklenmelidir
        }

        public class DeployTransaction : Transaction
        {
            // Ek özellikler buraya eklenmelidir
        }

        public class L1HandlerTransaction : Transaction
        {
            // Ek özellikler buraya eklenmelidir
        }
    }
}
