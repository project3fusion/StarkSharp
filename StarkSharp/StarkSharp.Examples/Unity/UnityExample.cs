using System;
using UnityEngine;

public class UnityExample : Monobehaviour {

    public Platform platform;
    public Connector connector;
    bool isTransactionSent;

    private void Start(){
        //Create a new platform
        unityPlatform = UnityPlatform.New(PlatformConnectorType.WebGL);

        //Create a new connector
        Connector connector = new Connector(unityPlatform);

        //Connect ArgentX or Braavos
        connector.ConnectWallet(WalletType.ArgentX,
            (successMessage) => OnWalletConnectionSuccess(successMessage),
            (errorMessage) => OnWalletConnectionError(errorMessage));

        /*
        connector.ConnectWallet(WalletType.ArgentX,
            (successMessage) => OnWalletConnectionSuccess(successMessage),
            (errorMessage) => OnWalletConnectionError(errorMessage));
        */
    }

    public void OnWalletConnectionSuccess(string message)
    {
        connector.DebugMessage("On Wallet Connection Success: " + message);

        //If connection is successfull, send transaction

        string sendTransactionContractAddress = "";
        string sendTransactionRecipientAddress = "";
        string amount = "";

        connector.SendTransaction(
            ERC20Standart.TransferToken(sendTransactionContractAddress, sendTransactionRecipientAddress, amount),
            (successMessage) => OnSendTransactionSuccess(successMessage),
            (errorMessage) => OnSendTransactionError(errorMessage));
    }

    public void OnWalletConnectionError(string message)
    {
        connector.DebugMessage("On Wallet Connection Error: " + message);
    }

    public void OnSendTransactionSuccess(string message)
    {
        //If we send transaction let's set it to true.
        isTransactionSent = true;
        connector.DebugMessage("On Send Transaction Success: " + message);
    }

    public void OnSendTransactionError(string message)
    {
        connector.DebugMessage("On Send Transaction Error: " + message);
    }

    private void Update(){
        //If transaction is sent on wallet connected, let's call contract
        if(isTransactionSent){
            isTransactionSent = false;
            CheckBalance();
        }
    }

    public void CheckBalance()
    {
        //Create a new platform
        Platform unityRPCPlatform = UnityPlatform.New(PlatformConnectorType.RPC);

        //Create a new connector
        Connector rpcConnector = new Connector(unityRPCPlatform);

        //Call contract
        string callContractContractAddress = "";
        string callOtherUserWalletAddress = "";

        connector.CallContract(
            ERC20Standart.BalanceOf(callContractContractAddress, callOtherUserWalletAddress),
            (successMessage) => OnCallContractSuccess(successMessage),
            (errorMessage) => OnCallContractError(errorMessage));
    }
}