using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StarkSharp.Rpc.Platforms;

namespace StarkSharp.Rpc
{
    public enum RpcPlatform
    {
        Unity,
        DotNet,
        Godot,
        CryEngine
    }

    public enum RpcPlatformType
    {
        WebGl,
        Standart
    }
    public class JsonRpcRequest
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

    public class JsonRpcRequestHandler
    {
        public static async Task<JsonRpcResponse> SendJsonRpcRequest(string method, string contractAddress, string entryPointSelector, object data, RpcPlatform rpcPlatform)
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

            var requestData = new JsonRpcRequest
            {
                id = 1,
                method = method,
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

            if (rpcPlatform == RpcPlatform.Unity)
            {
                return await RequestUnity.SendPostRequestUnity(Settings.Settings.apiurl, requestData);
            }
            else if (rpcPlatform == RpcPlatform.DotNet)
            {
                return await RequestDotnet.SendPostRequestDotnet(Settings.Settings.apiurl, requestData);
            }
            else
            {
                throw new ArgumentException("Invalid platform specified.");
            }
        }
    }
}
