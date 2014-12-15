using UnityEngine;

using System;
using System.Collections;

public class WaitAction : Action {
	
	private MouseDistanceHelperScript _helper;
	
	private float _costPerUnit;
	
	// Used by GameData for definition
	public WaitAction(float costPerUnit)
		: base("wait", "Attendre", "Zzzz")
	{
		this._costPerUnit = costPerUnit;
	}
	
	public WaitAction(string k, string name, string d, float costPerUnit)
		: base (k, name, d)
	{
		this._costPerUnit = costPerUnit;
	}
	
	public WaitAction (string k, string name, string d, float cost,Vector3 startPosition, Vector3 destPosition)
		: base(k, name, d)
	{
		this._actionCost = cost;
		this._startPosition = startPosition;
		this._endPosition = destPosition;
		
	}
	
	public override Action getCopy(Action a){
		WaitAction ma = null;
		
		if (System.Object.ReferenceEquals(a.GetType (), this.GetType())) {
			ma = (WaitAction)a;
		} else {
			Debug.LogError("<"+ this.GetType().ToString() + "> have been initialized with bad parameters");
			return null;
		}
		
		return new WaitAction (ma.costPerUnit);
	}
	
	public override void onActionSelection(CharacterManager cm, bool drawHelper=true){
		ActionHelperDrawer ahd = GameData.getActionHelperDrawer ();

		Func<MouseDistanceHelperScript, float> calculateFunc = delegate(MouseDistanceHelperScript mdhs)
		{ 
			return (float)Math.Round(
				(Vector3.Distance(mdhs.getStartPoint(), mdhs.getEndPoint()) * this._costPerUnit),
				2,
				MidpointRounding.ToEven
				);
		};

		object[] p = {false,calculateFunc}; // <bool> , <Func<MouseDistanceHelperScript,float>>
		this._helper = ahd.pushMouseDistanceHelperScript(cm, this,p);
		
		this._helper.activate (cm, this);
	}
	
	public override void onActionValidation(CharacterManager cm, object[] param){
		Debug.Log("ACTION IS POWA VALIDATED GOSU !!!!!");
		
		//GameData.getActionHelperDrawer().validateCurrentPlayerHelper();
		
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
	
	
	// Get / Set
	public float costPerUnit{
		get {return this._costPerUnit;}
	}
}
