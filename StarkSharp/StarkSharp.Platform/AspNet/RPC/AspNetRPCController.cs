using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StarkSharp.Connectors.Components;
using StarkSharp.Platforms.AspNet;
using StarkSharp.Rpc;
using StarkSharp.Settings;

public class AspNetRPCController : AspNetPlatform
{

    public override async void CallContract(ContractInteraction contractInteraction, Action<string> successCallback, Action<string> errorCallback)
    {
        if (contractInteraction != null)
        {
            var RequestData = JsonRpcHandler.GenerateRequestData(contractInteraction.ContractAdress, contractInteraction.EntryPoint, contractInteraction.CallData);


            var response = await SendPostRequest(RequestData);
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
            var response = await httpClient.PostAsync(Settings.apiurl, content);

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