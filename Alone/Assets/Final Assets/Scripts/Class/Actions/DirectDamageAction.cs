using UnityEngine;

using System;
using System.Collections;

public class DirectDamageAction : Action {
	private DirectDamageHelperScript _helper;

	private float _costPerUnit;
	private float _degree;
	private float _radius;

	private int _damages;
	private int _maxAttacks = 1; // Maybe player (CharacterStats) have to manage it



	// Used by GameData for definition
	public DirectDamageAction(float costPerUnit, float degree, float radius, int damages)
		: base("directdamage", "Traquer", "Attend que ta proie soit dans ta ligne de mire puis FEU !")
	{
		this._costPerUnit = costPerUnit;
		this._degree = degree;
		this._radius = radius;
		this._damages = damages;
	}
	
	public DirectDamageAction(string k, string name, string d, float costPerUnit,float degree, float radius, int damages)
		: base (k, name, d)
	{
		this._costPerUnit = costPerUnit;
		this._degree = degree;
		this._radius = radius;
		this._damages = damages;
	}
	
	public DirectDamageAction (
		string k, string name, string d,  
		float costPerUnit,float degree, float radius, int damages, 
		float cost,Vector3 startPosition, Vector3 destPosition
	)
		: base(k, name, d)
	{
		this._degree = degree;
		this._radius = radius;
		this._damages = damages;

		this._actionCost = cost;
		this._startPosition = startPosition;
		this._endPosition = destPosition;
		
	}
	
	public override Action getCopy(Action a){
		DirectDamageAction ma = null;
		
		if (System.Object.ReferenceEquals(a.GetType (), this.GetType())) {
			ma = (DirectDamageAction)a;
		} else {
			Debug.LogError("DirectDamageAction have been initialized with bad parameters");
			return null;
		}
		
		return new DirectDamageAction (ma.key, ma.name, ma.desc,ma.costPerUnit, ma.degree, ma.radius, ma.damages);
	}

	public override void onActionSelection(CharacterManager cm, bool drawHelper=true){
		ActionHelperDrawer ahd = GameData.getActionHelperDrawer ();
		
		this._helper = ahd.pushDirectDamageHelper(cm, this);
		
		this._helper.activate (cm, this);
	}
	
	public override void onActionValidation(CharacterManager cm, object[] param){

		
	}
	
	public override void onActionInvalid (CharacterManager cm, object[] param)
	{
		if(this._helper != null)
			this._helper.RPCcallback = true;
	}


	public override void onActionCanceled(CharacterManager cm, object[] param=null){
		this._helper.cancel (cm);
		
	}
	
	public override void onActionStart(CharacterManager cm, object[] param=null){

		
	}
	
	public override void onActionEnd(CharacterManager cm, object[] param=null){

	}




	public float calculateCost(Vector3 start, Vector3 dest){
		return (float)Math.Round(
			(Vector3.Distance(start, dest) * this._costPerUnit),
			2,
			MidpointRounding.ToEven
			);
	}
	
	public GameObject getCollider(){
		GameObject go = new GameObject ();
		Transform t = go.AddComponent<Transform> ();
		t.position = this._endPosition;
		
		SphereCollider sc = go.AddComponent<SphereCollider> ();
		sc.radius = this._radius;

		//go.AddComponent<SphereColliderScript> ();

		return go;
	}

	// Get / Set
	public float costPerUnit{
		get {return this._costPerUnit;}
	}
	public float degree{
		get {return this._degree;}
	}
	public float radius{
		get {return this._radius;}
	}
	public int damages{
		get {return this._damages;}
	}
}
