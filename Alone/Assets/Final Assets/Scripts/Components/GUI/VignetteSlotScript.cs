using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;

public class VignetteSlotScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler {
	
	[SerializeField]
	GameObject _vignetteSlotObject;

	[SerializeField]
	Button _button;

	[SerializeField]
	int _width;

	[SerializeField]
	int _height;

	[SerializeField]
	private Image _mask;
	

	private Vignette _vignette;

	private IPlayerGUI _playerGUI;

	// State
	// 0 : empty
	// 1 : 
	private uint _state = 0;
	private bool _cursorInRect = false;
	// Constant
	private Color _emptyColor = new Color (0f, 0f, 0f, 0.3f);
	private Color _availableColor = new Color (0f, 0f, 0f, 0f);
	private Color _selectedColor = new Color (1f, 0f, 0f, 0.3f);

	void Start(){
		this._mask.color = _emptyColor;
	}

	void Update(){

	}

	private void onVignetteChange(){



	}

	public void setPlayerGUI(IPlayerGUI gui){
		this._playerGUI = gui;
		/*
		foreach (VignetteSlotScript vss in this._vignetteSlotObject) {
			vss.setPlayerGUI (gui);
		}
		*/
	}
	
	public void OnPointerEnter(PointerEventData p){
		this._cursorInRect = true;
		StartCoroutine ("showTooltip");

	}

	public void OnPointerExit(PointerEventData p){
		this._cursorInRect = false;
		
	}

	IEnumerator showTooltip(){

		yield return new WaitForSeconds(1);
		if(this._cursorInRect)
			Debug.Log ("Tooltip !!!");
	}

	public void OnPointerUp(PointerEventData p){
		Debug.Log ("up");
	}

	public void OnPointerDown(PointerEventData p){
		Debug.Log ("down");
	}
	// Get / Set
	public Vignette vignette
	{
		get {
			return this._vignette;
		}
		set{
			this._vignette = value;
			this.GetComponent<Image> ().sprite =  Resources.Load <Sprite>(this._vignette.imagePath);
		}
	}

	public uint state
	{
		get {
			return this._state;
		}
		set{
			this._state = value;
			if (this._state == 0) {
				this._mask.color = _emptyColor;
			} else if (this._state == 1) {
				this._mask.color = _availableColor;
			} else if (this._state == 2) {
				this._mask.color = _selectedColor;
			}
		}
	}

	public Button button
	{
		get {
			return this._button;
		}
	}
}
