using UnityEngine;
using System.Collections;
using System;
using Assets.Final_Assets.Scripts.Class;

public class DeplacementActionScript : MonoBehaviour, ActionInterface {

	[SerializeField]
	Material _lineColorMaterial;

    GameObject _player;
    public GameObject Player
    {
        get { return _player; }
        set { _player = value; }
    }

    playerCaracScript carac;
    AStarScript networkDeplacement;

    NavMeshAgent agent;
    NavMeshPath path;
    LineRenderer line;

    Vector3 target;
    Vector3 currentTarget;

    bool wantToMove = false;
    bool isMoving = false;

    public delegate void ActionEventImplemente(Vector3 target);
    public event ActionEventImplemente action;

    public void fireAction(object[] param)
    {
        Vector3 pos = (Vector3)param[0];
        action(pos);
    }

	// Use this for initialization
	void Start () {
        _player = gameObject;
        networkDeplacement = new AStarScript(_player.networkView);
        agent = _player.GetComponent<NavMeshAgent>();
        carac = _player.GetComponent<playerCaracScript>();
        path = new NavMeshPath();
        line = gameObject.AddComponent<LineRenderer>();
        line.SetWidth(1.0f, 1.0f);
		/*
		Material m = line.GetComponent<Material>();
		m = _lineColorMaterial;
		*/
        line.SetColors(Color.yellow, Color.yellow);
	}
	
	// Update is called once per frame
	void Update () {
        if (networkView.isMine)
        {
            if (carac.IsInFight)
            {
                if (Input.GetKeyDown(KeyCode.P))
                    _player.GetComponent<testSpellScript>().enabled = !_player.GetComponent<testSpellScript>().enabled;
                if (Input.GetKeyDown(KeyCode.D))
                    wantToMove = !wantToMove;
                if (wantToMove)
                {
                    if (Input.GetMouseButton(0))
                        drawPathTotarget(getTarget());
                    if (this.target != null && Input.GetKeyDown(KeyCode.KeypadEnter))
                    {
                        object[] obj = new object[] { this.target };

                        StackSchemClass schem = new StackSchemClass(this, obj, getTime());
                        _player.GetComponent<stackActionScript>().addAction(schem);
                        action += moveToTarget;
                        wantToMove = false;
                    }
                }
            }
            else
            {
                if (isMoving)
                    drawPathTotarget(this.target);
                if (Input.GetMouseButton(0))
                {
                    moveToTarget(getTarget());
                }
            }
        }

	}

    void FixedUpdate()
    {
    }

    void moveToTarget(Vector3 target)
    {
        if (Network.isServer)
        {
            agent.path = networkDeplacement.setTarget(_player.transform.position, target,false);
        }
        else
        {
            networkView.RPC("setTargetRPC", RPCMode.Server,networkView.viewID, _player.transform.position, target);
        }
        isMoving = true;
        //agent.SetDestination(target);
    }

    Vector3 getTarget()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Vector3 target = this.target;
        if (Physics.Raycast(ray, out hit))
        {
            target = hit.point;
            target.y = _player.transform.position.y;
            this.target = target;
        }
        return target;
    }

    void drawPathTotarget(Vector3 target)
    {
        if (agent.CalculatePath(target, path))
        {
            Vector3[] pathPosList = path.corners;
            line.SetVertexCount(pathPosList.Length);
            line.SetPosition(0, pathPosList[0]);
            float distance = Vector3.Distance(pathPosList[0], agent.transform.position);
            for (int i = 1; i < pathPosList.Length; i++)
            {
                line.SetPosition(i, pathPosList[i]);
                distance += Vector3.Distance(pathPosList[i], pathPosList[i - 1]);
            }
            float time = distance / agent.speed;
        }
    }

    float getTime()
    {
        Vector3[] pathPosList = path.corners;
        float distance = Vector3.Distance(pathPosList[0], agent.transform.position);
        for (int i = 1; i < pathPosList.Length; i++)
            distance += Vector3.Distance(pathPosList[i], pathPosList[i - 1]);
        return distance / agent.speed;
    }

    private void changeDirection(NetworkViewID id,string pathCorner)
    {
        Debug.Log("change direction");
        //NetworkView.Find(id).gameObject.GetComponent<NavMeshAgent>().path = networkDeplacement.changeDirection(pathCorner);
    }

    [RPC]
    private void setTargetRPC(NetworkViewID id, Vector3 startPos, Vector3 newTarget)
    {
        Debug.Log("setTargetRPC");
        NavMeshPath path = networkDeplacement.setTarget(startPos, newTarget,true);
        if (path != null)
            NetworkView.Find(id).gameObject.GetComponent<NavMeshAgent>().path = path;
    }
}
