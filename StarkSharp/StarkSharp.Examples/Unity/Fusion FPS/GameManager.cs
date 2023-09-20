using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager {
    public static Connector connector, rpcConnector;
    public static Platform platform, rpcPlatform;

    public void GetPlayerLevel()
    {
        string contractAddress = "";
        string entryPoint = "";
        string callDataJson = "";

        rpcConnector.CallContract(
            new List<string> { contractAddress, entryPoint, callDataJson },
            (successMessage) => OnGetPlayerLevelCallContractSuccess(successMessage),
            (errorMessage) => OnGetPlayerLevelCallContractError(errorMessage));
    }

    public int playerLevel;

    public void EnterGame(){
        GetPlayerLevel();
    }

    public void OnGetPlayerLevelCallContractSuccess(string message){
        connector.DebugMessage(message);
        //If call contract is successfull then we can get the player level
        playerLevel = int.TryParse(message);
        //Load scene based on the player level
        SceneManager.LoadScene("GameLevel" + playerLevel);
    }

    public void OnGetPlayerLevelCallContractError(string message){
        connector.DebugMessage(message);
    }
}
