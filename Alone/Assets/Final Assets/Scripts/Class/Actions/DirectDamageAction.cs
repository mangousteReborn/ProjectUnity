using UnityEngine;

using System;
using System.Collections;

public class DirectDamageAction : Action {
	//FIXME
	//private ConeHelerScript _helper;
	
	private float _costPerUnit;
	private float _degree;
	private float _radius;
	private float _damages;
	private int _maxAttacks = 1;



	// Used by GameData for definition
	public DirectDamageAction(float costPerUnit, float degree, float radius, int damages)
		: base("directdamage", "Traquer", "Attend que ta proie soit dans ta ligne de mire puis FEU !")
	{
		this._costPerUnit = costPerUnit;
		this._degree = degree;
		this._radius = radius;
		this._damages = damages;
	}
	// FIXME
	public DirectDamageAction(string k, string name, string d, float costPerUnit)
		: base (k, name, d)
	{
		this._costPerUnit = costPerUnit;
	}
	// FIXME
	public DirectDamageAction (string k, string name, string d, float cost,Vector3 startPosition, Vector3 destPosition)
		: base(k, name, d)
	{
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
		
		return new DirectDamageAction (ma.costPerUnit, ma.degree, ma.radius, ma.damages);
	}
	//FIXME
	public override void onActionSelection(CharacterManager cm, bool drawHelper=true){
		ActionHelperDrawer ahd = GameData.getActionHelperDrawer ();
		
		//this._lineHelper = ahd.pushMoveHelper(cm, this);
		
		//this._lineHelper.activate (cm, this);
	}
	
	public override void onActionValidation(CharacterManager cm, object[] param){
		
		//GameData.getActionHelperDrawer().validateCurrentPlayerHelper();
		
	}
	// FIXME
	public override void onActionInvalid (CharacterManager cm, object[] param)
	{
		//if(this._lineHelper != null)
		//	this._lineHelper.RPCcallback = true;
	}
	//FIXME
	public override void onActionCanceled(CharacterManager cm, object[] param=null){
		//this._helper.cancel (cm);
		
	}
	
	public override void onActionStart(CharacterManager cm, object[] param=null){

		
	}
	
	public override void onActionEnd(CharacterManager cm, object[] param=null){

	}
	// FIXME ??
	public float calculateCost(Vector3 start, Vector3 dest){
		return (float)Math.Round(
			(Vector3.Distance(start, dest) * this._costPerUnit),
			2,
			MidpointRounding.ToEven
			);
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
	public float damages{
		get {return this._damages;}
	}
}
