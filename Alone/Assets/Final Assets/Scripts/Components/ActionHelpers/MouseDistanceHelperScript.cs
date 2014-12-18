using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using System;
using System.Collections;
using System.Collections.Generic;

public class MouseDistanceHelperScript : MonoBehaviour, IActionHelper{
	
	[SerializeField]
	private GameObject _object;
	
	[SerializeField]
	private GameObject _textObject;
	
	[SerializeField]
	private LineRenderer _lineRenderer;
	
	[SerializeField]
	private Text _text;
	
	private Vector3 _endPoint; // Implements
	private Vector3 _startPoint; // Implements
	private Vector3 _middlePoint; // Implements
	
	private float _costPerUnit = 0.15f;

	
	private bool _validated = false;
	private bool _activated = false;
	private bool _RPCcallback = true;

	private bool _drawLine = false;
	private Func<MouseDistanceHelperScript, float> _calculateDistanceFuncion = null;

	private float _currentCost;
	private float _size;
	
	private Action _action;
	private CharacterManager _owner;
	
	public CharacterManager manager{
		get { return _owner; }
	}
	
	void Update(){
		if (!this._activated)
			return;
		
		if (this._validated)
			return;
		//bool b = EventSystemManager.currentSystem.IsPointerOverEventSystemObject ();
		if (Input.GetMouseButton (0) && !EventSystem.current.IsPointerOverGameObject()) {
			Vector3 target = getTarget();
			calcDistance(target);
			
			if(this._RPCcallback){
				this._owner.networkView.RPC("validateWaitActionRPC", RPCMode.All,this._owner.player.id, this._endPoint);
				this._RPCcallback = false;
			}
			return;
		}
		
		calcDistance(getTarget());
	}
	
	
	
	/*
	public void setAction(Action a){
		this._moveAction = (MoveAction)a;
		
	}
	

	public void setOwner(CharacterManager cm){
		this._owner = cm;
		
	}
	*/

	// IMPLEMENTS ME
	public void activate (CharacterManager cm, Action a){
		
		this._action = a;
		this._owner = cm;

		// WaitAction case
		if(a.key == "wait"){
			WaitAction wa = (WaitAction) a;
			cm.networkView.RPC("pushWaitActionAsPendingRPC", RPCMode.All, wa.key, wa.name, wa.desc,  wa.costPerUnit);
			
		} else if (a.key == "move"){
			Debug.LogError("General helper for Move key doest implemented");
			delete();
			return;
		} else {
			Debug.LogError("MouseDistanceHelperScript : <active> Action's key '" + a.key + "' not supported");
			delete();
			return;
		}

		
		this._activated = true;
	}
	
	public void cancel(CharacterManager cm){
		cm.networkView.RPC ("removePendingActionRPC", RPCMode.All);
		GameObject.Destroy (this._object);
	}
	
	public void validate(CharacterManager cm){
		GameData.getActionHelperDrawer().networkView.RPC("pushDefaultStaticHelperRPC", RPCMode.All, this._owner.player.id, this._startPoint, this._middlePoint, this._endPoint, this._text.text);
		this._validated = true;
	}
	
	public void setStartPosition(Vector3 startPos){
		this._startPoint = startPos;
		this._object.transform.position = startPos;
		
	}
	
	
	
	
	
	public void delete(){
		GameObject.Destroy (this._object);
	}
	
	public bool validate(object[] param){
		
		//this._endPoint = (Vector3)param [0];
		return true;
		
	}
	
	
	private void calcDistance(Vector3 destPos){

		this._endPoint = destPos;

		if (this._drawLine) {
			this._lineRenderer.SetVertexCount(2);
			this._lineRenderer.SetPosition(0, this._startPoint);
			this._lineRenderer.SetPosition(1, this._endPoint);
		}

		if(null != this._calculateDistanceFuncion){
			this._currentCost = this._calculateDistanceFuncion(this);
			Debug.Log("defined Func, this._curr cost == " + this._currentCost);
		} else {
			this._currentCost = defaultCalculateDistance(this._startPoint, this._endPoint);
		}

		this._middlePoint = ((this._endPoint - this._startPoint ) / 2) + this._startPoint;

		this._textObject.transform.position = this._middlePoint;
		this._text.text = this._action.name + "\n" + this._currentCost + "s";
		if (this._currentCost > this._owner.characterStats.currentActionPoint) {
			this._text.color = Color.red;
		} else 
			this._text.color = Color.white;

	}

	private Vector3 getTarget()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		Vector3 target = this._startPoint;
		if (Physics.Raycast(ray, out hit))
		{
			target = hit.point;
			target.y = 0;
		}
		return target;
	}

	private float defaultCalculateDistance(Vector3 start, Vector3 dest){
		return (float)Math.Round(
			(Vector3.Distance(start, dest) * this._costPerUnit),
			2,
			MidpointRounding.ToEven
			);
	}

	public Vector3 getEndPoint() {
		return this._endPoint;
	}
	public Vector3 getStartPoint() {
		return this._startPoint;
	}
	public Vector3 getMiddlePoint() {
		return this._middlePoint;
	}

	public bool drawLine {
		get {return this._drawLine;}
		set {this._drawLine = value;}
	}

	public Func<MouseDistanceHelperScript, float> calculateDistanceFuncion{
		set{this._calculateDistanceFuncion = value;}
	}
	public bool RPCcallback{
		set{
			Debug.Log("rollracking");
			this._RPCcallback = value;
		}
	}
	
	
}
