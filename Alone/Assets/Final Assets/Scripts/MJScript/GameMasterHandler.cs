using UnityEngine;
using System.Collections;

public class GameMasterHandler : MonoBehaviour {

	[SerializeField]
	private GameObject _basicEntityPrefab;

	private VignetteEntity _pendingVignette;
	private GameMasterPlayer _owner;
	private GameMasterGUIScript _gui;

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

	public void onReadyButtonAction(object[] data){
		Debug.Log ("onReadyButtonAction");

		
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
		GameData.getGameManager ().networkView.RPC ("addHiddenBasicEntity", RPCMode.All, pos);


		this._pendingVignette = null;

	}

}
