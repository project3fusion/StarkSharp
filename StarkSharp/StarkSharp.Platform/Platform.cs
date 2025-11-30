using System;
using System.Numerics;
using StarkSharp.Connectors.Components;
using StarkSharp.Core.Interfaces;
using StarkSharp.Rpc;
using StarkSharp.Tools.Notification;

namespace StarkSharp.Platforms
{
    /// <summary>
    /// Base platform class implementing IPlatform interface
    /// </summary>
    public abstract class Platform : IPlatform
    {
        public abstract PlatformName PlatformName { get; }
        public abstract PlatformConnectorType ConnectorType { get; }

        public virtual void ConnectWallet(string walletType, int id) { }
        public virtual void SendTransaction(string walletType, int id, string contractAddress, string entryPoint, string callData) { }
        public virtual void SendTransaction(TransactionInteraction transactionInteraction, Action<JsonRpcResponse> successCallback, Action<JsonRpcResponse> errorCallback) { }
        public virtual void CallContract(ContractInteraction contractInteraction, Action<string> successCallback, Action<string> errorCallback) { }
        public virtual void WaitUntil(int id, Action<string> successCallback, Action<string> failCallback, Func<bool> predicate, Action<int, Action<string>, Action<string>> action) { }
        public virtual bool CheckWalletConnection() { return true; }
        public virtual string GetAccountInformation() { return string.Empty; }
        public virtual void DebugMessage(string message) { }
        public virtual void PlatformRequest(JsonRpc requestData, Action<JsonRpcResponse> callback) { }
        public virtual void PlatformLog(string logMessage, NotificationType notifyType) { }

        // Backward compatibility - deprecated, use SendTransaction with TransactionInteraction instead
        [Obsolete("Use SendTransaction(TransactionInteraction, ...) instead")]
        public virtual void SendTransaction(Platform platform, TransactionInteraction transactionInteraction, Action<JsonRpcResponse> successCallback, Action<JsonRpcResponse> errorCallback)
        {
            SendTransaction(transactionInteraction, successCallback, errorCallback);
        }
    }
}