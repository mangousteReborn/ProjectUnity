using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class setIpScript : MonoBehaviour {

    public StaticVariableScript script;

    public InputField nameInputField = null;

    public Button submitButton = null;

    public LobbyScript lobby;
    public Canvas lobbyCanvas;
    public Canvas myCanvas;

    private void Start()
    {
        //nameInputField.onSubmit.AddListener((value) => SubmitName(value));
        submitButton.onClick.AddListener(() => SubmitName(nameInputField.value));
    }

    private void SubmitName(string ip)
    {
        script.setIp(ip);
        if (StaticVariableScript.isServer)
        {
            if (script.createServer())
            {
                myCanvas.gameObject.SetActive(false);
                lobbyCanvas.gameObject.SetActive(true);
                lobby.addPlayer();
            }
        }
        else
        {
            script.ConnectToServer();
        }
    }

    void OnConnectedToServer()
    {
        myCanvas.gameObject.SetActive(false);
        lobbyCanvas.gameObject.SetActive(true);
        networkView.RPC("newPlayerConnected", RPCMode.All, null);
    }
}
