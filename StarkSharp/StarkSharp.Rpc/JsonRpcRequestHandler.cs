using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StarkSharp.Rpc;

namespace StarkSharp.Api.RPC
{
    public class JsonRpcRequest
    {
        [JsonProperty("jsonrpc")]
        public string jsonrpc { get; set; } = "2.0";

        [JsonProperty("method")]
        public string method { get; set; } = string.Empty;

        [JsonProperty("params")]
        public object[] @params { get; set; } = Array.Empty<object>();

        [JsonProperty("id")]
        public int id { get; set; } = 1;
    }

    /// <summary>
    /// Starknet JSON-RPC client handler. Uses RpcSettingsProvider.ApiUrl for the endpoint.
    /// </summary>
    public class JsonRpcRequestHandler
    {
        private static readonly HttpClient Client = new HttpClient();

        public static string ApiUrl => RpcSettingsProvider.ApiUrl;

        /// <summary>
        /// Sends a generic JSON-RPC request to the configured Starknet node.
        /// </summary>
        public static async Task<JsonRpcResponse> SendRpcAsync(string method, object[] parameters, int id = 1)
        {
            var requestData = new JsonRpcRequest
            {
                id = id,
                method = method,
                @params = parameters
            };

            var responseJson = await SendPostRequestAsync(ApiUrl, requestData);
            return JsonConvert.DeserializeObject<JsonRpcResponse>(responseJson) 
                   ?? new JsonRpcResponse { error = new JsonRpcError { message = "Failed to deserialize RPC response." } };
        }

        public static async Task<JsonRpcResponse> SendJsonRpcRequest(string method, string contractAddress, string entryPointSelector, string userAddress)
        {
            return await Call(contractAddress, entryPointSelector, userAddress);
        }

        /// <summary>starknet_call: invokes a contract function in read-only mode using the default configured API URL.</summary>
        public static Task<JsonRpcResponse> Call(string contractAddress, string entryPointSelector, params string[] calldata) =>
            Call(null, contractAddress, entryPointSelector, calldata);

        /// <summary>starknet_call: invokes a contract function in read-only mode using a specific RPC URL (or default if null).</summary>
        public static async Task<JsonRpcResponse> Call(string? url, string contractAddress, string entryPointSelector, params string[] calldata)
        {
            string targetUrl = string.IsNullOrWhiteSpace(url) ? ApiUrl : url;
            var requestData = new JsonRpcRequest
            {
                id = 1,
                method = "starknet_call",
                @params = new object[]
                {
                    new
                    {
                        contract_address = contractAddress,
                        entry_point_selector = entryPointSelector,
                        calldata = calldata ?? Array.Empty<string>()
                    },
                    "latest"
                }
            };
            var responseJson = await SendPostRequestAsync(targetUrl, requestData);
            return JsonConvert.DeserializeObject<JsonRpcResponse>(responseJson)
                   ?? new JsonRpcResponse { error = new JsonRpcError { message = "Failed to deserialize RPC response." } };
        }

        private static async Task<string> SendPostRequestAsync(string url, object data)
        {
            string json = JsonConvert.SerializeObject(data);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await Client.PostAsync(url, content);
            string responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                var error = $"Error: {response.StatusCode} - {response.ReasonPhrase}. Details: {responseBody}";
                throw new HttpRequestException(error);
            }

            return responseBody;
        }
    }

    /// <summary>Backward compatibility alias; prefer JsonRpcRequestHandler for new code.</summary>
    public class JsonPpcRequest : JsonRpcRequestHandler
    {
        public static string Api_URL => JsonRpcRequestHandler.ApiUrl;
    }
}
