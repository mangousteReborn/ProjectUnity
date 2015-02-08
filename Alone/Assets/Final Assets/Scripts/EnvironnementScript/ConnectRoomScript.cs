using UnityEngine;
using System.Collections;

public class ConnectRoomScript : MonoBehaviour {

    [SerializeField]
    private GameObject EntryCollider;

    [SerializeField]
    private GameObject ExitSpawn;

    [SerializeField]
    private GameObject handler;

    private bool isOpen = true;
    private GameManager gm;

	// Use this for initialization
	void Start () {
        gm = handler.GetComponent<GameManager>();
        EntryCollider.AddComponent<fireCollisionEventScript>().setFireScript(this);
	}
	
    public void onCollisionEnter(Collision collision)
    {
        GameObject obj = collision.gameObject;
        if(obj.tag == "Player" && isOpen)
        {
            var playerList = GameData.getPlayerList();
            int i = 0;
            foreach (Player player in playerList)
            {
                NavMeshAgent nav = player.playerObject.GetComponent<NavMeshAgent>();
                if (nav != null)
                    nav.enabled = false;
                player.playerObject.transform.position = ExitSpawn.transform.position + new Vector3(i * 0.2f,0,0);
                if (nav != null)
                    nav.enabled = true;
                i++;
            }
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
                gm.enterFightMode(player.id);//view.GetComponent<playerCaracScript>().enterFight(id);
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
