using UnityEngine;
using System.Collections;

public class fireCollisionEventScript : MonoBehaviour {

    private MonoBehaviour fireScript;

	// Use this for initialization
	void Start () {
	}
	
    public void setFireScript(MonoBehaviour script)
    {
        this.fireScript = script;
    }

	// Update is called once per frame
    void OnCollisionEnter(Collision collision)
    {
        if (this.fireScript != null)
            ((ConnectRoomScript)fireScript).onCollisionEnter(collision);
    }
}
