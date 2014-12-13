using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * @author : Thomas P
 * @created_at : 11/12/14
 * 
 * Action's helpers manager for Player / AI / Allies
 * 
 * Show / delete helpers depending of game state.
 */

public class ActionHelperDrawer : MonoBehaviour {

	[SerializeField]
	GameObject _moveHelperObject;

	private Stack<IActionHelper> _playerPendingHelpers;
	private Stack<IActionHelper> _playerValidatedHelpers;

	// string : Player or IA helpers
	private Dictionary<string, Stack<IActionHelper>> _othersHelpersMap;

	private IActionHelper _currentPlayerHelper;


	// Use this for initialization
	void Start () {
		this._playerPendingHelpers = new Stack<IActionHelper> ();
		this._playerValidatedHelpers = new Stack<IActionHelper> ();
		this._othersHelpersMap = new Dictionary<string, Stack<IActionHelper>>();

	}

	public void removeCurrentPlayerHelper(){
		if (this._currentPlayerHelper == null)
			return;

		this._currentPlayerHelper.delete();
		this._currentPlayerHelper = null;
	}

	public void validateCurrentPlayerHelper(object[] param=null){
		if (this._currentPlayerHelper == null)
			return;
		this._currentPlayerHelper.validate (param);
		this._playerPendingHelpers.Push (this._currentPlayerHelper);

		this._currentPlayerHelper = null;
	}

	public MoveHelperScript pushMoveHelper(CharacterManager cm, Action a){
		Vector3 startPos;
		IActionHelper lastActionHelper = this._playerPendingHelpers.Count > 0 ? this._playerPendingHelpers.Peek() : null;

		if (lastActionHelper != null) {
			startPos = lastActionHelper.getEndPoint();
			GameData.getCameraObject().GetComponent<CameraMovementScriptMouse>().lockCamera = false;
			Vector3 camPos = GameData.getCameraObject().transform.position;
			GameData.getCameraObject().transform.position = new Vector3(startPos.x, camPos.y, startPos.z);

		} else {
			startPos = cm.character.transform.position;
		}

		GameObject go = (GameObject)Instantiate (_moveHelperObject,startPos , Quaternion.identity);
		MoveHelperScript mhs = go.GetComponent<MoveHelperScript> ();

		mhs.setStartPosition (startPos);
		mhs.setAction (a);
		mhs.setOwner (cm);

		this._currentPlayerHelper = mhs;

		return mhs;

	}


}
