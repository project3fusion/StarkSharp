using Newtonsoft.Json;
using StarkSharp.Rpc;
using System;
using System.Collections.Generic;
using HTTPRequest = Godot.HTTPRequest;
using Godot;
using StarkSharp.Components;

namespace StarkSharp.Platforms.Godot.RPC
{
    internal class GodotRPCPlatform : GodotPlatform
    {
        public override void CallContract(List<string> callContractData, Action<string> successCallback, Action<string> errorCallback)
        {
            if (callContractData.Count >= 3)
            {
                var newGodotRPCRequestNode = GodotRPCManager.Instance.CreateNewNode();
                newGodotRPCRequestNode.AddHTTPRequestNode();
                newGodotRPCRequestNode.successCallback = successCallback;
                newGodotRPCRequestNode.failCallback = errorCallback;

                var requestdata = JsonRpcHandler.GenerateRequestData(callContractData[0], callContractData[1], callContractData[2]);
                SendPostRequestGodot(requestdata, newGodotRPCRequestNode);
            }
            else
            {
                errorCallback?.Invoke("Insufficient callContractData parameters");
            }
        }

        

        public static void SendPostRequestGodot(JsonRpc requestData, GodotRPCRequestNode newGodotRPCRequestNode)
        {
            string json = JsonConvert.SerializeObject(requestData);
            newGodotRPCRequestNode.SendHTTPRequest(Settings.Settings.apiurl, json);
        }
    }
}