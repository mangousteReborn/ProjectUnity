using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LobbyMenuScript : MonoBehaviour {

    public Text prefabText;

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    void OnPlayerConnected(NetworkPlayer player)
    {
        Text playerName = (Text)Network.Instantiate(prefabText,Vector3.zero,Quaternion.identity,0);
        playerName.text = "toto";
    }
}
