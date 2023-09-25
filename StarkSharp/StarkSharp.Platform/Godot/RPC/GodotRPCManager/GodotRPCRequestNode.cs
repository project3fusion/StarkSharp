using Godot;
using System;
using System.Net.Http;
using System.Text;
using UnityEditor.Experimental.GraphView;
using WebSocketSharp;

namespace StarkSharp.Platforms.Godot.RPC
{
    public class GodotRPCRequestNode : Node
    {
        public Action<string> successCallback;
        public Action<string> failCallback;
        public string url, json;
        public HTTPRequest httpRequestNode;

        public void AddHTTPRequestNode() => AddChild(httpRequestNode = new HTTPRequest());

        public void SendHTTPRequest(string url, string json)
        {
            GetChild(httpRequestNode.GetIndex()).Connect("request_completed", this, "OnRequestCompleted");
            HTTPRequest httpRequest = (HTTPRequest)GetChild(httpRequestNode.GetIndex());
            string[] headers = new string[] { "Content-Type: application/json" };
            httpRequest.Request(url, headers, true, HTTPClient.Method.Post, json);
        }

        public void OnRequestCompleted(int result, int response_code, string[] headers, byte[] body)
        {
            JSONParseResult json = JSON.Parse(Encoding.UTF8.GetString(body));
            successCallback(json.Result.ToString());
            QueueFree();
        }
    }
}