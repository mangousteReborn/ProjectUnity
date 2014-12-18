using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System;

/*
 *  author: Thomas P
 * 	created_at:27/11/14
 * 
 * /?\ : Classic Player Interface (whole)
 * General and complete interface for Classic players (Game Master will get
 * another GUI).
 * 
 * 
 * 
 */
public class PlayerDesktopGUIScript : MonoBehaviour, IPlayerGUI, IPointerClickHandler {

	// Hierarchized comps
	[SerializeField]
	GameObject _mainContainerObject;
		
		[SerializeField]
		GameObject _topContainerObject;
			
			[SerializeField]
			GameObject _lastActionVignetteSlotObject;
			
			[SerializeField]
			GameObject _cancelActionButtonObject;
			
			[SerializeField]
			GameObject _readyButtonObject;

			[SerializeField]
			GameObject _gameStateObject;
			
			[SerializeField]
			GameObject _timerObject;

		[SerializeField]
		GameObject _bottomContainerObject;
				
			[SerializeField]
			GameObject _buttonPanelObject;
				
				[SerializeField]
				GameObject _bonusButtonObject;

				[SerializeField]
				GameObject _actionButtonObject;
				
				[SerializeField]
				GameObject _menuButtonObject;

		[SerializeField]
		GameObject _actionBarObject;

	// Independents comps
	[SerializeField]
	GameObject _VignettesPickerObject;


	//[SerializeField]
	GameObject _bonusVignettesPickerObject;

	GameObject _actionVignettesPickerObject;

	/* TODO : soon !
	[SerializeField]
	GameObject _tooltipPanelObject;
	*/


	// GUI general management variable
	private bool _bonusVignettePickerOpened = false;
	private bool _actionVignettePickerOpened = false;

	private Image _dragAndDropImage;

	private int _screenHeight;
	private int _screenWidth;

	private CharacterManager _charaterManager;
	private CharacterStats _characterStats;

	private Action _pendingAction;

    public delegate void addReadyPlayerHandler(object sender, EventArgs e);
    public event addReadyPlayerHandler readyPlayer;

	/*	- currentMode :
	 * 	0 : default.
	 * 	1 : restMode (change VignetteBonus)
	 * 	2 : battleMode (use Action)
	 */
	private uint _currentMode = 0;

