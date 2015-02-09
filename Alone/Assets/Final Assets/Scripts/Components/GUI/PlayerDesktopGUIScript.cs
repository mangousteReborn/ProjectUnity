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


	private int _screenHeight;
	private int _screenWidth;

	private Player _owner;
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
	private int _currentMode = 1;

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
	public void setOwner(Player p){
		this._owner = p;
		CharacterManager cm = p.characterManager;

		this._charaterManager = cm;
		this._characterStats = cm.characterStats;
		/*
		this._characterStats.register(CharacterStatsEvent.actionAdded, onActionAdded);
		this._characterStats.register(CharacterStatsEvent.gameModeChanged, onGameModeChange);
		this._characterStats.register(CharacterStatsEvent.hotActionPushed, onHotActionPushed);
		this._characterStats.register(CharacterStatsEvent.currentActionPointChanged, onCurrentActionPointChange);
		*/
		// Setting default actions of player
		foreach (Action a in this._characterStats.availableActionList) {
			Vignette v = GameData.getActionVignette(a.key);
			this._actionVignettesPickerObject.GetComponent<VignettesPickerScript>().pushVignette(v.key,v,onActionVignetteSlotClick);
		}
		switchToFreeMode();
	}

	// IMPLEMENTS
	public void updateGUI(){
		this._timerObject.GetComponent<Text> ().text = 
			this._owner.characterManager.characterStats.currentActionPoint.ToString("F2") + "s / " + 
				this._owner.characterManager.characterStats.maxActionPoint.ToString("F2") + "s";

	}

	// IMPLEMENTS
	public void changeGameMode(int mode){
		this._currentMode = mode;
		
		if (this._currentMode == 1) {
			switchToFreeMode ();
		} else if (this._currentMode == 2) {
			switchToBattleMode ();
		} else if (this._currentMode == 3){
			switchToSpectatorMode();
		} else if (this._currentMode == 4){
			switchLooseMode();
		} else{
			this._gameStateObject.GetComponent<Text> ().text = "MODE " + mode + " UNKNOW";
		}
	}

	/*
	 *  Listeners Methods
	 */
	/* CharacterStats listeners
	// Listen current game mode and change UI.
	private void onGameModeChange(CharacterStats cs, object[] param){
		changeGameMode((int)param [0]);

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
	 */



	public void setActionPointText(float curr, float max){
		this._timerObject.GetComponent<Text> ().text = Math.Round(curr, 2) + " / " + Math.Round(max, 2);
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
		
		this._pendingAction.onActionCanceled (this._charaterManager);
		this._pendingAction = null;

		this._cancelActionButtonObject.SetActive (false);
	}

	private void onReadyButtonClick(){
		if (this._currentMode != 2) {
			return;		
		}
		int count = GameData.myself.characterManager.characterStats.hotActionsStack.Count;
		Debug.Log ("Actions count :: " + count);
        //changeGameMode (3);
		GameData.getGameManager().networkView.RPC("addReadyPlayer", RPCMode.All, GameData.myself.playerObject.networkView.viewID, count);
		/*
		if (Network.isServer) {
			GameData.getGameManager().addReadyPlayer(this._charaterManager.networkView.viewID);
		} else {
			GameData.getGameManager().networkView.RPC("addReadyPlayer", RPCMode.Server, this._charaterManager.networkView.viewID);
		}
		*/
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
	private void switchToFreeMode(){
		this._bonusButtonObject.SetActive (true);

		this._cancelActionButtonObject.SetActive (false);
		this._readyButtonObject.SetActive (false);

		this._timerObject.GetComponent<Text> ().text = "0.0";
		this._gameStateObject.GetComponent<Text> ().text = "Déplacement Libre";

	}
	private void switchToBattleMode(){
		this._bonusButtonObject.SetActive (false);

		this._cancelActionButtonObject.SetActive (false);
		this._readyButtonObject.SetActive (true);
		this._actionButtonObject.SetActive (true);

		this._timerObject.GetComponent<Text> ().text = "0.0";
		this._gameStateObject.GetComponent<Text> ().text = "Planification";
	}
	private void switchToSpectatorMode(){
		this._bonusButtonObject.SetActive (false);
		this._actionButtonObject.SetActive (false);

		this._cancelActionButtonObject.SetActive (false);
		this._readyButtonObject.SetActive (false);

		this._timerObject.GetComponent<Text> ().text = "0.0";
		this._gameStateObject.GetComponent<Text> ().text = "Combat";
	}

	private void switchLooseMode(){
		this._bonusButtonObject.SetActive (false);
		this._actionButtonObject.SetActive (false);
		
		this._cancelActionButtonObject.SetActive (false);
		this._readyButtonObject.SetActive (false);
		
		this._timerObject.GetComponent<Text> ().text = "";
		this._gameStateObject.GetComponent<Text> ().color = new Color(1f,0,0);
		this._gameStateObject.GetComponent<Text> ().fontSize = 20;
		this._gameStateObject.GetComponent<Text> ().text = "Vous avez Perdu";
	}

	public void OnPointerClick(PointerEventData ped){
		/*
		if(this._pendingAction == null)
			return;
		this._pendingAction.onActionValidation(this._charaterManager,ped.worldPosition);
		this._pendingAction = null;
		*/
	}

}
