using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class stackActionScript : MonoBehaviour {

    private List<ActionInterface> stackAction;

	// Use this for initialization
	void Start () {
        stackAction = new List<ActionInterface>();
	}

    public void addAction(ActionInterface script)
    {
        stackAction.Add(script);
    }

    void startStack()
    {
        foreach (ActionInterface script in stackAction)
            script.fireActionSpell();
    }

	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Space))
            startStack();
	}
}
