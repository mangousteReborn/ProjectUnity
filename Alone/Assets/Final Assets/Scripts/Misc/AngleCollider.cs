using UnityEngine;
using System.Collections;
using System;

public class AngleCollider : MonoBehaviour {

	private Action<Collider> _action;
	private float _radius;
	/*
	void OnCollisionEnter(Collision col){
		Debug.Log("OnCollisionEnter");
		this._action(col.collider);

	}
	void OnCollisionStay(Collision col){
		Debug.Log("OnCollisionStay");
		this._action(col.collider);
	}


	void OnTriggerStay(Collider col){
		Debug.Log("OnTriggerStay");
		this._action(col);
	}
	*/

	void OnTriggerEnter(Collider col){
		Debug.Log("OnTriggerEnter");
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
