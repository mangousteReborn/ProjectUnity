using UnityEngine;
using System.Collections;

public class GameMasterPlayer : Player {



	public GameMasterPlayer(NetworkView id, IPlayerGUI gui)
		: base("GameMaster", id, gui)
	{
		this._isGM = true;
	}

	public GameMasterPlayer(NetworkView id)
		: base("GameMaster", id)
	{
		this._isGM = true;
	}

	public override void resetRound(){
		Debug.Log ("GameMaster reset Round");
	}
}
