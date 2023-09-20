using System;
using System.Collections;
using UnityEngine;

namespace StarkSharp.Platforms.Unity.WebGL
{
    public class UnityWebGLPlatform : UnityPlatform
    {
        public override void ConnectWallet(string walletType, int id)
            => UnityBrowserManager.ConnectWallet(walletType, id, "BrowserManager", "RecieveMessage");

        public override void SendTransaction(string walletType, int id, string contractAddress, string entryPoint, string callData)
            => UnityBrowserManager.SendTransaction(walletType, id, contractAddress, entryPoint, callData, "BrowserManager", "RecieveMessage");


        public override bool CheckWalletConnection()
            => UnityBrowserManager.CheckWalletConnection();

        public override string GetAccountInformation()
            => UnityBrowserManager.GetAccountInformation();

        public override void WaitUntil(int id, Action<string> successCallback, Action<string> failCallback, Func<bool> predicate, Action<int, Action<string>, Action<string>> action)
        {
            IEnumerator CustomWaitUntil()
            {
                yield return new WaitUntil(() => predicate());
                action(id, successCallback, failCallback);
            }
            UnityBrowserManager.Instance.StartCoroutine(CustomWaitUntil());
        }

        public override void DebugMessage(string message) => UnityBrowserManager.DebugMessage(message);
    }
}
