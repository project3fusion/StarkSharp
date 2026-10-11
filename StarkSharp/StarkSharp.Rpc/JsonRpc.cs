using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StarkSharp.Connectors.Components;
using StarkSharp.Tools.Notification;

namespace StarkSharp.Rpc
{
    public class JsonRpc
    {
        public int id { get; set; }
        public string jsonrpc { get; } = "2.0";
        public string method { get; set; }
        public object[] @params { get; set; }
    }
    public class JsonRpcResponse
    {
        public string jsonrpc { get; set; }
        public int id { get; set; }
        public object result { get; set; }
        public JsonRpcError error { get; set; }

        [JsonIgnore]
        public bool HasError => error != null;

        [JsonIgnore]
        public string[]? ResultAsArray => result switch
        {
            JArray jArray => jArray.ToObject<string[]>(),
            string[] strArray => strArray,
            _ => null
        };

        [JsonIgnore]
        public string? ResultAsString => result?.ToString();

        public T? GetResult<T>()
        {
            if (result == null) return default;
            if (result is JToken token) return token.ToObject<T>();
            if (result is T typed) return typed;
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(result));
        }
    }

    public class JsonRpcError
    {
        public int code { get; set; }
        public string message { get; set; }
        public object data { get; set; }
    }

     public class JsonRpcHandler
    {
		public class TransactionRpc : JsonRpc { }
		public class QueryRpc : JsonRpc { }
		
        public static JsonRpc GenerateRequestData(string method, object[] data)
        {
            try
            {
                var requestData = new JsonRpc
                {
                    id = 1,
                    method = method,
                    @params = data
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
                    calldata = deserializedData
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

        public static TransactionRpc GenerateTransactionRequestData(string senderadress, string serializedData, string _maxfee, string _nonce, string[] _signature, string _type, string _version)
        {
            try
            {
                JArray deserializedData = serializedData.StartsWith("[") && serializedData.EndsWith("]") ?
                                          JArray.Parse(serializedData) :
                                          new JArray(serializedData);
                var requestBody = new
                {
                    id = 1,
                    jsonrpc = "2.0",
                    method = "starknet_addInvokeTransaction",
                    @params = new[]
                    {
                new
                {
                    sender_address = senderadress,
                    type = _type,
                    nonce = _nonce,
                    signature = _signature,
                    version = _version,
                    max_fee = _maxfee,
                    calldata = deserializedData.ToObject<string[]>()
                    }
                }
                };

                TransactionRpc jsonData = JsonConvert.DeserializeObject<TransactionRpc>(JsonConvert.SerializeObject(requestBody));
                return jsonData;
            }
            catch (Exception ex)
            {
                Notify.ShowNotification($"Error generating request data: {ex.Message}", NotificationType.Error, NotificationPlatform.Console);
                return null;
            }
        }

        public static QueryRpc GenerateQueryRequestData(QueryInteraction queryInteraction)
        {
            try
            {

                var transactionRequest = new
                {
                    Id = 1,
                    Jsonrpc = "2.0",
                    Method = queryInteraction._queryType,
                    Params = new[]
                    {
                        new
                        {
                           queryInteraction._query
                        }
                    }
                };

                return JsonConvert.DeserializeObject<QueryRpc>(JsonConvert.SerializeObject(transactionRequest));
            }
            catch (Exception ex)
            {
                Notify.ShowNotification($"Error generating query request data: {ex.Message}", NotificationType.Error, NotificationPlatform.Console);
                return null;
            }
        }
    }
}
