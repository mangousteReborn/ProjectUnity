using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class fastConnectionScript : MonoBehaviour {

    public bool localClientServer = false;

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

    [SerializeField]
    private Material _materialGameMaster;

	[SerializeField]
	private Material[] _materialArray;

    private StaticVariableScript setting;

	private static int _playerColorIndex = 1; // 0 will be default

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
        if (data.Length == 0 && !localClientServer)
        {
            Network.InitializeSecurity();
            Network.InitializeServer(3, 9090, !Network.HavePublicAddress());
            //MasterServer.RegisterHost("MyUnityProject", "DefaultGameFastConnection", "");
            OnConnectedToServer();
        }
        else
        {
            //Network.Connect(data[0]);
            Network.Connect("127.0.0.1", 9090);
        }
    }

    void OnFailedToConnect(NetworkConnectionError error)
    {
        Debug.Log("Could not connect to server: " + error);
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
            GameObject newPlayer = (GameObject)Network.Instantiate(playerPrefab, spawnPos.transform.position, Quaternion.identity, 10);

			CharacterManager cm = newPlayer.GetComponent<CharacterManager>();

            // Setting values for player GUI
            GameManager manager = this.GetComponent<GameManager>();
            IPlayerGUI gui = GameData.getGameManager().instanciateAndGetPlayerGUI();

            

			// Material for player's unit color
			Material mat = _playerColorIndex <= _materialArray.Length ? _materialArray[_playerColorIndex] : _materialArray[0];

			// Logic Stats
			CharacterStats stats = new CharacterStats(cm.networkView, 100, 10f, 10);
			stats.addTargetType(CharacterStats.TargetType.mine);
			stats.addTargetType(CharacterStats.TargetType.player);

			// Player (data)
			Player p = new Player("Player_"+_playerColorIndex, newPlayer.networkView,false,gui);
            p.characterManager = cm;
            p.playerObject = newPlayer;
           


			// CharacManager Step 2 : Instantiate (logic data)
			cm.initialize(stats,p,mat);
			gui.setOwner(p);

            newPlayer.GetComponent<DeplacementActionScript>().enabled = true;
            //setting.ListPlayer.Add(newPlayer.networkView.viewID);
            Camera.main.GetComponent<CameraMovementScriptMouse>().enabled = true;

            networkView.RPC("addPlayer", RPCMode.Others, newPlayer.networkView.viewID);
			GameData.addPlayer(p);

			_playerColorIndex ++;
        }
        else
        {
			GameManager manager = this.GetComponent<GameManager>();
			IPlayerGUI gui = GameData.getGameManager().instanciateAndGetGameMasterGUI();

            Vector3 spawnPos = gmSpawnPoint.transform.position;
            GameObject newPlayer = (GameObject)Network.Instantiate(gmEmpty, spawnPos, Quaternion.identity, 1);
            spawnPos.y = Camera.main.transform.position.y;
            Camera.main.transform.position = spawnPos;
            //
            CameraMovementScriptMouse camScript = Camera.main.GetComponent<CameraMovementScriptMouse>();
            camScript.enabled = true;
            camScript.setJoueur = newPlayer.transform;
            camScript.cameraCanBeLocked = false;
            camScript.lockCamera = false;
            gameObject.GetComponent<InstantiateNPCScript>().enabled = true;

            CharacterManager cm = newPlayer.GetComponent<CharacterManager>();

            Player p = new Player("GM", newPlayer.networkView, true, gui);
            p.characterManager = cm;
            p.playerObject = newPlayer;

			gui.setOwner(p);
            CharacterStats carac = new CharacterStats(newPlayer.networkView, 99999, 0, 0);

            cm.initialize(carac, p, _materialGameMaster);

            GameData.setGameMasterPlayer(p);
            networkView.RPC("setGameMasterPlayerRPC", RPCMode.Others, newPlayer.networkView.viewID, 99999,0.0f,0);
        }
    }


    [RPC]
    void setGameMasterPlayerRPC(NetworkViewID id,int health, float actionPoint, int strength)
    {
        GameObject newPlayer = NetworkView.Find(id).gameObject;
        CharacterManager cm = newPlayer.GetComponent<CharacterManager>();
        Player p = new Player("GM", newPlayer.networkView, true);
        CharacterStats carac = new CharacterStats(newPlayer.networkView, health, actionPoint, strength);
        cm.initialize(carac, p, _materialGameMaster);
        p.characterManager = cm;
        p.playerObject = newPlayer;
        GameData.setGameMasterPlayer(p);
    }


    [RPC]
    void requestPlayerList(NetworkPlayer player)
    {
        if (Network.isServer)
        {
            List<Player> playerList = GameData.getPlayerList();
            foreach (Player playerData in playerList)
                networkView.RPC("addPlayer", player, playerData.id);
            if (GameData.getGameMasterPlayer() != null)
            {
                Player p = GameData.getGameMasterPlayer();
                CharacterStats stats = p.characterManager.characterStats;
                networkView.RPC("setGameMasterPlayerRPC", player, p.id, stats.maxLife, stats.maxActionPoint, stats.maxStrength);
            }
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
		Material mat = _playerColorIndex <= _materialArray.Length ? _materialArray[_playerColorIndex] : _materialArray[0];


		Player p = new Player("Player"+_playerColorIndex, NetworkView.Find(networkViewID).gameObject.networkView,false);
		p.characterManager = cm;
		p.playerObject = NetworkView.Find(networkViewID).gameObject;
		GameData.addPlayer(p);
		
		// Logic Stats
		CharacterStats stats = new CharacterStats(cm.networkView, 100, 10f, 10);
		stats.addTargetType(CharacterStats.TargetType.ally);
		stats.addTargetType(CharacterStats.TargetType.player);

		
		// CharacManager Step 2 : Instantiate (logic data)
		cm.initialize(stats,p,mat);

		Debug.Log ("Player : " + p.name + " Is : " + stats.targetTypesToString ());
		_playerColorIndex ++;
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
