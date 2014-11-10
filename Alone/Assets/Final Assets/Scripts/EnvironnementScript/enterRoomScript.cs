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
            if(setting.ListPlayer.Count == listEnter.Count)
            {
                NetworkView view;
                foreach(NetworkViewID id in setting.ListPlayer)
                {
                    view = NetworkView.Find(id);
                    if (!Network.isServer)
                        view.RPC("enterFight", RPCMode.Server, id);
                    else
                        view.GetComponent<playerCaracScript>().enterFight(id);
                }
            }
        }
    }
}
