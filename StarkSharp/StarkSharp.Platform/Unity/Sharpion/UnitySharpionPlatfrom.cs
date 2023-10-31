using StarkSharp.Connectors.Components;
using StarkSharp.Fusion.Sharpion;
using StarkSharp.Fusion.Sharpion.Manager;
using StarkSharp.Rpc;
using System;

namespace StarkSharp.Platforms.Unity.Sharpion
{
    public class UnitySharpionPlatform : UnityPlatform
    {
        private SharpionManager manager;

        public UnitySharpionPlatform()
        {
            manager = SharpionManager.New(PlatformName.Unity);
        }

        public override void ConnectWallet(string walletType, int id)
        {
            manager.ConnectWallet();
        }

        public override void SendTransaction(Platform platform, TransactionInteraction transactionInteraction, Action<JsonRpcResponse> successCallback, Action<JsonRpcResponse> errorCallback)
        {
            manager.SendTransaction(transactionInteraction);
        }
    }
}
