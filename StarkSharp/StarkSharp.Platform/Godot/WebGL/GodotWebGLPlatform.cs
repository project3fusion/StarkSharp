using System;


namespace StarkSharp.Platforms.Godot.WebGL
{
    public class GodotWebGLPlatform : GodotPlatform
    {

        public override void ConnectWallet(string walletType, int id)
        => GodotBrowserManager.ConnectWallet(walletType, id);

        public override void SendTransaction(string walletType, int id, string contractAddress, string entryPoint, string callData)
             => GodotBrowserManager.SendTransaction(walletType, id, contractAddress, entryPoint, callData);

        public override bool CheckWalletConnection()
            => GodotBrowserManager.CheckWalletConnection();

        public override string GetAccountInformation()
            => GodotBrowserManager.GetAccountInformation();

        public override void WaitUntil(int id, Action<string> successCallback, Action<string> failCallback, Func<bool> predicate, Action<int, Action<string>, Action<string>> action)
        {
            CustomWaitUntil(predicate, id, successCallback, failCallback, action);
        }

        public void CustomWaitUntil(Func<bool> predicate, int id, Action<string> successCallback, Action<string> failCallback, Action<int, Action<string>, Action<string>> action)
        {
            GodotBrowserManager.Instance.UpdateTimer(predicate, id, successCallback, failCallback, action);
        }

        public override void DebugMessage(string message) => GodotBrowserManager.DebugMessage(message);
    }
 }
