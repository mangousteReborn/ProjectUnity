﻿using UnityEngine;
using System.Collections;
using System;
using Assets.Final_Assets.Scripts.Class;
using UnityEngine.EventSystems;

public class DeplacementActionScript : MonoBehaviour, ActionInterface {

	[SerializeField]
	Material _lineColorMaterial;

    GameObject _player;
    public GameObject Player
    {
        get { return _player; }
        set { _player = value; }
    }

	CharacterManager _characterManager;
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
        _characterManager = _player.GetComponent<CharacterManager>();
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
			if (_characterManager.isInFight)
            {
				line.SetWidth(0,0);
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
				/*
				 * /?\ EventSystem.current.IsPointerOverGameObject() check if cursor is not on GUI
				*/
				if (Input.GetMouseButton(0) &&!EventSystem.current.IsPointerOverGameObject())
                {
                    moveToTarget(getTarget());
                }
            }
        }

	}

    void FixedUpdate()
    {
    }

    public void moveToTarget(Vector3 target)
    {
        if (Network.isServer)
        {
			NavMeshPath nmp = networkDeplacement.setTarget(_player.transform.position, target,false);
			if(null != nmp)
				agent.path = nmp;
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

	public AStarScript AStarScript{
		get{ return this.networkDeplacement;}
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
