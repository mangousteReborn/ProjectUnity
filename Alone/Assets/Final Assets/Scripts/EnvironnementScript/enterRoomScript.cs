using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class enterRoomScript : MonoBehaviour {

    List<NetworkViewID> listEnter;
    StaticVariableScript setting;

	// Use this for initialization
	void Start () {
        listEnter = new List<NetworkViewID>();
        setting = new StaticVariableScript();
	}

    void OnTriggerEnter(Collider other)
    {
        if (!listEnter.Contains(other.gameObject.networkView.viewID))
        {
            listEnter.Add(other.gameObject.networkView.viewID);
            var playerList = GameData.getPlayerList();
            if (playerList.Count == listEnter.Count)
            {
                NetworkView view;
                foreach (Player player in playerList)
                {
                    view = NetworkView.Find(player.id);
                    if (!Network.isServer)
                        view.RPC("enterFightMode", RPCMode.Server, player.id);
                    else
                        view.GetComponent<CharacterManager>().enterFightMode(player.id);//view.GetComponent<playerCaracScript>().enterFight(id);
                }
            }
        }
    }
}
