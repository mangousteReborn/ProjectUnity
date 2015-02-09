using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System;

/*
 *  author: Thomas P
 * 	created_at:13/12/14
 * 
 * /?\ : Game Master Interface (whole)
 * 
 * 
 * 
 */

public class GameMasterGUIScript : MonoBehaviour, IPlayerGUI{
	[SerializeField]
	GameObject _mainContainerObject;

		[SerializeField]
		GameObject _topContainerObject;
		
			[SerializeField]
			GameObject _gameStateObject;

			[SerializeField]
			GameObject _readyButtonObject;
			
			[SerializeField]
			Text _posePointText;
	
	// Independents comps
	[SerializeField]
	GameObject _VignettesPickerObject;


	private Player _owner;

	private GameObject _unitsVignettesPicker;

	private int _screenHeight;
	private int _screenWidth;

	private int _currentMode;

	private Action<VignetteEntity> _vignetteAction;
	private Action _readyAction;

	// Use this for initialization
	void Start () {
		this._screenHeight = Screen.height;
		this._screenWidth = Screen.width;

		// Bonus Vignettes picker def
		this._unitsVignettesPicker = (GameObject)Instantiate (this._VignettesPickerObject);
		this._unitsVignettesPicker.transform.parent = this._mainContainerObject.transform;
		this._unitsVignettesPicker.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0, 0);
		this._unitsVignettesPicker.GetComponent<VignettesPickerScript> ().setDimension (this._screenWidth, 50);
		this._unitsVignettesPicker.GetComponent<VignettesPickerScript> ().setPlayerGUI (this);

        this._readyButtonObject.GetComponent<Button>().onClick.AddListener(() => { onReadyButtonClick(); });

		fillVignettesPicker ();
	}

	public void setReadyButtonAction(Action<object[]> a){
		//_readyAction = a;
		this._readyButtonObject.GetComponent<Button>().onClick.AddListener(() => { a(null); });
	}

	public void setVignetteButtonAction(Action<VignetteEntity> a){
		_vignetteAction = a;
	}

	public void setOwner(Player p){
		_owner = p;
	}

	public void setPosePointValue(int value, int max){
		_posePointText.text = value + " / " + max;
	}

    private void fillVignettesPicker()
    {
        VignettesPickerScript bvpScript = this._unitsVignettesPicker.GetComponent<VignettesPickerScript>();
		foreach (KeyValuePair<string, Vignette> kvp in GameData.getEntitiesVignettes())
        {
			bvpScript.pushVignette(kvp.Key, kvp.Value, onEntityVignetteClick );

        }
    }

	private void onEntityVignetteClick(object[] data){
		VignetteSlotScript vss = (VignetteSlotScript)data [0];
		VignetteEntity ve = (VignetteEntity)vss.vignette;

		if (null != _vignetteAction) {
			_vignetteAction(ve);
		}
        //GameData.getGameManager().gameObject.GetComponent<InstantiateNPCScript>().instantiateEnemy(ve.entityType, ve.cost);
	}

    private void onReadyButtonClick()
    {

		//GameData.getGameManager().gameObject.GetComponent<InstantiateNPCScript>().onValidate();
		return;

		if(Network.isServer)
			GameData.getGameManager ().gameMasterReady();
		else
			GameData.getGameManager ().networkView.RPC ("gameMasterReady", RPCMode.Server);
			
        Debug.Log("instanciate");
		GameData.getGameManager().gameObject.GetComponent<InstantiateNPCScript>().onValidate();
    }

	// Update is called once per frame
	void Update () {
	
	}

	public void changeGameMode(int gameMode){
		this._currentMode = gameMode;
		
		if (this._currentMode == 1) {
			switchToPoseMode ();
		} else if (this._currentMode == 2) {
			//switchToBattleMode ();
		} else if (this._currentMode == 3){
			//switchToSpectatorMode();
		} else{
			this._gameStateObject.GetComponent<Text> ().text = "MODE " + gameMode + " UNKNOW";
		}
	}

	private void switchToPoseMode(){
		this._gameStateObject.GetComponent<Text> ().text = "Pose";
	}
}
