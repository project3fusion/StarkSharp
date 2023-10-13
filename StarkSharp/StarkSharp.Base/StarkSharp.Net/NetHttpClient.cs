using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace StarkSharp.Base.Net
{
    public class NetHttpClient
    {
        public abstract class HttpClientBase
        {
            protected readonly HttpClient _client;

            public HttpClientBase(string url)
            {
                _client = new HttpClient { BaseAddress = new Uri(url) };
            }

            public async Task<T> Request<T>(
                HttpMethod method,
                string path,
                Dictionary<string, string>? parameters = null,
                object? payload = null
            )
            {
                var uri = new Uri(_client.BaseAddress, path);
                HttpContent content;

                if (payload != null)
                {
                    content = new StringContent(JsonConvert.SerializeObject(payload), System.Text.Encoding.UTF8, "application/json");
                }
                else
                {
                    content = new FormUrlEncodedContent(parameters ?? new Dictionary<string, string>());
                }

                HttpResponseMessage response = method == HttpMethod.Post
                    ? await _client.PostAsync(uri, content)
                    : await _client.GetAsync(uri); // Consider adding support for other HTTP methods

                if (response.StatusCode >= System.Net.HttpStatusCode.MultipleChoices) // Status code 300 and above
                {
                    throw new ClientError((int)response.StatusCode, await response.Content.ReadAsStringAsync());
                }

                return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
            }

            public abstract Task<T> HandleRequestError<T>(HttpResponseMessage response);
        }

        public class GatewayHttpClient : HttpClientBase
        {
            public GatewayHttpClient(string url) : base(url) { }

            public async Task<T> Call<T>(string method, Dictionary<string, string>? parameters = null)
            {
                return await Request<T>(HttpMethod.Get, method, parameters);
            }

            public async Task<T> Post<T>(string method, object? payload = null, Dictionary<string, string>? parameters = null)
            {
                return await Request<T>(HttpMethod.Post, method, parameters, payload);
            }

            protected override async Task<T> HandleRequestError<T>(HttpResponseMessage response)
            {
                throw new ClientError((int)response.StatusCode, await response.Content.ReadAsStringAsync());
            }
        }

        public class RpcHttpClient : HttpClientBase
        {
            public RpcHttpClient(string url) : base(url) { }

            public async Task<T> Call<T>(string method, Dictionary<string, object> parameters)
            {
                var payload = new Dictionary<string, object>
            {
                {"jsonrpc", "2.0"},
                {"method", method},
                {"params", parameters},
                {"id", 0},
            };

                var response = await Request<Dictionary<string, object>>(HttpMethod.Post, "/jsonrpc", payload: payload);

                if (!response.ContainsKey("result"))
                {
                    HandleRpcError(response);
                }

                return (T)response["result"];
            }

            protected override async Task<T> HandleRequestError<T>(HttpResponseMessage response)
            {
                var responseBody = JsonConvert.DeserializeObject<Dictionary<string, object>>(await response.Content.ReadAsStringAsync());

                if (!responseBody.ContainsKey("error"))
                {
                    throw new ServerError(responseBody);
                }

                throw new ClientError((int)responseBody["error"]["code"], (string)responseBody["error"]["message"]);
            }

            private void HandleRpcError(Dictionary<string, object> response)
            {
                throw new ClientError((int)response["error"]["code"], (string)response["error"]["message"]);
            }
        }

        public class ClientError : Exception
        {
            public ClientError(int code, string message) : base(message)
            {
                Code = code;
            }

            public int Code { get; }
        }

        public class ServerError : Exception
        {
            public ServerError(Dictionary<string, object> response) : base((string)response["message"])
            {
                Response = response;
            }

            public Dictionary<string, object> Response { get; }
        }
    }
}
