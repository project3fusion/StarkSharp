using StarkSharp.Connectors.Components;
using System.Runtime.InteropServices;
using UnityEngine;

namespace StarkSharp.Platforms.Unity.WebGL
{
    public class UnityBrowserManager : MonoBehaviour
    {
        public static UnityBrowserManager Instance;

        [DllImport("__Internal")]
        public static extern void DebugMessage(string message);

        [DllImport("__Internal")]
        public static extern bool CheckWalletConnection();

        [DllImport("__Internal")]
        public static extern string GetAccountInformation();

        [DllImport("__Internal")]
        public static extern void ConnectWallet(string walletType, int id, string callbackObjectName, string callbackMethodName);

        [DllImport("__Internal")]
        public static extern void SendTransaction(string walletType, int id, string contractAddress, string entryPoint, string callData, string callbackObjectName, string callbackMethodName);

        private void Awake() => DontDestroyOnLoad(Instance = this);

        public void RecieveMessage(string data) => ConnectorTask.RecieveMessage(data);
    }
}
