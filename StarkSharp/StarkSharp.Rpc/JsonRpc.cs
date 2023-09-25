using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace StarkSharp.Rpc
{
    public enum RpcPlatform
    {
        Unity,
        DotNet,
        Godot,
        CryEngine,
        AspNet
    }

    public class JsonRpc
    {
        public string jsonrpc { get; } = "2.0";
        public string method { get; set; }
        public object[] @params { get; set; }
        public int id { get; set; }
    }

    public class JsonRpcResponse
    {
        public string jsonrpc { get; set; }
        public int id { get; set; }
        public List<string> result { get; set; }
        public JsonRpcError error { get; set; }
    }

    public class JsonRpcError
    {
        public int code { get; set; }
        public string message { get; set; }
        public object data { get; set; }
    }

    public class JsonRpcHandler
    {
        public static JsonRpc GenerateRequestData(string contractAddress, string entryPointSelector, object data)
        {
            string serializedData;
            if (data is string || data is ValueType)
            {
                serializedData = data.ToString();
            }
            else
            {
                serializedData = JsonConvert.SerializeObject(data);
            }

            var requestData = new JsonRpc
            {
                id = 1,
                method = "starknet_call",
                @params = new object[]
                {
                new
                {
                    contract_address = contractAddress,
                    entry_point_selector = entryPointSelector,
                    calldata = new string[] { serializedData }
                },
                "latest"
                }
            };

            return requestData;
        }
    }

}
