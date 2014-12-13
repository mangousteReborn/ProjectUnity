using UnityEngine;
using System.Collections;

public class WaitAction : Action {
	
	private WaitHelperScript _waitHelper;
	
	private float _costPerUnit;
	
	// Used by GameData for definition
	public WaitAction(float costPerUnit)
	: base("wait", "Attendre", "Zzzz ...")
	{
		this._costPerUnit = costPerUnit;
	}
	
	public override Action getCopy(Action a){
		WaitAction ma = null;
		if (Object.ReferenceEquals(a.GetType (), this.GetType())) {
			ma = (WaitAction)a;
		} else {
			Debug.LogError("WaitAction have been initialized with bad parameters");
			return null;
		}
		
		return new MoveAction (ma.costPerUnit);
	}
	
	public override void onActionSelection(CharacterManager cm, bool drawHelper=true){
		ActionHelperDrawer ahd = GameData.getActionHelperDrawer ();
		
		//this._waitHelper = ahd.pushMoveHelper(cm, this);
		
		Debug.Log ("Action on Selectionnnnn !!");
	}
	
	/* Param 
	 * [0] <float> cost
	 * [1] <Vector3> target 
	 */ 
	public override bool onActionValidation(CharacterManager cm, object[] param){
		float cost = (float)param[0];
		
		if (cost > cm.characterStats.currentActionPoint) {
			return false;
			
		}
		cm.characterStats.currentActionPoint -= cost;
		
		this._actionCost = cost;
		
		Debug.Log ("Validated .!.");
		
		// Maybe dangerous ....
		GameData.getActionHelperDrawer ().validateCurrentPlayerHelper();
		
		cm.characterStats.pushHotAction (this);
		//this._waitHelper.validate();
		return true;
		
	}
	
	public override void cancelAction(object[] param=null){
		Debug.Log ("Cancel action ...");
		CharacterStats cs = (CharacterStats)param [0];
		
		cs.currentActionPoint += this._actionCost;
	}
	
	public override void onActionCanceled(CharacterManager cm, object[] param=null){
		Debug.Log ("onCancel action ...");
		cm.characterStats.currentActionPoint += this._actionCost;
		
	}
	
	public override void onActionRunning(CharacterManager cm, object[] param=null){
		Debug.Log ("Running action ...");
		cm.character.GetComponent<DeplacementActionScript> ().moveToTarget(this._waitHelper.getEndPoint());
		
	}
	
	public float costPerUnit{
		get {return this._costPerUnit;}
	}
}
