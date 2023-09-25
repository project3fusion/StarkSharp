using System;
using System.Collections.Generic;
using StarkSharp.Accounts;
using StarkSharp.Connector.Components;
using StarkSharp.Connectors.Components;
using StarkSharp.Platforms;

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

        public virtual void SendTransaction(List<string> sendTransactionData, Action<string> successCallback, Action<string> failCallback)
        {
            int id = ConnectorTask.CreateNewTask();

            ConnectorSendTransaction(id, sendTransactionData[0], sendTransactionData[1], sendTransactionData[2]);

            ConnectorWaitUntil(id, successCallback, failCallback, ConnectorEventPredicate(id));
        }

        public virtual void CallContract(ContractInteraction contractInteraction, Action<string> successCallback, Action<string> failCallback)
        {
            int id = ConnectorTask.CreateNewTask();

            ConnectorCallContract(id, contractInteraction.ContractAdress, contractInteraction.EntryPoint, contractInteraction.CallData, successCallback, failCallback);

            ConnectorWaitUntil(id, successCallback, failCallback, ConnectorEventPredicate(id));
        }

        public virtual void ConnectorWaitUntil(int id, Action<string> successCallback, Action<string> failCallback, Func<bool> predicate) => platform.WaitUntil(id, successCallback, failCallback, predicate, ConnectorEventTrigger);

        public virtual Func<bool> ConnectorEventPredicate(int id) => () => ConnectorCheckWalletConnectionStatus() && ConnectorTask.GetStatus(id) != 0;

        public virtual Func<bool> ConnectorConnectEventPredicate(int id) => () => ConnectorCheckWalletConnectionStatus() || ConnectorTask.GetStatus(id) != 0;

        public virtual void ConnectorConnectWallet(string walletType, int id) => platform.ConnectWallet(walletType, id);

        public virtual void ConnectorSendTransaction(int id, string contractAddress, string entryPoint, string callData) => platform.SendTransaction(walletType.ToString(), id, contractAddress, entryPoint, callData);

        public virtual void ConnectorCallContract(int id ,string contractAddress, string entryPoint, string callData, Action<string> successCallback, Action<string> failCallback)
        {
            List<string> callDataList = new List<string> { contractAddress, entryPoint, callData };

            platform.CallContract(callDataList,
                                  message => ConnectorOnCallContractSucceeded(successCallback, message),
                                  message => ConnectorOnCallContractFailed(failCallback, message));
        }


        public virtual bool ConnectorCheckWalletConnectionStatus() => platform.CheckWalletConnection();

        public virtual void ConnectorEventTrigger(int id, Action<string> successCallback, Action<string> failCallback)
        {
            if (ConnectorCheckWalletConnectionStatus() && ConnectorTask.GetStatus(id, isRemove: true) == 1) ConnectorOnWalletConnectionSucceeded(successCallback, ConnectorTask.GetMessage(id));
            else ConnectorOnWalletConnectionFailed(failCallback, ConnectorTask.GetMessage(id));
        }

        public virtual void ConnectorOnWalletConnectionSucceeded(Action<string> callback, string message)
        {
            if (message == WalletType.ArgentX.ToString()) walletType = WalletType.ArgentX;
            else if (message == WalletType.Braavos.ToString()) walletType = WalletType.Braavos;
            account.WalletAdress = platform.GetAccountInformation();
            callback(message);
        }

        public virtual void ConnectorOnWalletConnectionFailed(Action<string> callback, string message) => callback(message);

        public virtual void ConnectorOnSendTransactionSucceeded(Action<string> callback, string message) => callback(message);

        public virtual void ConnectorOnSendTransactionFailed(Action<string> callback, string message) => callback(message);

        public virtual void ConnectorOnCallContractSucceeded(Action<string> callback, string message) => callback(message);

        public virtual void ConnectorOnCallContractFailed(Action<string> callback, string message) => callback(message);

        public virtual void DebugMessage(string message) => platform.DebugMessage(message);
    }
}