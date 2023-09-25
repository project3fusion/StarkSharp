using Newtonsoft.Json;
using System.Collections.Generic;
using StarkSharp.Rpc.Utils;
using StarkSharp.Connector.Components;


namespace StarkSharp.Components.Token
{
    public class ERCStandart
    {
        public static ContractInteraction GenerateStandartData(string contractAddress, string entryPoint, string[] callData)
        {
            string callDataString = JsonConvert.SerializeObject(new CallDataComponent { callData = callData });
            return new ContractInteraction ( contractAddress, entryPoint, callDataString );
        }

    }
}
