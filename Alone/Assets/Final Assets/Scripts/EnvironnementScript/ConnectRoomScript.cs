using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConnectRoomScript : MonoBehaviour {

    [SerializeField]
    private GameObject EntryCollider;

    [SerializeField]
    private List<ListSpwan> ExitSpawn;

    public bool isExit;

    private bool isOpen = true;
	private bool active = true;
    private int currentIndex;

	// Use this for initialization
	void Start () {
        EntryCollider.AddComponent<fireCollisionEventScript>().setFireScript(this);
        currentIndex = 0;
	}
	
    public void onCollisionEnter(Collision collision)
    {
		if(!active)
			return;
		
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

				Vector3 nextPos = ExitSpawn[currentIndex].spwan[i].transform.position;
				player.playerObject.transform.position = nextPos;

				//player.characterManager.networkView.RPC ("moveToPoint", RPCMode.All, nextPos);


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

			active = false;
            if(!isExit)
			    GameData.getGameManager().networkView.RPC("initNextRound", RPCMode.All);

        }
    }


    public void setIsOpen(bool value)
    {
		active = value;
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
