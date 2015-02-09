using UnityEngine;
using System.Collections;

public class AIEntityData {

	GameObject _prefab;

	GameObject _instanciateObject;

	Vector3 _initPos;

	Player _owner;

	int _life;
	int _str;
	float _actionPoint;

	public AIEntityData(GameObject p, Vector3 v, Player owner, int life, int str, float ap){
		_prefab = p;
		_initPos = v;
		_owner = owner;
		_life = life;
		_str = str;
		_actionPoint = ap;
	}

	public Vector3 initPos {
		get {
			return _initPos;
		}
		set {
			_initPos = value;
		}
	}

	public GameObject prefab {
		get {
			return _prefab;
		}
		set {
			_prefab = value;
		}
	}

	public Player owner {
		get {
			return _owner;
		}
		set {
			_owner = value;
		}
	}

	public int life {
		get {
			return _life;
		}
		set {
			_life = value;
		}
	}

	public int str {
		get {
			return _str;
		}
		set {
			_str = value;
		}
	}

	public float actionPoint {
		get {
			return _actionPoint;
		}
		set {
			_actionPoint = value;
		}
	}

	public GameObject instanciateObject {
		get {
			return _instanciateObject;
		}
		set {
			_instanciateObject = value;
		}
	}
}
