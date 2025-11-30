using System;
using StarkSharp.Accounts;
using StarkSharp.Connectors.Components;
using StarkSharp.Rpc;

namespace StarkSharp.Core.Interfaces
{
    /// <summary>
    /// Connector interface for wallet and transaction operations
    /// </summary>
    public interface IConnector
    {
        Account Account { get; }
        WalletType WalletType { get; }
        PlatformConnectorType ConnectorType { get; }

        void ConnectWallet(WalletType walletType, Action<string> successCallback, Action<string> failCallback);
        void SendTransaction(TransactionInteraction transactionInteraction, Action<JsonRpcResponse> successCallback, Action<JsonRpcResponse> failCallback);
        void CallContract(ContractInteraction contractInteraction, Action<string> successCallback, Action<string> failCallback);
        void DebugMessage(string message);
    }
}

