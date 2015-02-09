using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameMasterHandler : MonoBehaviour {

	/* Prefabs */
	[SerializeField]
	private GameObject _basicEntityPrefab;

	[SerializeField]
	private List<GameObject> _GMSpawn;

	private VignetteEntity _pendingVignette;
	private GameMasterPlayer _owner;
	private GameMasterGUIScript _gui;

	private int _currRoom = 0;

	private bool _active = false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(!_active)
			return;
	}

	public void initialize(GameMasterPlayer p, GameMasterGUIScript gui){
		_owner = p;
		_gui = gui;

		_gui.setReadyButtonAction (onReadyButtonAction);
		_gui.setVignetteButtonAction (onVignetteButtonAction);
		_pendingVignette = null;
		_active = true;
	}

	public void moveGMToCurrentRoom(){
		GameData.getGameManager().networkView.RPC("moveGMToPosition",RPCMode.All, this._GMSpawn[_currRoom].transform.position);
		//_owner.playerObject.transform.position = this._GMSpawn[_currRoom].transform.position;
		
		CameraMovementScriptMouse cam = Camera.main.GetComponent<CameraMovementScriptMouse>();
		Camera.main.transform.position = _GMSpawn[_currRoom].transform.position + new Vector3(0,Camera.main.transform.position.y - this._GMSpawn[_currRoom].transform.position.y,0);
		cam.replaceRestictArea(_GMSpawn[_currRoom].transform.parent.transform.parent.gameObject);
	}

	public void moveGMToNextRoom(){

		if(_currRoom + 1 > _GMSpawn.Count){
			Debug.LogWarning("Last room reach : " + _currRoom + "/" + this._GMSpawn.Count);
			_currRoom = 0;
		}

		_currRoom++;

		GameData.getGameManager().networkView.RPC("moveGMToPosition",RPCMode.All, this._GMSpawn[_currRoom].transform.position);
		//_owner.playerObject.transform.position = this._GMSpawn[_currRoom].transform.position;
		
		CameraMovementScriptMouse cam = Camera.main.GetComponent<CameraMovementScriptMouse>();
		Camera.main.transform.position = _GMSpawn[_currRoom].transform.position + new Vector3(0,Camera.main.transform.position.y - this._GMSpawn[_currRoom].transform.position.y,0);
		cam.replaceRestictArea(_GMSpawn[_currRoom].transform.parent.transform.parent.gameObject);


	}

	// Clicks
	public void onReadyButtonAction(object[] data){
		Debug.Log ("onReadyButtonAction");

		GameData.getGameManager ().networkView.RPC("instanciateHiddenEntities", RPCMode.All);
		GameData.getGameManager().networkView.RPC("openRoomNumber", RPCMode.All, _currRoom + 1);
		Debug.Log("Opening room " + _currRoom);
		// Next : Move GM TO NEXT ROOM

		return;
		if(Network.isServer)
			GameData.getGameManager ().gameMasterReady();
		else
			GameData.getGameManager ().networkView.RPC ("gameMasterReady", RPCMode.Server);
		
		Debug.Log("instanciate");
		GameData.getGameManager().gameObject.GetComponent<InstantiateNPCScript>().onValidate();
	}

	public void onVignetteButtonAction(VignetteEntity ve){

		if(_pendingVignette != null)
			return;

		if (ve.cost > _owner.currPosePoint) {
			return;
		}

		Mesh m = null;
		Transform t = null;
		GameObject go = null;
		// Classic enemy
		if (ve.entityType == VignetteEntity.EntityType.Base) {
			go = (GameObject)Instantiate(_basicEntityPrefab);
			t = go.GetComponent<Transform>();
			m = go.GetComponent<MeshFilter>().mesh;

		}

		if (null == go) {
			Debug.LogError("Cannot create Entity helper for GM ...");
			return;
		}
		_pendingVignette = ve;
		AIEntityHelperScript aie = GameData.getActionHelperDrawer ().pushAIEntityHelper (m, t, onValidationClick);
		Destroy (go);
	}

	private void onValidationClick(Vector3 pos){

		GameData.getGameManager ().networkView.RPC ("setGameMasterPosePoint", RPCMode.All, _owner.currPosePoint - this._pendingVignette.cost);
		GameData.getGameManager ().networkView.RPC ("addHiddenBasicEntity", RPCMode.All, pos, _owner.id);


		this._pendingVignette = null;

	}

}
