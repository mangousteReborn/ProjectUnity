using UnityEngine;
using System.Collections;

public class ConnectRoomScript : MonoBehaviour {

    [SerializeField]
    private GameObject EntryCollider;

    [SerializeField]
    private GameObject ExitSpawn;

	// Use this for initialization
	void Start () {
        EntryCollider.AddComponent<fireCollisionEventScript>().setFireScript(this);
	}
	
    public void onCollisionEnter(Collision collision)
    {
        Debug.Log("Collision");
        GameObject obj = collision.gameObject;
        if(obj.tag == "Player")
        {
            Debug.Log("TP");
            NavMeshAgent nav = obj.GetComponent<NavMeshAgent>();
            if (nav != null)
                nav.enabled = false;
            collision.gameObject.transform.position = ExitSpawn.transform.position;
            if (nav != null)
                nav.enabled = true;
        }
    }
}
