using UnityEngine;
using System.Collections;

public class spawnEntityInfos {

    private GameObject _entity;
    private Vector3 _position;

    public GameObject entity
    {
        get { return this._entity; }
    }

    public Vector3 position
    {
        get { return this._position; }
    }

    public spawnEntityInfos(GameObject entity,Vector3 position)
    {
        this._entity = entity;
        this._position = position;
    }

}
