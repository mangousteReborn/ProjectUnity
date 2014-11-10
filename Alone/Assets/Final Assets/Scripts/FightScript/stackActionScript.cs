using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Final_Assets.Scripts.Class;

public class stackActionScript : MonoBehaviour {

    private List<StackSchemClass> stackAction;

	// Use this for initialization
	void Start () {
        stackAction = new List<StackSchemClass>();
	}

    public void addAction(StackSchemClass schem)
    {
        stackAction.Add(schem);
    }

    void startStack()
    {
        foreach (StackSchemClass schem in stackAction)
        {
            schem.Script.fireAction(schem.Param);
            wait(schem.Time);
        }
    }

    IEnumerator wait(float time)
    {
        yield return new WaitForSeconds(time);
    }

	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Space))
            startStack();
	}
}
