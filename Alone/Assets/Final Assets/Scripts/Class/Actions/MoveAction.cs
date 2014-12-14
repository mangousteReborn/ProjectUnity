using UnityEngine;
using System.Collections;

public class MoveAction : Action {

	private MoveHelperScript _lineHelper;

	private float _costPerUnit;
	private Vector3 _destPosition;

	// Used by GameData for definition
	public MoveAction(float costPerUnit)
	: base("move", "Deplacement", "Marche petit !")
	{
		this._costPerUnit = costPerUnit;
	}

	public MoveAction (string k, string name, string d, float cost, Vector3 destPosition)
		: base(k, name, d)
	{
		this._actionCost = cost;
		this._destPosition = destPosition;

	}

	public override Action getCopy(Action a){
		MoveAction ma = null;
		if (Object.ReferenceEquals(a.GetType (), this.GetType())) {
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

		Debug.Log ("Action on Selectionnnnn !!");
	}

	/* Param 
	 * [0] <NetworkViewID> id of Player 
	 * [1] <Vector3> dest
	 */ 
	public override bool onActionValidation(CharacterManager cm, object[] param){
        float cost = (float)param[0];

        if (cost > cm.characterStats.currentActionPoint)
        {
			Debug.Log("No enough action point : ");
            return false;
        }
        cm.characterStats.currentActionPoint -= cost;

        this._actionCost = cost;

        Debug.Log("Validated .!.");

        // Maybe dangerous ....
        GameData.getActionHelperDrawer().validateCurrentPlayerHelper();

        cm.characterStats.pushHotAction(this);
        //this._lineHelper.validate();
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

	public override void onActionStart(CharacterManager cm, object[] param=null){
		Debug.Log ("Running action ...");
		cm.character.GetComponent<DeplacementActionScript> ().moveToTarget(this._destPosition);
		
	}

	public override void onActionEnd(CharacterManager cm, object[] param=null){
		Debug.Log ("Running end ...");
	}

	public float costPerUnit{
		get {return this._costPerUnit;}
	}
}
