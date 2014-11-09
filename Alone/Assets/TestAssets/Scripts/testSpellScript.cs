using UnityEngine;
using System.Collections;

public class testSpellScript : MonoBehaviour, ActionInterface {

    public GameObject spellIndication;

    private GameObject instanceObject;
    public float timeIncantation = 120;

    private stackActionScript stack;

    public delegate void fireAction();
    public event fireAction action;

    void Start()
    {
        Debug.Log("start testSpell");
        stack = gameObject.GetComponent<stackActionScript>();
    }

    void OnEnable()
    {
        instanceObject = (GameObject)Instantiate(spellIndication);
    }
	
	// Update is called once per frame
	void Update () {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Vector3 mouseLocation = hit.point;
            mouseLocation.y = 0.1f;
            if(instanceObject.transform.position != mouseLocation)
                instanceObject.transform.position = mouseLocation;
        }
        if (Input.GetMouseButtonDown(0))
        {
            stack.addAction(this);
            action += attack;
            this.enabled = false;
        }
	}

    public void fireActionSpell()
    {
        action();
    }

    void attack()
    {
        Debug.Log("boooom");
    }

    void OnDisable()
    {
        Destroy(instanceObject);
    }
}
