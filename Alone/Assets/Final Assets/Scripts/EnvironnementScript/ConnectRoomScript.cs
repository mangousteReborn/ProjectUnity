using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConnectRoomScript : MonoBehaviour {

    [SerializeField]
    private GameObject EntryCollider;

    [SerializeField]
    private List<ListSpwan> ExitSpawn;

    [SerializeField]
    private GameObject handler;

    private bool isOpen = true;
    private GameManager gm;
    private int currentIndex;

	// Use this for initialization
	void Start () {
        gm = handler.GetComponent<GameManager>();
        EntryCollider.AddComponent<fireCollisionEventScript>().setFireScript(this);
        currentIndex = 0;
	}
	
    public void onCollisionEnter(Collision collision)
    {
        GameObject obj = collision.gameObject;
        if(obj.tag == "Player" && isOpen && currentIndex < ExitSpawn.Count)
        {
            var playerList = GameData.getPlayerList();
            int i = 0;
            foreach (Player player in playerList)
            {
                NavMeshAgent nav = player.playerObject.GetComponent<NavMeshAgent>();
                if (nav != null)
                    nav.enabled = false;
                player.playerObject.transform.position = ExitSpawn[currentIndex].spwan[i].transform.position;
                CameraMovementScriptMouse cam = Camera.main.GetComponent<CameraMovementScriptMouse>();
                if (cam != null)
                {
                    cam.lockCamera = true;
                    cam.replaceRestictArea(ExitSpawn[currentIndex].spwan[i].transform.parent.gameObject);
                }
                if (nav != null)
                    nav.enabled = true;
                i++;
            }
            setIsOpen(false);
            currentIndex++;
            if (Network.isServer)
                fireCombat();
        }
    }

    private void fireCombat()
    {
        var playerList = GameData.getPlayerList();
        foreach (Player player in playerList)
        {
            if (!Network.isServer)
                gm.networkView.RPC("enterFightMode", RPCMode.Server, player.id);
            else
                gm.enterFightMode(player.id);
        }
    }

    public void setIsOpen(bool value)
    {
        this.isOpen = value;
        if(isOpen)
        {
            EntryCollider.renderer.enabled = false;
        }
        else
        {
            EntryCollider.renderer.enabled = true;
        }
    }
}
