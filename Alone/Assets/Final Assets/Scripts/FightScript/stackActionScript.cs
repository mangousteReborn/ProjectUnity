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

    IEnumerator startStack()
    {
        foreach (StackSchemClass schem in stackAction)
        {
            schem.Script.fireAction(schem.Param);
            yield return new WaitForSeconds(schem.Time);
        }
        stackAction.Clear();
    }

	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Space))
            StartCoroutine(startStack());
	}
}
