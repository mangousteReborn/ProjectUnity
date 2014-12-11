using UnityEngine;
using System.Collections;

public class MoveAction : Action {

	public MoveAction()
	: base("move", "Deplacement", "Marche petit !")
	{

	}

	public override void onActionSelection(CharacterManager cm, bool drawHelper=true){
		Debug.Log ("Action on Selectionnnnn !!");
	}
	
	public override void onActionValidation(CharacterManager cm, Vector3 nextPos){

	}
}
