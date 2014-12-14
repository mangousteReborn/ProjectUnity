using UnityEngine;
using UnityEngine.UI;

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

	[SerializeField]
	GameObject _defaultStaticHelperObject;


	private Stack<GameObject> _playerHelpers;

	// string : Player or IA helpers
	private Dictionary<string, Stack<GameObject>> _othersHelpersMap;

	private IActionHelper _currentPlayerHelper;


	// Use this for initialization
	void Start () {
		this._playerHelpers = new Stack<GameObject> ();
		this._othersHelpersMap = new Dictionary<string, Stack<GameObject>>();

	}

	/*
	public void removeCurrentPlayerHelper(){
		if (this._currentPlayerHelper == null)
			return;

		this._currentPlayerHelper.delete();
		this._currentPlayerHelper = null;
	}

public void addHelperInMap(NetworkViewID playerID, GameObject helper){
		string k = playerID.ToString ();
		if (playerID.isMine) {
			Debug.LogError("Calling addHelerInMap for current player");
			return;
		}

		if (this._othersHelpersMap.ContainsKey(k)){
			Stack<GameObject> s = null;
			this._othersHelpersMap.TryGetValue(k, out s);
			s.Push(helper);

		}else {
			Stack<GameObject> s = new Stack<GameObject>();
			s.Push(helper);
			this._othersHelpersMap.Add(k, s);
		}
	}
	
	*/

	private void pushHelperForOtherPlayer(NetworkViewID playerID, GameObject helper){
		string k = playerID.ToString ();
		if (playerID.isMine) {
			Debug.LogError("Calling addHelerInMap for current player");
			return;
		}
		
		if (this._othersHelpersMap.ContainsKey(k)){
			Stack<GameObject> s = null;
			this._othersHelpersMap.TryGetValue(k, out s);
			s.Push(helper);
			
		}else {
			Stack<GameObject> s = new Stack<GameObject>();
			s.Push(helper);
			this._othersHelpersMap.Add(k, s);
		}
	}

	public MoveHelperScript pushMoveHelper(CharacterManager cm, Action a){
		Vector3 startPos;
		Action lastAction = cm.characterStats.getLastHotAction ();
		Debug.Log ("lastAction ? " + lastAction);
		if (lastAction != null) {
			startPos = lastAction.endPosition;
			GameData.getCameraObject().GetComponent<CameraMovementScriptMouse>().lockCamera = false;
			Vector3 camPos = GameData.getCameraObject().transform.position;
			GameData.getCameraObject().transform.position = new Vector3(startPos.x, camPos.y, startPos.z);

		} else {
			startPos = cm.character.transform.position;
		}

		GameObject go = (GameObject)Instantiate (_moveHelperObject,startPos , Quaternion.identity);
		MoveHelperScript mhs = go.GetComponent<MoveHelperScript> ();

		mhs.setStartPosition (startPos);

		//mhs.activate(cm,a);

		this._currentPlayerHelper = mhs;

		return mhs;

	}

	[RPC]
	public void pushDefaultStaticHelperRPC(NetworkViewID playerID, Vector3 startPoint, Vector3 endPoint, string label){
		if(GameData.getPlayerByNetworkViewID(playerID).isGM)
			return;
		Debug.Log ("ok");
		GameObject go = (GameObject)Instantiate (_defaultStaticHelperObject, startPoint , Quaternion.identity);
		LineRenderer ln = go.GetComponent<LineRenderer>();
		DefaultStaticHelperScript dshs = go.GetComponent<DefaultStaticHelperScript>();
		dshs.text.text = label;
		dshs.textObject.transform.position = ((endPoint - startPoint ) / 2) + startPoint;

		ln.SetVertexCount(2);
		ln.SetPosition(0, startPoint);
		ln.SetPosition(1, endPoint);

		if (playerID.isMine) {
			this._currentPlayerHelper.delete();
			this._playerHelpers.Push(go);
		} else {
			pushHelperForOtherPlayer(playerID, go);
		}

	}

	[RPC]
	public void pushMoveHelperRPC(NetworkViewID playerID, Vector3 startPoint, Vector3 middlePoint, Vector3 endPoint){


		Debug.Log ("pushmovehlepr : mine ? " + playerID.isMine); 
		if (playerID.isMine) {
			return;
			this._currentPlayerHelper.delete();
		}
	}
	

}
