using UnityEngine;
using System.Collections;

public abstract class Action  {

	protected string _key;
	protected string _name;
	protected string _desc;

	protected Vector3 _startPosition;
	protected Vector3 _endPosition;

	protected float _actionCost;
	protected bool _pending;

	public Action(string key, string name="NONE", string desc="This is an action. And it rulez", float actionCost=0f){
		this._key = key;
		this._name = name;
		this._desc = desc;
		this._actionCost = actionCost;
	}


	// Get / Set
	public string key
	{
		get {
			return this._key;
		}
	}
	public string name
	{
		get {
			return this._name;
		}
	}
	public string desc
	{
		get {
			return this._desc;
		}
	}
	public bool pending
	{
		get {
			return this._pending;
		}
		set {
			this._pending = value;
		}
	}
	public float actionCost
	{

		get {
			return this._actionCost;
		}
		set {
			this._actionCost = value;
		}
	}
	public Vector3 endPosition
	{
		get {
			return this._endPosition;
		}
		set {
			this._endPosition = value;
		}
	}

	public Vector3 startPosition
	{
		get {
			return this._startPosition;
		}
		set {
			this._startPosition = value;
		}
	}

	public abstract Action getCopy(Action a);

	//1: Called on vignette Click (client side)
	public abstract void onActionSelection(CharacterManager cm, bool drawHelper);

	//2: Called on click validation (client side ? helper ?)
	public abstract void onActionValidation(CharacterManager cm, object[] param=null);

	//2.b : Called on action not valid (costing too much, etc ..)
	public abstract void onActionInvalid(CharacterManager cm, object[] param=null);

	//*: Called on action cancel (server side ?)
	public abstract void onActionCanceled(CharacterManager cm, object[] param=null);

	//3: Called on execution
	public abstract void onActionStart(CharacterManager cm, object[] param=null);

	//4 : Called on end of exection
	public abstract void onActionEnd(CharacterManager cm, object[] param=null);

}
