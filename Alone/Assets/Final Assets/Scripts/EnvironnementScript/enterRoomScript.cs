using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class enterRoomScript : MonoBehaviour {

    [SerializeField]
    GameObject handler;

    GameManager gm;

	bool _active;
    List<NetworkViewID> listEnter;
    StaticVariableScript setting;

	// Use this for initialization
	void Start () {
		_active = true;
        listEnter = new List<NetworkViewID>();
        setting = new StaticVariableScript();
        gm = handler.GetComponent<GameManager>();
	}

    void OnTriggerEnter(Collider other)
    {
		if(!_active)
			return;

        if (!listEnter.Contains(other.gameObject.networkView.viewID))
        {
            listEnter.Add(other.gameObject.networkView.viewID);
            var playerList = GameData.getPlayerList();
            if (playerList.Count == listEnter.Count)
            {
                foreach (Player player in playerList)
                {
                    if (!Network.isServer)
                        gm.networkView.RPC("enterFightMode", RPCMode.Server, player.id);
                    else
                        gm.enterFightMode(player.id);//view.GetComponent<playerCaracScript>().enterFight(id);
                }
				_active = false;
            }
        }
    }
}
