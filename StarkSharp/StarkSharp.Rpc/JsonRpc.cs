using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using StarkSharp.Tools.Notification;

namespace StarkSharp.Rpc
{
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
        public object result { get; set; }
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
        public static JsonRpc GenerateRequestData(string method, object data)
        {
            try
            {
                object[] refinedData;
                if (data.GetType().IsArray)
                {
                    refinedData = new object[((object[])data).Length];
                    for (int i = 0; i < ((object[])data).Length; i++)
                    {
                        if (((object[])data)[i].GetType() == typeof(string))
                        {
                            refinedData[i] = ((string)((object[])data)[i]).Trim();
                        }
                        else
                        {
                            refinedData[i] = ((object[])data)[i];
                        }
                    }
                }
                else
                {
                    refinedData = new object[] { data };
                }

                var requestData = new JsonRpc
                {
                    id = 1,
                    method = method,
                    @params = refinedData
                };

                return requestData;
            }
            catch (Exception ex)
            {
                Notify.ShowNotification($"Error generating request data: {ex.Message}", NotificationType.Error, NotificationPlatform.Console);
                return null;
            }
        }
        public static JsonRpc GenerateContractRequestData(string contractAddress, string entryPointSelector, string serializedData)
        {
            try
            {
                // serializedData'nın zaten bir JSON dizisi olup olmadığını kontrol et ve dizi olarak çözümle
                JArray deserializedData = serializedData.StartsWith("[") && serializedData.EndsWith("]") ?
                                          JArray.Parse(serializedData) :
                                          new JArray(serializedData);
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
                    calldata = deserializedData // Dizi olarak atandı
                },
                "latest"
                    }
                };

                return requestData;
            }
            catch (Exception ex)
            {
                Notify.ShowNotification($"Error generating request data: {ex.Message}", NotificationType.Error, NotificationPlatform.Console);
                return null;
            }
        }
    }
}
