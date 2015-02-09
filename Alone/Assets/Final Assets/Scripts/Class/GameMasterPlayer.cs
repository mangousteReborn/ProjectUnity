using UnityEngine;
using System.Collections;

public class GameMasterPlayer : Player {

	private int _currPosePoint;
	private int _maxPosePoint;

	private GameMasterGUIScript _castedGui;

	public GameMasterPlayer(NetworkView id, IPlayerGUI gui=null)
		: base("GameMaster", id, gui)
	{
		this._isGM = true;
		this._castedGui = (GameMasterGUIScript)gui;
		this._currPosePoint = GameData._GAME_MASTER_AP;
		this._maxPosePoint = GameData._GAME_MASTER_AP;
	}

	public override void onInitNextRound(){

	}

	public override void resetFight(){

	}

	public override void resetRound(){
		Debug.Log ("GameMaster reset Round");
	}

	public int currPosePoint {
		get {
			return _currPosePoint;
		}
		set {

			if(this._castedGui != null)
				this._castedGui.setPosePointValue(value, this._maxPosePoint);

			_currPosePoint = value;
		}
	}
	
	public int maxPosePoint {
		get {
			return _maxPosePoint;
		}
		set {
			_maxPosePoint = value;
		}
	}
}
