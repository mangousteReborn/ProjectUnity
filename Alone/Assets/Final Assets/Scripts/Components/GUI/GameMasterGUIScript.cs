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

	
	// Independents comps
	[SerializeField]
	GameObject _VignettesPickerObject;


	private Player _owner;

	private GameObject _unitsVignettesPicker;

	private int _screenHeight;
	private int _screenWidth;

	private int _currentMode;

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

	public void setOwner(Player p){
		this._owner = p;
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

        GameData.getGameManager().gameObject.GetComponent<InstantiateNPCScript>().instantiateEnemy(ve.entityType, ve.cost);
	}

    private void onReadyButtonClick()
    {
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
