using UnityEngine;
using System.Collections;
using System;

public class DeplacementActionScript : MonoBehaviour {

    [SerializeField]
    GameObject player;

    NavMeshAgent agent;
    NavMeshPath path;
    LineRenderer line;

    Vector3 target;

    bool wantToMove = false;

    Vector3 lastFramePosition;
    bool isMoving = false;
    DateTime startMovingTime;

	// Use this for initialization
	void Start () {
        agent = player.GetComponent<NavMeshAgent>();
        lastFramePosition = agent.transform.position;
        path = new NavMeshPath();
        line = gameObject.AddComponent<LineRenderer>();
        line.SetColors(Color.red, Color.red);
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.D))
            wantToMove = !wantToMove;
        if (wantToMove)
            deplacementGesture();
        if(Input.GetKeyDown(KeyCode.P))
            player.GetComponent<testSpellScript>().enabled = !player.GetComponent<testSpellScript>().enabled;
	}

    void deplacementGesture()
    {
        if (Input.GetMouseButtonDown(0))
            deplacementAction();
        if (isMoving)
        {
            float distance = Vector3.Distance(lastFramePosition, agent.transform.position);
            float currentSpeed = Mathf.Abs(distance) / Time.deltaTime;
            if (currentSpeed < 0.0001)
            {
                Debug.Log(DateTime.Now - startMovingTime);
                isMoving = false;
            }
            lastFramePosition = agent.transform.position;
        }
        if (Input.GetMouseButtonDown(1))
        {
            startMovingTime = DateTime.Now;
            agent.SetDestination(target);
            isMoving = true;
        }
    }

    void deplacementAction()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            target = hit.point;
            target.y = player.transform.position.y;
            agent.CalculatePath(target, path);
            Vector3[] pathPosList = path.corners;
            //
            //line.SetVertexCount(0);
            line.SetVertexCount(pathPosList.Length);
            float distance = Vector3.Distance(pathPosList[0], agent.transform.position);
            line.SetPosition(0, pathPosList[0]);
            for (int i = 1; i < pathPosList.Length; i++)
            {
                line.SetPosition(i, pathPosList[i]);
                distance += Vector3.Distance(pathPosList[i], pathPosList[i-1]);
            }
            float time = distance / agent.speed;
            Debug.Log("Time : " + time);
            //agent.SetDestination(target);
        }
    }
}
