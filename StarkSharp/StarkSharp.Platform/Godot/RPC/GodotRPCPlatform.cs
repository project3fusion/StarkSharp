using Newtonsoft.Json;
using StarkSharp.Rpc;
using System;
using System.Collections.Generic;
using HTTPRequest = Godot.HTTPRequest;
using Godot;
using StarkSharp.Components;
using StarkSharp.Connectors.Components;

namespace StarkSharp.Platforms.Godot.RPC
{
    internal class GodotRPCPlatform : GodotPlatform
    {
        public override void CallContract(ContractInteraction contractInteraction, Action<string> successCallback, Action<string> errorCallback)
        {
            if (contractInteraction != null)
            {
                var newGodotRPCRequestNode = GodotRPCManager.Instance.CreateNewNode();
                newGodotRPCRequestNode.AddHTTPRequestNode();
                newGodotRPCRequestNode.successCallback = successCallback;
                newGodotRPCRequestNode.failCallback = errorCallback;

                var requestdata = JsonRpcHandler.GenerateRequestData(contractInteraction.ContractAdress, contractInteraction.EntryPoint, contractInteraction.CallData);
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