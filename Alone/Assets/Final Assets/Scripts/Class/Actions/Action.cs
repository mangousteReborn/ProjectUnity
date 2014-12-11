using UnityEngine;
using System.Collections;

public abstract class Action  {

	private string _key;
	private string _name;
	private string _desc;

	private bool _pending;

	public Action(string key, string name="NONE", string desc="This is an action. And it rulez"){
		this._key = key;
		this._name = name;
		this._desc = desc;
	}

	// Get / Set
	public string key
	{
		get {
			return _key;
		}
	}
	public string name
	{
		get {
			return _name;
		}
	}
	public string desc
	{
		get {
			return _desc;
		}
	}
	public bool pending
	{
		get {
			return _pending;
		}
		set {
			this._pending = value;
		}
	}

	public abstract void onActionSelection(CharacterManager cm, bool drawHelper);

	public abstract void onActionValidation(CharacterManager cm, Vector3 nextPos);
}
