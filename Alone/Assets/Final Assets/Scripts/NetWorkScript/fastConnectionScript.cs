using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class fastConnectionScript : MonoBehaviour {

    [SerializeField]
    private GameObject playerPrefab;

    [SerializeField]
    private GameObject[] spawnPoint;

    private StaticVariableScript setting;

	// Use this for initialization
	void Start () {
        Application.runInBackground = true;
        MasterServer.ClearHostList();
        MasterServer.RequestHostList("MyUnityProject");
        StartCoroutine(wait5Second());

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
        setting = new StaticVariableScript();
        setting.ListPlayer = new List<NetworkViewID>();
        if (Network.isServer)
            instantiateMyPlayer();
        else
            networkView.RPC("requestPlayerList", RPCMode.Server, Network.player);
    }

    void instantiateMyPlayer()
    {
        int playerConnected = setting.ListPlayer.Count;
        GameObject spawnPos = spawnPoint[playerConnected];
        GameObject newPlayer = (GameObject)Network.Instantiate(playerPrefab, spawnPos.transform.position, Quaternion.identity, 1);
        

		// Setting values for player GUI
		IPlayerGUI gui = this.GetComponent<GameManager> ().playerGUI;
		CharacterManager cm = newPlayer.GetComponent<CharacterManager> ();

		Player p = new Player("Player", newPlayer.networkView, Color.green);
        GameData.addPlayer(p);

		cm.player = p;

		gui.setCharacterManager(cm);

		newPlayer.GetComponent<DeplacementActionScript>().enabled = true;
        setting.ListPlayer.Add(newPlayer.networkView.viewID);
        Camera.main.GetComponent<CameraMovementScriptMouse>().enabled = true;
        networkView.RPC("addPlayer", RPCMode.Others, newPlayer.networkView.viewID);
    }

    [RPC]
    void requestPlayerList(NetworkPlayer player)
    {
        if (Network.isServer)
        {
            foreach (NetworkViewID id in setting.ListPlayer)
                networkView.RPC("addPlayer", player, id);
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
        Player p = new Player("Player", NetworkView.Find(networkViewID).gameObject.networkView, Color.green);
        GameData.addPlayer(p);
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
