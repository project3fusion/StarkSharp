using StarkSharp.Rpc.Utils;
using System.Collections.Generic;

namespace StarkSharp.Connector.Components
{
    public class ContractInteraction
    {
        public string ContractAdress {  get; set; } 
        public string EntryPoint { get; set; } 
        public string CallData { get; set; }    

        public ContractInteraction(string _ContractAdress, string _EntryPoint , string _CallData) {

            ContractAdress = _ContractAdress;
            EntryPoint = StarknetOps.CalculateFunctionSelector(_EntryPoint);
            CallData = _CallData;

        }
    }
}
