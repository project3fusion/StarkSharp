using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace StarkSharp.Rpc.Platforms
{
    public class RequestDotnet
    {
        public static async Task<JsonRpcResponse> SendPostRequestDotnet(string url, JsonRpcRequest requestData)
        {
            string json = JsonConvert.SerializeObject(requestData);

            Console.WriteLine("JSON-RPC Request: " + json);

            using (var httpClient = new HttpClient())
            {
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                try
                {
                    HttpResponseMessage response = await httpClient.PostAsync(url, content);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseText = await response.Content.ReadAsStringAsync();
                        Console.WriteLine("JSON-RPC Response: " + responseText);
                        return JsonConvert.DeserializeObject<JsonRpcResponse>(responseText);
                    }
                    else
                    {
                        Console.WriteLine("Error: " + response.ReasonPhrase);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception: " + ex.Message);
                }
            }

            return null;
        }

        public async Task<JsonRpcResponse> SendPostRequest(JsonRpcRequest requestData)
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
