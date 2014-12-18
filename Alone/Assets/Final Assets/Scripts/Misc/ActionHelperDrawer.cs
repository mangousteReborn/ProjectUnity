﻿using UnityEngine;
using UnityEngine.UI;

using System;
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

	/* Use for client side*/
	[SerializeField]
	GameObject _mouseDistanceHelperObject;

	[SerializeField]
	GameObject _moveHelperObject;

	[SerializeField]
	GameObject _directDamageObject;

	/* Use in server side*/
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

	public void deleteAllHelpers(){
		Debug.Log ("deleteAllHelpers");
		// Current player helpers
		GameObject go = this._playerHelpers.Count > 0 ? this._playerHelpers.Pop () : null;
		while (null != go) {
			GameObject.Destroy(go);
			go = this._playerHelpers.Count > 0 ? this._playerHelpers.Pop () : null;
		}

		// Others Players
		foreach(KeyValuePair<string, Stack<GameObject>> kvp in this._othersHelpersMap){
			GameObject gobj =  kvp.Value.Count > 0 ?  kvp.Value.Pop () : null;
			while (null != gobj) {
				GameObject.Destroy(gobj);
				gobj =  kvp.Value.Count > 0 ?  kvp.Value.Pop () : null;
			}

		}

	}

	/*
	 * Client side Components
	 */
	private Vector3 defineStartPosition(CharacterManager cm){
		Vector3 startPos;
		Action lastAction = cm.characterStats.getLastHotAction ();
		if (lastAction != null) {
			startPos = lastAction.endPosition;
			GameData.getCameraObject().GetComponent<CameraMovementScriptMouse>().lockCamera = false;
			Vector3 camPos = GameData.getCameraObject().transform.position;
			GameData.getCameraObject().transform.position = new Vector3(startPos.x, camPos.y, startPos.z);
			
		} else {
			startPos = cm.character.transform.position;
		}
		return startPos;
	}
	/* Params 
	 * [0] <bool> draw linerenderer
	 * [1] <Action<MouseDistanceHelperScript> > calculateDistance formula
	 */
	public MouseDistanceHelperScript pushMouseDistanceHelperScript(CharacterManager cm, Action a, object[] param){
		Vector3 startPos = defineStartPosition(cm);

		bool drawLine = true;
		Func<MouseDistanceHelperScript, float> func = null;
		if(null != param){
			if(param.Length >= 1)
				drawLine = (bool)param[0];
			if(param.Length >= 2)
				func = (Func<MouseDistanceHelperScript, float>)param[1];
		}

		GameObject go = (GameObject)Instantiate (_mouseDistanceHelperObject,startPos , Quaternion.identity);
		MouseDistanceHelperScript helper = go.GetComponent<MouseDistanceHelperScript> ();
		
		helper.setStartPosition (startPos);
		helper.drawLine = drawLine;
		helper.calculateDistanceFuncion = func;

		this._currentPlayerHelper = helper;
		
		return helper;
	}

	public MoveHelperScript pushMoveHelper(CharacterManager cm, Action a){
		Vector3 startPos = defineStartPosition(cm);

		GameObject go = (GameObject)Instantiate (_moveHelperObject,startPos , Quaternion.identity);
		MoveHelperScript mhs = go.GetComponent<MoveHelperScript> ();

		mhs.setStartPosition (startPos);

		this._currentPlayerHelper = mhs;

		return mhs;

	}

	public DirectDamageHelperScript pushDirectDamageHelper(CharacterManager cm, Action a){
		Vector3 startPos = defineStartPosition(cm);
		
		GameObject go = (GameObject)Instantiate (_directDamageObject, startPos , Quaternion.identity);
		DirectDamageHelperScript helper = go.GetComponent<DirectDamageHelperScript> ();
		
		helper.setStartPosition (startPos);



		this._currentPlayerHelper = helper;
		
		return helper;
	}


	/*
	 *  RPC
	 */

	// Specific Helpers will ne puched for client side obly. After validating a helper, will put for all player a static helper
	// as a Description of current player actions.
	[RPC]
	public void pushDefaultStaticHelperRPC(NetworkViewID playerID, Vector3 startPoint, Vector3 endPoint, string label){
		if(GameData.getPlayerByNetworkViewID(playerID).isGM)
			return;
		GameObject go = (GameObject)Instantiate (_defaultStaticHelperObject, startPoint , Quaternion.identity);
		LineRenderer ln = go.GetComponent<LineRenderer>();
		ln.material = GameData.getPlayerByNetworkViewID (playerID).characterManager.material;

		DefaultStaticHelperScript dshs = go.GetComponent<DefaultStaticHelperScript>();
		dshs.text.text = label;
		dshs.textObject.transform.position = ((endPoint - startPoint ) / 2) + startPoint;

		ln.SetVertexCount(2);
		startPoint.y = 0;
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
	public void pushDirectDamageStaticHelperRPC(NetworkViewID playerID, Vector3 startPoint, Vector3 endPoint, string label , float degree, float radius, float angle){
		if(GameData.getPlayerByNetworkViewID(playerID).isGM)
			return;
		GameObject go = (GameObject)Instantiate (_directDamageObject, startPoint , Quaternion.identity);
		DirectDamageHelperScript ddhs = go.GetComponent<DirectDamageHelperScript>();
		ddhs.activateAsStatic(GameData.getPlayerByNetworkViewID(playerID).characterManager, startPoint, endPoint ,label, degree, radius, angle);
		ddhs.material = GameData.getPlayerByNetworkViewID (playerID).characterManager.material;

		if (playerID.isMine) {
			this._currentPlayerHelper.delete();
			this._playerHelpers.Push(go);
		} else {
			pushHelperForOtherPlayer(playerID, go);
		}
	}

}
