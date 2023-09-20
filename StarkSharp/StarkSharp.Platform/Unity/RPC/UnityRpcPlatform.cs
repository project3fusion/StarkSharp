using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json;

using StarkSharp.Rpc;

using UnityEngine;
using UnityEngine.Networking;

using StarkSharp.RPC.Components;

namespace StarkSharp.Platforms.Unity.RPC
{
    public class TempCoroutineMonoBehaviour : MonoBehaviour { }
    public class UnityRpcPlatform : UnityPlatform
    {
        MonoBehaviour mb = new GameObject("TempCoroutineObject").AddComponent<TempCoroutineMonoBehaviour>();

        public override void CallContract(List<string> callContractData, Action<string> successCallback, Action<string> errorCallback)
        {
            if (callContractData.Count >= 3)
            {
                mb.StartCoroutine(SendJsonRpcRequest(callContractData[0], callContractData[1], callContractData[2], (response) =>
                {
                    if (response == null || response.error != null)
                    {
                        errorCallback?.Invoke(response?.error?.message ?? "Unknown error");
                    }
                    else
                    {
                        successCallback?.Invoke(JsonConvert.SerializeObject(response.result));
                    }
                }));
            }
            else
            {
                errorCallback?.Invoke("Insufficient callContractData parameters");
            }
        }


        public static IEnumerator SendJsonRpcRequest(string contractAddress, string entryPointSelector, object data, Action<JsonRpcResponse> callback)
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

            yield return SendPostRequestUnity(requestData, callback);
        }

        public static IEnumerator SendPostRequestUnity(JsonRpcRequest requestData, Action<JsonRpcResponse> callback)
        {
            string json = JsonConvert.SerializeObject(requestData);

            Debug.Log("JSON-RPC Request: " + json);

            using (UnityWebRequest www = new UnityWebRequest(Settings.Settings.apiurl, "POST"))
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
                www.uploadHandler = new UploadHandlerRaw(bodyRaw);
                www.downloadHandler = new DownloadHandlerBuffer();
                www.SetRequestHeader("Content-Type", "application/json");

                Debug.Log($"Sending POST request to {Settings.Settings.apiurl} with data: {json}");

                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError)
                {
                    Debug.LogError("Error: " + www.error);
                    callback?.Invoke(null);
                }
                else
                {
                    string responseText = www.downloadHandler.text;
                    Debug.Log("JSON-RPC Response: " + responseText);
                    var response = JsonConvert.DeserializeObject<JsonRpcResponse>(responseText);
                    callback?.Invoke(response);
                }
            }
        }
    }
}
