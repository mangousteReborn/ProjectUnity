using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class fastConnectionScript : MonoBehaviour {

    [SerializeField]
    private GameObject playerPrefab;

    [SerializeField]
    private GameObject gmEmpty;

    [SerializeField]
    private GameObject[] spawnPoint;

    [SerializeField]
    private GameObject gmSpawnPoint;

    [SerializeField]
    private bool _isGM;

    private StaticVariableScript setting;

	// Use this for initialization
	void Start () {
        Application.runInBackground = true;
        MasterServer.ClearHostList();
        MasterServer.RequestHostList("MyUnityProject");
        StartCoroutine(wait5Second());
        GameData.init();
	}

    IEnumerator wait5Second()
    {
        yield return new WaitForSeconds(2);
        HostData[] data = MasterServer.PollHostList();
        if (data.Length == 0)
        {
            Network.InitializeServer(3, 8080, !Network.HavePublicAddress());
            MasterServer.RegisterHost("MyUnityProject", "DefaultGameFastConnection", "");
            OnConnectedToServer();
        }
        else
        {
            Network.Connect(data[0]);
        }
    }

    void OnConnectedToServer()
    {
        if (Network.isServer)
            instantiateMyPlayer();
        else
            networkView.RPC("requestPlayerList", RPCMode.Server, Network.player);
    }

    void instantiateMyPlayer()
    {
        if (!_isGM)
        {
            int playerConnected = GameData.getPlayerList().Count;
            GameObject spawnPos = spawnPoint[playerConnected];
            GameObject newPlayer = (GameObject)Network.Instantiate(playerPrefab, spawnPos.transform.position, Quaternion.identity, 1);


            // Setting values for player GUI
            GameManager manager = this.GetComponent<GameManager>();
            manager.setPlayerGUI();
            IPlayerGUI gui = manager.playerGUI;
            CharacterManager cm = newPlayer.GetComponent<CharacterManager>();

            Player p = new Player("Player", newPlayer.networkView, Color.green);
            p.characterManager = cm;
            p.playerObject = newPlayer;
            GameData.addPlayer(p);

            cm.player = p;

            gui.setCharacterManager(cm);

            newPlayer.GetComponent<DeplacementActionScript>().enabled = true;
            //setting.ListPlayer.Add(newPlayer.networkView.viewID);
            Camera.main.GetComponent<CameraMovementScriptMouse>().enabled = true;
            networkView.RPC("addPlayer", RPCMode.Others, newPlayer.networkView.viewID);
        }
        else
        {
            this.GetComponent<GameManager>().setGMGui();
            Vector3 spawnPos = gmSpawnPoint.transform.position;
            GameObject newPlayer = (GameObject)Network.Instantiate(gmEmpty, spawnPos, Quaternion.identity, 1);
            spawnPos.y = Camera.main.transform.position.y;
            Camera.main.transform.position = spawnPos;
            CameraMovementScriptMouse camScript = Camera.main.GetComponent<CameraMovementScriptMouse>();
            camScript.enabled = true;
            camScript.setJoueur = newPlayer.transform;
            camScript.cameraCanBeLocked = false;
            camScript.lockCamera = false;
        }
    }

    [RPC]
    void requestPlayerList(NetworkPlayer player)
    {
        if (Network.isServer)
        {
            List<Player> playerList = GameData.getPlayerList();
            foreach (Player playerData in playerList)
                networkView.RPC("addPlayer", player, playerData.id);
            networkView.RPC("addClientPlayer", player);
        }
    }

    [RPC]
    void addClientPlayer()
    {
        instantiateMyPlayer();
    }

	[RPC]
	void addPlayer(NetworkViewID networkViewID)
	{
		CharacterManager cm = NetworkView.Find(networkViewID).gameObject.GetComponent<CharacterManager>();
		Player p = new Player("Player", NetworkView.Find(networkViewID).gameObject.networkView, Color.green);
		p.characterManager = cm;
		p.playerObject = NetworkView.Find(networkViewID).gameObject;
		GameData.addPlayer(p);
		cm.player = p;
	}

    void DestroyPlayer(GameObject player)
    {
        Network.Destroy(player);
        Network.RemoveRPCs(player.networkView.viewID);
    }

    void OnDisconnectedFromServer(NetworkDisconnection info)
    {
        DestroyPlayer(gameObject);
    }

    void OnApplicationQuit()
    {
        Network.Disconnect();
        MasterServer.UnregisterHost();
    }
}
