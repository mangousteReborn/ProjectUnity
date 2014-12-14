using UnityEngine;

using System;
using System.Collections;

public class MoveAction : Action {

	private MoveHelperScript _lineHelper;

	private float _costPerUnit;

	// Used by GameData for definition
	public MoveAction(float costPerUnit)
	: base("move", "Deplacement", "Marche petit !")
	{
		this._costPerUnit = costPerUnit;
	}

	public MoveAction(string k, string name, string d, float costPerUnit)
	: base (k, name, d)
	{
		this._costPerUnit = costPerUnit;
	}

	public MoveAction (string k, string name, string d, float cost,Vector3 startPosition, Vector3 destPosition)
	: base(k, name, d)
	{
		this._actionCost = cost;
		this._startPosition = startPosition;
		this._endPosition = destPosition;

	}

	public override Action getCopy(Action a){
		MoveAction ma = null;

		if (System.Object.ReferenceEquals(a.GetType (), this.GetType())) {
			ma = (MoveAction)a;
		} else {
			Debug.LogError("MoveAction have been initialized with bad parameters");
			return null;
		}

		return new MoveAction (ma.costPerUnit);
	}

	public override void onActionSelection(CharacterManager cm, bool drawHelper=true){
		ActionHelperDrawer ahd = GameData.getActionHelperDrawer ();

		this._lineHelper = ahd.pushMoveHelper(cm, this);

		this._lineHelper.activate (cm, this);
		Debug.Log ("Action on Selectionnnnn !!");
	}

	public override void onActionValidation(CharacterManager cm, object[] param){
        Debug.Log("ACTION IS POWA VALIDATED GOSU !!!!!");

        // Maybe dangerous ....
        GameData.getActionHelperDrawer().validateCurrentPlayerHelper();

	}

	public override void onActionInvalid (CharacterManager cm, object[] param)
	{
		this._lineHelper.RPCcallback = true;
	}

	public override void onActionCanceled(CharacterManager cm, object[] param=null){
		Debug.Log ("onCancel action ...");
		this._lineHelper.cancel (cm);
		//cm.characterStats.currentActionPoint += this._actionCost;

	}

	public override void onActionStart(CharacterManager cm, object[] param=null){
		Debug.Log ("Running action ...");
		cm.character.GetComponent<DeplacementActionScript> ().moveToTarget(this._endPosition);
		
	}

	public override void onActionEnd(CharacterManager cm, object[] param=null){
		Debug.Log ("Running end ...");
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
