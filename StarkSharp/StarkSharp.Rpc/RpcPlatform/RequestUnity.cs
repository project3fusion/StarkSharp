using System;
using System.Text;

using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.Networking;

using Newtonsoft.Json;


namespace StarkSharp.Rpc.Platforms
{
    public class RequestUnity
    {


        public static async Task<JsonRpcResponse> SendPostRequestUnity(string url, JsonRpcRequest requestData) 
        {
            string json = JsonConvert.SerializeObject(requestData);

            Debug.Log("JSON-RPC Request: " + json);

            UnityWebRequest www = new UnityWebRequest(url, "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            Debug.Log($"Sending POST request to {url} with data: {json}");

            // Wrap UnityWebRequest in TaskCompletionSource to use await
            await new UnityWebRequestAwaiter(www);

            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.LogError("Error: " + www.error);
                return null;
            }
            else
            {
                string responseText = www.downloadHandler.text;
                Debug.Log("JSON-RPC Response: " + responseText);
                return JsonConvert.DeserializeObject<JsonRpcResponse>(responseText);
            }
        }


        // Helper class to wrap UnityWebRequest with TaskCompletionSource
        public class UnityWebRequestAwaiter : INotifyCompletion
        {
            private UnityWebRequestAsyncOperation asyncOp;
            private Action continuation;

            public UnityWebRequestAwaiter(UnityWebRequest www)
            {
                this.asyncOp = www.SendWebRequest();
                asyncOp.completed += OnRequestCompleted;
            }

            public bool IsCompleted => asyncOp.isDone;

            public void OnCompleted(Action continuation)
            {
                this.continuation = continuation;
            }

            public void GetResult() { }

            private void OnRequestCompleted(AsyncOperation obj)
            {
                continuation?.Invoke();
            }

           
            public UnityWebRequestAwaiter GetAwaiter()
            {
                return this;
            }
        }

        public UnityWebRequestAwaiter GetAwaiter(UnityWebRequest www)
        {
            return new UnityWebRequestAwaiter(www);
        }
    }
}
