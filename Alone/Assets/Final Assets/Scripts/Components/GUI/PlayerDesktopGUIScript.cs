using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

/*
 *  author: Thomas P
 * 	created_at:27/11/14
 * 
 * /?\ : Classic Player Interface (whole)
 * General and complete interface for Classic players (Game Master will get
 * another GUI).
 * 
 * 
 * GUI Hierarchie :
 * 
 * TOP Content
 * 		TODO :
 * BOTTOM Content
 * 		ButtonsWrapper
 * 			Menu Btn
 * 			Action Btn
 * 			Bonus Btn
 * 		Left Action Bar ?
 * 
 */
public class PlayerDesktopGUIScript : MonoBehaviour, IPlayerGUI, IPointerEnterHandler {

	// Hierarchized comps
	[SerializeField]
	GameObject _mainContainerObject;
		
		[SerializeField]
		GameObject _topContainerObject;
			
			[SerializeField]
			GameObject _lastActionVignetteSlotObject;
			
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



	[SerializeField]
	GameObject _vignetteSlotObject;

	[SerializeField]
	GameObject _tooltipPanelObject;



	// GUI general management variable
	private bool _bonusVignettePickerOpened = false;
	private bool _actionVignettePickerOpened = false;

	private Image _dragAndDropImage;

	private int _screenHeight;
	private int _screenWidth;

	private CharacterStats _characterStats;

	/*	- Mode :
	 * 	0 : default.
	 * 	1 : restMode (change VignetteBonus)
	 * 	2 : battleMode (use Action)
	 */
	private uint _currentMode = 0;


	void Start() {
		this._screenHeight = Screen.height;
		this._screenWidth = Screen.width;

		this._topContainerObject.GetComponent<RectTransform> ().sizeDelta = new Vector2(this._screenWidth, 50);
		this._bottomContainerObject.GetComponent<RectTransform> ().sizeDelta = new Vector2(this._screenWidth, 50);

		// ActionBar def
		int actionBarWidth = (this._screenWidth - (int)this._buttonPanelObject.GetComponent<RectTransform>().sizeDelta.x) - 20;
		this._actionBarObject.GetComponent<ActionBarScript> ().setDimension (actionBarWidth > 0 ? actionBarWidth : 100, 50);
		this._actionBarObject.GetComponent<ActionBarScript> ().setPlayerGUI (this);

		// Bonus Vignettes picker def
		this._bonusVignettesPickerObject = (GameObject)Instantiate (this._VignettesPickerObject);
		this._bonusVignettesPickerObject.transform.parent = this._mainContainerObject.transform;
		this._bonusVignettesPickerObject.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0, 50);
		this._bonusVignettesPickerObject.GetComponent<VignettesPickerScript> ().setDimension (this._screenWidth, 50);
		this._bonusVignettesPickerObject.GetComponent<VignettesPickerScript> ().setPlayerGUI (this);
		this._bonusVignettesPickerObject.SetActive (false);
		// Action Vignettes picker def
		this._actionVignettesPickerObject = (GameObject)Instantiate (this._VignettesPickerObject);
		this._actionVignettesPickerObject.transform.parent = this._mainContainerObject.transform;
		this._actionVignettesPickerObject.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0, 50);
		this._actionVignettesPickerObject.GetComponent<VignettesPickerScript> ().setDimension (this._screenWidth, 50);
		this._actionVignettesPickerObject.GetComponent<VignettesPickerScript> ().setPlayerGUI (this);
		this._actionVignettesPickerObject.SetActive (false);



		// Buttons listeners
		this._bonusButtonObject.GetComponent<Button> ().onClick.AddListener(() => { onBonusButtonClick();}); 
		this._actionButtonObject.GetComponent<Button> ().onClick.AddListener(() => { onActionButtonClick();}); 

		fillBonusVignettesPicker (); // Define default bonus vignettes (in GameData)

		switchToDefaultMode ();
	}

	private void fillBonusVignettesPicker(){
		VignettesPickerScript bvpScript = this._bonusVignettesPickerObject.GetComponent<VignettesPickerScript>();
		foreach (KeyValuePair<string, Vignette> kvp in GameData.getBonusVignettes()) {
			bvpScript.pushVignette(kvp.Key, kvp.Value, onBonusVignetteSlotClick);
			
		}
	}

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

		// TODO : Check if we are in "passiveMode" (to avoid changing vignettes during battle)
		if(this._characterStats.hasVignette(vss.vignette.key) ){
			this._characterStats.removeVignette(vss.vignette.key);
			vss.state = 1;
		} else {
			this._characterStats.pushVignette ((VignetteBonus)vss.vignette);
			vss.state = 2;
		}
			
	}




	public void setCharacterStats(CharacterStats cs){
		this._characterStats = cs;
		// TODO : Register for specifics events !!!
	}

	// Methods that change UI depending of set mode
	public void changeGameMode(uint mode){
		this._currentMode = mode;

		if (this._currentMode == 1) {
						switchToRestMode ();
				} else if (this._currentMode == 2) {
						switchToBattleMode ();
				} else
						switchToDefaultMode ();
	}
	// Only have use for Debug/Demo
	private void switchToDefaultMode(){
		this._bonusButtonObject.SetActive (true);
		this._actionButtonObject.SetActive (true);

		this._timerObject.GetComponent<Text> ().text = "0.0";
		this._gameStateObject.GetComponent<Text> ().text = "Demo";
	}
	private void switchToRestMode(){
		this._bonusButtonObject.SetActive (true);

		this._timerObject.GetComponent<Text> ().text = "0.0";
		this._gameStateObject.GetComponent<Text> ().text = "Repos";

	}
	private void switchToBattleMode(){
		this._bonusButtonObject.SetActive (false);

		this._timerObject.GetComponent<Text> ().text = "0.0";
		this._gameStateObject.GetComponent<Text> ().text = "Combat";
	}


	public void OnPointerEnter(PointerEventData p){
		//Debug.Log ("hey ehyyyy");
	}
}
