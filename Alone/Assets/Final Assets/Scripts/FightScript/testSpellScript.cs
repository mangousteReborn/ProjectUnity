using UnityEngine;
using System.Collections;
using Assets.Final_Assets.Scripts.Class;

public class testSpellScript : MonoBehaviour, ActionInterface {

    public GameObject spellIndication;

    private GameObject instanceObject;
    public float timeIncantation = 120;

    private stackActionScript stack;

    public delegate void ActionEventImplemente(Vector3 pos);
    public event ActionEventImplemente action;

    void Start()
    {
        stack = gameObject.GetComponent<stackActionScript>();
    }

    void OnEnable()
    {
        instanceObject = (GameObject)Instantiate(spellIndication);
    }
	
	// Update is called once per frame
	void Update () {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 mouseLocation;
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            mouseLocation = hit.point;
            mouseLocation.y = 0.1f;
            if(instanceObject.transform.position != mouseLocation)
                instanceObject.transform.position = mouseLocation;
            if (Input.GetMouseButtonDown(0))
            {
                object[] obj = new object[]{ mouseLocation };
                StackSchemClass schem = new StackSchemClass(this, obj, this.timeIncantation);
                stack.addAction(schem);
                action += attack;
                this.enabled = false;
            }
        }
	}

    public void fireAction(object[] param)
    {
        Vector3 pos = (Vector3)param[0];
        action(pos);
    }

    void attack(Vector3 pos)
    {
        Debug.Log("boooom at : " + pos);
    }

    void OnDisable()
    {
        Destroy(instanceObject);
    }
}
