﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class roomBattleModeScript : MonoBehaviour {

    [SerializeField]
    private int _roomNumber;

    private List<GameObject> _enemyList;

	// Use this for initialization
	void Start () {
        this._enemyList = new List<GameObject>();
	}

    public int roomNumber
    {
        set { this._roomNumber = value; }
        get { return this._roomNumber; }
    }

    public List<GameObject> EnemyList
    {
        get { return this._enemyList; }
        set { this._enemyList = value; }
    }

    public void beginBattleMode()
    {
        Debug.Log("start IA : " + _enemyList.Count);
        foreach(GameObject enemy in _enemyList)
        {
            Debug.Log(enemy);
            //enemy.GetComponent<IA>().enable = true;
        }
    }
}
