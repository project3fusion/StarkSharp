using System;
using System.Text;
using System.Collections;

using Newtonsoft.Json;
using StarkSharp.Connectors.Components;
using StarkSharp.Rpc;

using UnityEngine;
using UnityEngine.Networking;
using StarkSharp.Rpc.Modules.Transactions;
using StarkSharp.Tools.Notification;

namespace StarkSharp.Platforms.Unity.RPC
{
    public class CoroutineMonoBehaviour : MonoBehaviour
    {
        public static CoroutineMonoBehaviour Instance;
        private void Start() => Instance = this;
    }
    public class UnityRpcPlatform : UnityPlatform
    {
        MonoBehaviour mb = (CoroutineMonoBehaviour.Instance == null) ? new GameObject("TempCoroutineObject").AddComponent<CoroutineMonoBehaviour>() : CoroutineMonoBehaviour.Instance;

        public override void CallContract(ContractInteraction contractInteraction, Action<string> successCallback, Action<string> errorCallback)
        {
            try
            {
                if (contractInteraction != null)
                {
                    var requestdata = JsonRpcHandler.GenerateContractRequestData(contractInteraction.ContractAdress, contractInteraction.EntryPoint, contractInteraction.CallData);
                    mb.StartCoroutine(SendPostRequestUnity(requestdata, (response) =>
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
            catch (Exception ex)
            {
                errorCallback?.Invoke("An error occurred: " + ex.Message);
            }
        }

        public override void SendTransaction(TransactionInteraction transactionInteraction, Action<JsonRpcResponse> successCallback, Action<JsonRpcResponse> errorCallback)
        {
            try
            {
                if (transactionInteraction.FunctionName != null)
                {
                    Transaction transaction = new Transaction(this);
                    var requestData = transaction.CreateTransaction(transactionInteraction);

                    mb.StartCoroutine(SendPostRequestUnity(requestData, response =>
                    {
                        transaction.OnNonceComplete(this, transactionInteraction, response);
                    }));
                }
                else
                {
                    errorCallback?.Invoke(new JsonRpcResponse
                    {
                        error = new JsonRpcError { code = -1, message = "Insufficient callContractData parameters" }
                    });
                }
            }
            catch (Exception ex)
            {
                errorCallback?.Invoke(new JsonRpcResponse
                {
                    error = new JsonRpcError { code = -1, message = "An error occurred: " + ex.Message }
                });
            }
        }

        public override void PlatformRequest(JsonRpc requestData, Action<JsonRpcResponse> Callback)
        {
            try
            {
                mb.StartCoroutine(SendPostRequestUnity(requestData, (response) =>
                {
                    if (response == null || response.error != null)
                    {
                        Callback?.Invoke(new JsonRpcResponse
                        {
                            error = new JsonRpcError
                            {
                                message = response?.error?.message ?? "Unknown error"
                            }
                        });
                    }
                    else
                    {
                        string resultString = response.result as string;

                        if (resultString != null)
                        {
                            Callback?.Invoke(new JsonRpcResponse
                            {
                                result = resultString
                            });
                        }
                        else
                        {
                            Debug.Log(requestData);
                        }
                    }
                }));
            }
            catch (Exception ex)
            {
                Callback?.Invoke(new JsonRpcResponse
                {
                    error = new JsonRpcError { code = -1, message = "An error occurred: " + ex.Message }
                });
            }
        }

        public static IEnumerator SendPostRequestUnity(JsonRpc requestData, Action<JsonRpcResponse> callback)
        {
            string json = JsonConvert.SerializeObject(requestData);

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
                    try
                    {
                        string responseText = www.downloadHandler.text;
                        var response = JsonConvert.DeserializeObject<JsonRpcResponse>(responseText);
                        callback?.Invoke(response);
                    }
                    catch (Exception ex)
                    {
                        callback?.Invoke(new JsonRpcResponse
                        {
                            error = new JsonRpcError { code = -1, message = "An error occurred: " + ex.Message }
                        });
                    }
                }
            }
        }
        public override void PlatformLog(string LogMessage, NotificationType notitype) { Notify.ShowNotification(LogMessage, notitype, NotificationPlatform.Unity); }
    }
}
