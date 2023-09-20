
using Newtonsoft.Json;
using System.Collections.Generic;
using StarkSharp.RPC.Components;
using StarkSharp.RPC.Utils;


namespace StarkSharp.RPC.Token
{
    public class ERCStandart
    {
        public static List<string> GenerateStandartData(string contractAddress, string entryPoint, string[] callData)
        {
            string callDataString = JsonConvert.SerializeObject(new CallDataComponent { callData = callData });
            return new List<string> {contractAddress, StarknetOps.CalculateFunctionSelector(entryPoint), callDataString };
        }

    }
}
