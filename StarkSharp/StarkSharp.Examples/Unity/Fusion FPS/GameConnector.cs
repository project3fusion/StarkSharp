using UnityEngine;
using StarkSharp.Connectors;
using StarkSharp.Platforms.Unity;

public class GameConnector : Monobehaviour
{
    public static bool isTransactionSent;

    private void Start(){
        //Create platforms
        GameManager.platform = UnityPlatform.New(PlatformConnectorType.WebGL);
        GameManager.rpcPlatform = UnityPlatform.New(PlatformConnectorType.RPC);

        //Create connectors
        GameManager.connector = new Connector(GameManager.platform);
        GameManager.rpcConnector = new Connector(GameManager.rpcPlatform);

        //Connect ArgentX or Braavos
        GameManager.connector.ConnectWallet(WalletType.ArgentX,
            (successMessage) => OnWalletConnectionSuccess(successMessage),
            (errorMessage) => OnWalletConnectionError(errorMessage));

        /*
        GameManager.connector.ConnectWallet(WalletType.Braavos,
            (successMessage) => OnWalletConnectionSuccess(successMessage),
            (errorMessage) => OnWalletConnectionError(errorMessage));
        */
    }

    public void OnWalletConnectionSuccess(string message)
    {
        //Wallet connection successfull, enter to the game.
        GameManager.connector.DebugMessage("On Wallet Connection Success: " + message);
        GameManager.EnterGame();
    }

    public void OnWalletConnectionError(string message)
    {
        GameManager.connector.DebugMessage("On Wallet Connection Error: " + message);
    }
}
