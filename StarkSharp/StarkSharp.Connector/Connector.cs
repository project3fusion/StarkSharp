using System;
using Newtonsoft.Json;
using StarkSharp.Accounts;
using StarkSharp.Connectors.Components;
using StarkSharp.Platforms;
using StarkSharp.Rpc;
using static StarkSharp.Platforms.Platform;

namespace StarkSharp.Connectors
{
    public class Connector
    {
        public Account account;
        public WalletType walletType;
        public Platform platform;
        public PlatformConnectorType connectorType;

        public Connector(Platform platform)
        {
            account = new Account();
            this.platform = platform;

        }

        public virtual void ConnectWallet(WalletType walletType, Action<string> successCallback, Action<string> failCallback)
        {
            int id = ConnectorTask.CreateNewTask();

            ConnectorConnectWallet(walletType.ToString(), id);

            ConnectorWaitUntil(id, successCallback, failCallback, ConnectorEventPredicate(id));
        }

        public virtual void ConnectorConnectWallet(string walletType, int id) => platform.ConnectWallet(walletType, id);

        public virtual void SendTransaction(TransactionInteraction transactionInteraction, Action<JsonRpcResponse> successCallback, Action<JsonRpcResponse> failCallback) => ConnectorSendTransaction(transactionInteraction, successCallback, failCallback);

        public virtual void CallContract(ContractInteraction contractInteraction, Action<string> successCallback, Action<string> failCallback) => ConnectorCallContract(contractInteraction, successCallback, failCallback);

        public virtual void ConnectorWaitUntil(int id, Action<string> successCallback, Action<string> failCallback, Func<bool> predicate) => platform.WaitUntil(id, successCallback, failCallback, predicate, ConnectorEventTrigger);

        public virtual Func<bool> ConnectorEventPredicate(int id) => () => ConnectorCheckWalletConnectionStatus();

        public virtual Func<bool> ConnectorConnectEventPredicate(int id) => () => ConnectorCheckWalletConnectionStatus();
        public virtual void ConnectorSendTransaction(TransactionInteraction transactionInteraction, Action<JsonRpcResponse> successCallback, Action<JsonRpcResponse> failCallback)
        {
            platform.SendTransaction(
                platform,
                 transactionInteraction,
                 successCallback,
                 failCallback
             );
        }

        public virtual void ConnectorCallContract(ContractInteraction contractInteraction, Action<string> successCallback, Action<string> failCallback)
        {
            platform.CallContract(contractInteraction, message => ConnectorOnCallContractSucceeded(successCallback, message), message => ConnectorOnCallContractFailed(failCallback, message));
        }

        public virtual bool ConnectorCheckWalletConnectionStatus() => platform.CheckWalletConnection();
        public virtual void ConnectorEventTrigger(int id, Action<string> successCallback, Action<string> failCallback) { }

        public virtual void ConnectorOnWalletConnectionSucceeded(Action<string> callback, string message)
        {
            if (message == WalletType.ArgentX.ToString()) walletType = WalletType.ArgentX;
            else if (message == WalletType.Braavos.ToString()) walletType = WalletType.Braavos;
            account.WalletAdress = platform.GetAccountInformation();
            callback(message);
        }

        public virtual void ConnectorOnSendTransactionFailed(Action<JsonRpcResponse> errorResponse, string message)
        {
            JsonRpcResponse jsonResponse = JsonConvert.DeserializeObject<JsonRpcResponse>(message);
            errorResponse?.Invoke(jsonResponse);
        }

        public virtual void ConnectorOnSendTransactionSucceeded(Action<JsonRpcResponse> successResponse, string message)
        {
            JsonRpcResponse jsonResponse = JsonConvert.DeserializeObject<JsonRpcResponse>(message);
            successResponse?.Invoke(jsonResponse);
        }

        public virtual void ConnectorOnSendTransactionFailed(Action<string> callback, string message) => callback(message);

        public virtual void ConnectorOnCallContractSucceeded(Action<string> callback, string message) => callback(message);

        public virtual void ConnectorOnCallContractFailed(Action<string> callback, string message) => callback(message);

        public virtual void DebugMessage(string message) => platform.DebugMessage(message);
    }
}