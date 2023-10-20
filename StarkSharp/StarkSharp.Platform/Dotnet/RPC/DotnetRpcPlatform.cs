using System;
using Newtonsoft.Json;
using StarkSharp.Connectors.Components;
using StarkSharp.Rpc;

using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace StarkSharp.Platforms.Dotnet.RPC
{
    public class DotnetRpcPlatform : DotnetPlatform
    {
        public override async void CallContract(ContractInteraction contractInteraction, Action<string> successCallback, Action<string> errorCallback)
        {
            if (contractInteraction != null)
            {
                var requestdata = JsonRpcHandler.GenerateRequestData(contractInteraction.ContractAdress,contractInteraction.EntryPoint,contractInteraction.CallData);
                var response = await SendPostRequest(requestdata);

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
