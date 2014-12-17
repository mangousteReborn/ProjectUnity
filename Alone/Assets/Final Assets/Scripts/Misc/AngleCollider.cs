using UnityEngine;
using System.Collections;
using System;

public class AngleCollider : MonoBehaviour {

	private Action<Collider> _action;
	private float _radius;

	void OnTriggerEnter(Collider col){
		this._action(col);
	}

	public void activate (float radius, Action<Collider> a){
		this._radius = radius;
		this._action =a;
	}

	public void delete(){
		GameObject.Destroy(this);
	}
	public Action<Collider> action {
		set{this._action = value;}
	}
}
