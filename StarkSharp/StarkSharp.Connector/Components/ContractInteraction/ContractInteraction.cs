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
            EntryPoint = _EntryPoint;
            CallData = _CallData;

        }

        public string GenerateEntryPointHex(string entryPointName) => entryPointName;

        public List<string> GetParameters() => new List<string>() { ContractAdress, EntryPoint, CallData };

    }
}