    void Awake()
    {
        this._screenHeight = Screen.height;
        this._screenWidth = Screen.width;

        this._topContainerObject.GetComponent<RectTransform>().sizeDelta = new Vector2(this._screenWidth, 50);
        this._bottomContainerObject.GetComponent<RectTransform>().sizeDelta = new Vector2(this._screenWidth, 50);

        // ActionBar def
        int actionBarWidth = (this._screenWidth - (int)this._buttonPanelObject.GetComponent<RectTransform>().sizeDelta.x) - 20;
        this._actionBarObject.GetComponent<ActionBarScript>().setDimension(actionBarWidth > 0 ? actionBarWidth : 100, 50);
        this._actionBarObject.GetComponent<ActionBarScript>().setPlayerGUI(this);

        // Bonus Vignettes picker def
        this._bonusVignettesPickerObject = (GameObject)Instantiate(this._VignettesPickerObject);
        this._bonusVignettesPickerObject.transform.parent = this._mainContainerObject.transform;
        this._bonusVignettesPickerObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 50);
        this._bonusVignettesPickerObject.GetComponent<VignettesPickerScript>().setDimension(this._screenWidth, 50);
        this._bonusVignettesPickerObject.GetComponent<VignettesPickerScript>().setPlayerGUI(this);
        this._bonusVignettesPickerObject.SetActive(false);
        // Action Vignettes picker def
        this._actionVignettesPickerObject = (GameObject)Instantiate(this._VignettesPickerObject);
        this._actionVignettesPickerObject.transform.parent = this._mainContainerObject.transform;
        this._actionVignettesPickerObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 50);
        this._actionVignettesPickerObject.GetComponent<VignettesPickerScript>().setDimension(this._screenWidth, 50);
        this._actionVignettesPickerObject.GetComponent<VignettesPickerScript>().setPlayerGUI(this);
        this._actionVignettesPickerObject.SetActive(false);



        // Buttons listeners
        this._bonusButtonObject.GetComponent<Button>().onClick.AddListener(() => { onBonusButtonClick(); });
        this._actionButtonObject.GetComponent<Button>().onClick.AddListener(() => { onActionButtonClick(); });
        this._cancelActionButtonObject.GetComponent<Button>().onClick.AddListener(() => { onCancelActionButtonClick(); });
        this._readyButtonObject.GetComponent<Button>().onClick.AddListener(() => { onReadyButtonClick(); });
        fillBonusVignettesPicker(); // Define default bonus vignettes (in GameData)

        switchToDefaultMode();
    }

	private void fillBonusVignettesPicker(){
		VignettesPickerScript bvpScript = this._bonusVignettesPickerObject.GetComponent<VignettesPickerScript>();
		foreach (KeyValuePair<string, Vignette> kvp in GameData.getBonusVignettes()) {
			bvpScript.pushVignette(kvp.Key, kvp.Value, onBonusVignetteSlotClick);
			
		}
	}

	/*
	 * Requested Methods
	 */
	// IMPLEMENTS
	public void setCharacterManager(CharacterManager cm){
		this._charaterManager = cm;
		this._characterStats = cm.characterStats;
		this._characterStats.register(CharacterStatsEvent.actionAdded, onActionAdded);
		this._characterStats.register(CharacterStatsEvent.gameModeChanged, onGameModeChange);
		this._characterStats.register(CharacterStatsEvent.hotActionPushed, onHotActionPushed);
		this._characterStats.register(CharacterStatsEvent.currentActionPointChanged, onCurrentActionPointChange);
		// Setting default actions of player
		foreach (Action a in this._characterStats.availableActionList) {
			Vignette v = GameData.getActionVignette(a.key);
			this._actionVignettesPickerObject.GetComponent<VignettesPickerScript>().pushVignette(v.key,v,onActionVignetteSlotClick);
		}
	}
	// IMPLEMENTS
	public void changeGameMode(uint mode){
		this._currentMode = mode;
		
		if (this._currentMode == 1) {
			switchToRestMode ();
		} else if (this._currentMode == 2) {
			switchToBattleMode ();
		} else if (this._currentMode == 3){
			switchToSpectatorMode();
		} else
			switchToDefaultMode ();
	}

	/*
	 *  Listeners Methods
	 */
	/* CharacterStats listeners */
	// Listen current game mode and change UI.
	private void onGameModeChange(CharacterStats cs, object[] param){
		changeGameMode((uint)param [0]);

	}
	// Add action in available actions list
	private void onActionAdded(CharacterStats cs, object[] data){
		Vignette v = (Vignette)data [0];
		this._actionVignettesPickerObject.GetComponent<VignettesPickerScript> ().pushVignette (v.key, v, onActionVignetteSlotClick);
	}
	// TODO : Fill cancel SlotVignette
	private void onHotActionPushed(CharacterStats cs, object[] param){
		// Fill Fill
		this._pendingAction = null;
		this._cancelActionButtonObject.SetActive (false);
		Debug.Log ("onHotActionPushed");
	}

	// Param[0] <float> old value, Param[1] <float> currentValue
	private void onCurrentActionPointChange(CharacterStats cs, object[] param){
		this._timerObject.GetComponent<Text> ().text = cs.currentActionPoint + "s";
	}




	/* Buttons / SlotButtons listeners */
	private void onBonusButtonClick(){
		if (this._bonusVignettePickerOpened) {
			this._bonusVignettesPickerObject.GetComponent<VignettesPickerScript>().hide();
			this._bonusVignettePickerOpened = false;
			return;
		}

		if (this._actionVignettePickerOpened) {
			this._actionVignettesPickerObject.GetComponent<VignettesPickerScript>().hide();
			this._actionVignettePickerOpened = false;
		}
		
		this._bonusVignettesPickerObject.GetComponent<VignettesPickerScript>().show();
		this._bonusVignettePickerOpened = true;

	}

	private void onActionButtonClick(){
		if (this._actionVignettePickerOpened) {
			this._actionVignettesPickerObject.GetComponent<VignettesPickerScript>().hide();
			this._actionVignettePickerOpened = false;
			return;
		}
		
		if (this._bonusVignettePickerOpened) {
			this._bonusVignettesPickerObject.GetComponent<VignettesPickerScript>().hide();
			this._bonusVignettePickerOpened = false;
		}
		
		this._actionVignettesPickerObject.GetComponent<VignettesPickerScript>().show();
		this._actionVignettePickerOpened = true;


	}
	private void onCancelActionButtonClick(){
		if (this._pendingAction == null)
			return;
		
		//GameData.getActionHelperDrawer ().removeCurrentPlayerHelper ();
		
		this._pendingAction.onActionCanceled (this._charaterManager);
		this._pendingAction = null;

		this._cancelActionButtonObject.SetActive (false);
	}

	private void onReadyButtonClick(){
		if (this._currentMode != 2) {
			return;		
		}
        changeGameMode (3);
		//this._charaterManager.runHotAcions ();
		newPlayerReady ();
		return;
        if(Network.isServer)
        {
            readyPlayer(null, null);
            networkView.RPC("newPlayerReady", RPCMode.Others);
        }
        else
        {
            networkView.RPC("newPlayerReady", RPCMode.Server);
        }
	}

	private void onBonusVignetteSlotClick(object[] data){
		if (null == this._characterStats) {
			Debug.LogError("onBonusVignetteSlotClick : CharacterStats not set !");
			return;
		}
		
		VignetteSlotScript vss = (VignetteSlotScript)data [0];
		if (vss.vignette.type != VignetteType.bonus) {
			Debug.LogError("onBonusVignetteSlotClick : Vignette is not a Bonus !");
			return;
		}
		
		
		if(this._characterStats.hasVignette(vss.vignette.key) ){
			this._characterStats.removeVignette(vss.vignette.key);
			vss.state = 1;
		} else {
			this._characterStats.pushVignette ((VignetteBonus)vss.vignette);
			vss.state = 2;
		}
		
	}
	
	/*
	 * /?\ Tricky :
	 * 	ActionVignette slot contains an Action predefined in GameData.
	 * 	The goal is to create a copy on this linked Action, with the same parameters.
	 *  So, every Action implementation (like MoveAction) have to override getCopy method.
	 * 	This method return a new instance of implemented Action object with the same parameters.
	 * 	We have to use Introspection to have a generic behaviour.
	 */
	private void onActionVignetteSlotClick(object[] data){
		if (this._currentMode != 2 || this._characterStats.pendingAction != null)
			return;

		VignetteAction va = (VignetteAction)((VignetteSlotScript)data [0]).vignette;

		object[] param = {va.action};
		Action newAction = (Action)va.action.GetType ().GetMethod ("getCopy").Invoke (va.action, param);
		
		newAction.onActionSelection (this._charaterManager, true);
		this._pendingAction = newAction;
		this._cancelActionButtonObject.SetActive (true);
	}


	// TODO : Check if we are in "passiveMode" (to avoid changing vignettes during battle)


	/*
	 *  Others Methods
	 */

	// Methods that change UI depending of set mode

	// Only have use for Debug/Demo
	private void switchToDefaultMode(){
		this._bonusButtonObject.SetActive (true);
		this._actionButtonObject.SetActive (true);

		this._timerObject.GetComponent<Text> ().text = "0.0";
		this._gameStateObject.GetComponent<Text> ().text = "Demo";
	}
	private void switchToRestMode(){
		this._bonusButtonObject.SetActive (true);

		this._cancelActionButtonObject.SetActive (false);
		this._readyButtonObject.SetActive (false);

		this._timerObject.GetComponent<Text> ().text = "0.0";
		this._gameStateObject.GetComponent<Text> ().text = "Repos";

	}
	private void switchToBattleMode(){
		this._bonusButtonObject.SetActive (false);

		this._cancelActionButtonObject.SetActive (false);
		this._readyButtonObject.SetActive (true);

		this._timerObject.GetComponent<Text> ().text = "0.0";
		this._gameStateObject.GetComponent<Text> ().text = "Combat";
	}
	private void switchToSpectatorMode(){
		this._bonusButtonObject.SetActive (false);
		this._actionButtonObject.SetActive (false);

		this._cancelActionButtonObject.SetActive (false);
		this._readyButtonObject.SetActive (false);

		this._timerObject.GetComponent<Text> ().text = "0.0";
		this._gameStateObject.GetComponent<Text> ().text = "Spectateur";
	}

	public void OnPointerClick(PointerEventData ped){
		/*
		if(this._pendingAction == null)
			return;
		this._pendingAction.onActionValidation(this._charaterManager,ped.worldPosition);
		this._pendingAction = null;
		*/
	}

	private void newPlayerReady()
	{
		GameData.getGameManager ().networkView.RPC ("increaseReadyPlayer", RPCMode.All, this._charaterManager.networkView.viewID);
		return;
		/*

		readyPlayer(null, null);
		if(Network.isServer)
			networkView.RPC("newPlayerReady", RPCMode.Others);
		*/
	}
	
	[RPC]
	public void broadcastStartSimulation()
	{
		if(Network.isServer)
		{
			networkView.RPC("broadcastStartSimulation", RPCMode.Others);
			this._charaterManager.runHotAcions();
		}
		else
		{
			this._charaterManager.runHotAcions();
		}
	}
}
