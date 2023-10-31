using StarkSharp.Connectors.Components;
using StarkSharp.Fusion.Sharpion.Manager.IonPlatforms.Unity;
using StarkSharp.Rpc;
using System;

namespace StarkSharp.Platforms.Unity.SharpIon
{
    public class UnitySharpionPlatform : UnityPlatform
    {
        public override void SendTransaction(Platform platform, TransactionInteraction transactionInteraction, Action<JsonRpcResponse> successCallback, Action<JsonRpcResponse> errorCallback) { IonUnity.socket.SendTransaction(transactionInteraction); }
        public void ConnectServer() { IonUnity.socket.ConnectToServer(); }
        public void ConnectWallet() { IonUnity.socket.ConnectWallet(); }
    }
}
