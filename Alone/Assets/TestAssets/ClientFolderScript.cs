using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ClientFolderScript : MonoBehaviour {

    [SerializeField]
    private GameObject playerPrefab;

    [SerializeField]
    private GameObject camPrefab;

    [SerializeField]
    private GameObject lampPrefab;

    private StaticVariableScript setting;

	// Use this for initialization
	void Start () {
        setting = new StaticVariableScript();
        if (Network.isServer)
            instantiateMyPlayer();
        else
            networkView.RPC("requestPlayerList", RPCMode.Server, Network.player);
	}



    void instantiateMyPlayer()
    {
        GameObject newPlayer = (GameObject)Network.Instantiate(playerPrefab, Vector3.up, Quaternion.identity, 1);
        networkView.RPC("addPlayer", RPCMode.Others, newPlayer.networkView.viewID);
            
    }

    [RPC]
    void requestPlayerList(NetworkPlayer player)
    {
        if(Network.isServer)
        {
            foreach(NetworkViewID id in setting.ListPlayer)
                networkView.RPC("addPlayer",player, id);
            networkView.RPC("addClientPlayer", player);
        }
    }

    [RPC]
    void addClientPlayer()
    {
        instantiateMyPlayer();
    }

    [RPC]
    void addPlayer(NetworkViewID playerID)
    {
        setting.ListPlayer.Add(playerID);
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
}
