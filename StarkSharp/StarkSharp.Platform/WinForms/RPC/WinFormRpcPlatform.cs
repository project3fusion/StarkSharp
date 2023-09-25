using Newtonsoft.Json;
using StarkSharp.Components;
using StarkSharp.Rpc;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace StarkSharp.Platforms.Winforms.RPC
{
    public class WinFormRpcPlatform : WinFormPlatform
    {
        public async Task CallContract(List<string> callContractData, Action<string> successCallback, Action<string> errorCallback)
        {
            if (callContractData.Count >= 3)
            {
                var response = await SendJsonRpcRequest(callContractData[0], callContractData[1], callContractData[2]);
                if (response == null || response.error != null)
                {
                    errorCallback?.Invoke(response?.error?.message ?? "Unknown error");
                }
                else
                {
                    successCallback?.Invoke(JsonConvert.SerializeObject(response.result));
                }
            }
            else
            {
                errorCallback?.Invoke("Insufficient callContractData parameters");
            }
        }

        public async Task<JsonRpcResponse> SendJsonRpcRequest(string contractAddress, string entryPointSelector, object data)
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
                        calldata = JsonConvert.DeserializeObject<CallDataComponent>(serializedData).callData
                    },
                    "latest"
                }
            };

            return await SendPostRequest(requestData);
        }

        public async Task<JsonRpcResponse> SendPostRequest(JsonRpc requestData)
        {
            string json = JsonConvert.SerializeObject(requestData);

            Console.WriteLine("JSON-RPC Request: " + json);

            using (var httpClient = new HttpClient())
            {
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync(Settings.Settings.apiurl, content);

                if (response.IsSuccessStatusCode)
                {
                    string responseText = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("JSON-RPC Response: " + responseText);
                    return JsonConvert.DeserializeObject<JsonRpcResponse>(responseText);
                }
                else
                {
                    Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                    return null;
                }
            }
        }
    }
}
