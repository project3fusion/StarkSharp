using System;
using StarkSharp.Connectors.Components;
using StarkSharp.Rpc;
using StarkSharp.Tools.Notification;

namespace StarkSharp.Core.Interfaces
{
    /// <summary>
    /// Platform abstraction interface for different platform implementations
    /// </summary>
    public interface IPlatform
    {
        PlatformName PlatformName { get; }
        PlatformConnectorType ConnectorType { get; }

        void ConnectWallet(string walletType, int id);
        void SendTransaction(string walletType, int id, string contractAddress, string entryPoint, string callData);
        void SendTransaction(TransactionInteraction transactionInteraction, Action<JsonRpcResponse> successCallback, Action<JsonRpcResponse> errorCallback);
        void CallContract(ContractInteraction contractInteraction, Action<string> successCallback, Action<string> errorCallback);
        void WaitUntil(int id, Action<string> successCallback, Action<string> failCallback, Func<bool> predicate, Action<int, Action<string>, Action<string>> action);
        bool CheckWalletConnection();
        string GetAccountInformation();
        void DebugMessage(string message);
        void PlatformRequest(JsonRpc requestData, Action<JsonRpcResponse> callback);
        void PlatformLog(string logMessage, NotificationType notifyType);
    }

    public enum PlatformName
    {
        CryEngine,
        Unity,
        Godot,
        Dotnet,
        AspNet,
        WinForms
    }

    public enum PlatformConnectorType
    {
        WebGL,
        Sharpion,
        HTML5,
        RPC
    }
}

