using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace StarkSharp.StarkSharp.Base.StarkSharp.Net
{
    public class NetHttpClient
    {
        public enum HttpMethod
        {
            GET,
            POST
        }

        public abstract class HttpClientBase
        {
            protected string Url { get; }
            protected HttpClient Session { get; }

            protected HttpClientBase(string url, HttpClient session = null)
            {
                Url = url;
                Session = session ?? new HttpClient();
            }

            public async Task<string> Request(string address, HttpMethod httpMethod, Dictionary<string, string> parameters = null, string payload = null)
            {
                HttpResponseMessage response;

                switch (httpMethod)
                {
                    case HttpMethod.GET:
                        response = await Session.GetAsync(address);
                        break;
                    case HttpMethod.POST:
                        response = await Session.PostAsync(address, new StringContent(payload));
                        break;
                    default:
                        throw new NotImplementedException();
                }

                await HandleRequestError(response);
                return await response.Content.ReadAsStringAsync();
            }

            protected abstract Task HandleRequestError(HttpResponseMessage response);
        }

        public class ClientError : Exception
        {
            public ClientError(string message, string code) : base(message)
            {
                Code = code;
            }

            public string Code { get; }
        }

        public class ServerError : Exception
        {
            public ServerError(string message, Dictionary<string, object> body) : base(message)
            {
                Body = body;
            }

            public Dictionary<string, object> Body { get; }
        }
    }
}
