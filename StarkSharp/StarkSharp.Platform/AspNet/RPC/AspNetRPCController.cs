using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using StarkSharp.Platforms.AspNet;
using StarkSharp.Rpc;

public class AspNetRPCController : AspNetPlatform
{
    private readonly ILogger<AspNetRPCController> _logger;


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


    private async Task<IActionResult> SendJsonRpcRequest(string contractAddress, string entryPointSelector, string data)
    {
        var requestData = new
        {
            id = 1,
            method = "starknet_call",
            @params = new object[]
            {
                    new
                    {
                        contract_address = contractAddress,
                        entry_point_selector = entryPointSelector,
                        calldata = data
                    },
                    "latest"
            }
        };

        _logger.LogInformation($"Request Data: {JsonConvert.SerializeObject(requestData)}");

        var httpClient = new HttpClient();
        var content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");

        var responseMessage = await httpClient.PostAsync("https://starknet-goerli.infura.io/v3/86bbe5b6292546f3ac1f3fe91f39e559", content);

        if (responseMessage.IsSuccessStatusCode)
        {
            string responseText = await responseMessage.Content.ReadAsStringAsync();
            var rpcResponse = JsonConvert.DeserializeObject<JsonRpcResponse>(responseText);

            if (rpcResponse.error != null)
            {
                return new BadRequestObjectResult(JsonConvert.SerializeObject(rpcResponse.error));
            }
            return new OkObjectResult(JsonConvert.SerializeObject(rpcResponse.result));
        }
        else
        {
            return new BadRequestObjectResult($"Error: {responseMessage.StatusCode} - {responseMessage.ReasonPhrase}");
        }
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

}