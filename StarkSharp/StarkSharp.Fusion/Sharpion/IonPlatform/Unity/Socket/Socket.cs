using System.Numerics;
using System.Text;

using StarkSharp.Fusion.Sharpion.Unity.Handlers;
using WebSocketSharp;

using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

using UnityEngine;
using System.Diagnostics;
using System;

namespace StarkSharp.Fusion.Sharpion.Unity
{
    public class Socket : MonoBehaviour
    {
        public static Socket instance;
        public static WebSocket ws;
        public static int SocketClientID;
        public static string SocketSessionToken;
        public string UserWalletAddress;
        public string UserBalanceOfEth;

        private void Awake() => SetSocket();
        private void SetSocket()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        public void ConnectToServer()
        {
            ws = new WebSocket($"ws://{Settings.Settings.webSocketipandport}");
            ws.OnMessage += async (sender, e) => await Handler.HandShake(e.Data);
            ws.OnOpen += (sender, e) => UnityEngine.Debug.Log("SharpION Connection Open.");
            ws.OnClose += (sender, e) => UnityEngine.Debug.Log("SharpION Connection Close.");
            ws.OnError += (sender, e) => UnityEngine.Debug.Log($"SharpION Connection Error: {e.Message}");
            ws.Connect();
        }

        public void ConnectWallet(){
            SocketSessionToken = Guid.NewGuid().ToString();
            string url = $"{Settings.Settings.webSocketWebsiteDomain}?token={SocketSessionToken}";
            Process.Start(url); 
            instance.SendDataFromJson(JsonUtility.ToJson(Packs.CreateLoginPack(SocketSessionToken,false, false)));
        }
        public void DisconnectWallet() => instance.SendDataFromJson(JsonUtility.ToJson(Packs.CreateDisconnectPack(SocketClientID, false, false)));
        public void BalanceOfWallet(string WalletAdress) => instance.SendDataFromJson(JsonUtility.ToJson(Packs.CreateBalanceOfPack(SocketClientID, WalletAdress)));
        public void SendTransaction(string Receivingaddress, BigInteger amount) => instance.SendDataFromJson(JsonUtility.ToJson(Packs.CreateTransactionPack(SocketClientID, Receivingaddress, amount)));

        public void DisconnectFromServer()
        {
            if (ws != null && ws.ReadyState == WebSocketState.Open)
            {
                ws.Close();
                UnityEngine.Debug.Log("The WebSocket connection was closed manually.");
            }
        }

        public bool IsSocketAlive() => ws?.IsAlive ?? false;
		
        public void SendDataFromJson(string sendjson)
        {
            try
            {
                JToken.Parse(sendjson);
            }
            catch (JsonReaderException jex)
            {
                UnityEngine.Debug.Log("The provided string is not in a valid JSON format.");
                UnityEngine.Debug.Log(jex.Message); 
                return;
            }
            if (ws.ReadyState == WebSocketState.Open)
            {
                byte[] dataToSend = Encoding.UTF8.GetBytes(sendjson);
                ws.Send(dataToSend);
            }
        }
        private void OnDestroy()
        {
            if (ws != null) ws.Close();
        }       
    }
}
