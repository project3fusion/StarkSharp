using Newtonsoft.Json;
using StarkSharp.Connectors.Components;
using StarkSharp.Rpc.Utils;

namespace StarkSharp.Components.Token
{
    public class ERCStandart
    {
        public static ContractInteraction GenerateStandartData(string contractAddress, string entryPoint, string[] callData)
        {
            string callDataString = JsonConvert.SerializeObject(callData);
            return new ContractInteraction(contractAddress, entryPoint, callDataString);
        }

        public static ContractInteraction GenerateStandartData(string contractAddress, ContractFunction function, string[] callData) =>
            GenerateStandartData(contractAddress, function.ToFunctionName(), callData);

        public static ContractInteraction GenerateStandartData(string contractAddress, ERC20Function function, string[] callData) =>
            GenerateStandartData(contractAddress, function.ToFunctionName(), callData);

        public static ContractInteraction GenerateStandartData(string contractAddress, ERC721Function function, string[] callData) =>
            GenerateStandartData(contractAddress, function.ToFunctionName(), callData);
    }
}
