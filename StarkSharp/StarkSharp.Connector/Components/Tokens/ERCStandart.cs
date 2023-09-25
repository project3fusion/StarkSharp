using Newtonsoft.Json;
using System.Collections.Generic;
using StarkSharp.Rpc.Utils;


namespace StarkSharp.Components.Token
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
