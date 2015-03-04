using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LobbyScript : MonoBehaviour {

    public Text lobbyText;
    public Button startGameButton;

    private string displayedText = "Waiting for other player : ";
    private int numberPlayer = 0;

    public void addPlayer()
    {
        numberPlayer += 1;
        if (numberPlayer > 3)
            startGameButton.interactable = true;
        else
            startGameButton.interactable = false;
        this.lobbyText.text = displayedText + " " + numberPlayer + " / 4"; 
    }

    [RPC]
    private void newPlayerConnected()
    {
        if(Network.isServer)
        {
            addPlayer();
            networkView.RPC("sendNumberPlayer", RPCMode.Others, numberPlayer);
        }
    }

    [RPC]
    private void sendNumberPlayer(int numberPlayer)
    {
        this.numberPlayer = numberPlayer;
        this.lobbyText.text = displayedText + " " + numberPlayer + " / 4";
        if(StaticVariableScript.numberClient == -1)
            StaticVariableScript.numberClient = numberPlayer - 2;
    }

    public void startGame()
    {
        Application.LoadLevel("mapOneScene");
        if(Network.isServer)
        {
            networkView.RPC("startGameRPC", RPCMode.Others, null);
        }
    }

    [RPC]
    private void startGameRPC()
    {
        Application.LoadLevel("mapOneScene");
    }
}
