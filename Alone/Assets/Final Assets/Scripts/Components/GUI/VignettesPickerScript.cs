using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
/*
 * author : Thomas P
 * created_at: 05/12/2014
 */ 
public class VignettesPickerScript : MonoBehaviour {

	[SerializeField]
	GameObject _vignettesPickerObject;

		[SerializeField]
		GameObject _vignettesWrapperObject;

		[SerializeField]
		GameObject _buttonLeftObject;

		[SerializeField]
		GameObject _buttonRightObject;

	[SerializeField]
	GameObject _vignetteSlotObject;

	[SerializeField]
	Text _title;

	[SerializeField]
	Animator _animator;
	
	private bool _active = false;

	private IPlayerGUI _GUI;

	private Dictionary <string, Vignette> _vignettesMap = new Dictionary <string, Vignette> ();
	private List <Vignette> _VignetteList = new List <Vignette> ();
	private List <VignetteSlotScript> _vignetteSlotList = new List<VignetteSlotScript> ();

	private int _maxWidth;
	private int _maxHeight;

	private int _wrapperWidth;
	private int _wrapperHeight;
	private int _currWrapperWidth;
	private int _currWrapperHeignt;

	private int _vignetteSlotCount;

	// Constants
	private int _buttonWidth = 30;
	private int _wrapperGap = 10;
	private int _vignetteSlotHeight = 40;
	private int _vignetteSlotWidth = 40;

	// Use this for initialization
	void Start () {
		/*
		this._vignettesMap = new Dictionary <string, Vignette> ();
		this._VignetteList = new List <Vignette> ();
		this._vignetteSlotList = new List<GameObject> ();

		this._currWrapperWidth = 0;
		this._currWrapperHeignt = 0;
		*/
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void setDimension(int widht, int height){
		this._maxWidth = widht;
		this._maxHeight = height;

		// Container
		this._vignettesPickerObject.GetComponent<RectTransform>().sizeDelta = new Vector2 (widht, height);

		// Commands Buttons
		this._buttonLeftObject.GetComponent<RectTransform> ().sizeDelta = new Vector2 (this._buttonWidth, height);
		this._buttonRightObject.GetComponent<RectTransform> ().sizeDelta = new Vector2 (this._buttonWidth, height);

		// V. Wrappers
		this._wrapperWidth = widht - (this._buttonWidth * 2 + this._wrapperGap);
		this._wrapperHeight = height;
		this._vignettesWrapperObject.GetComponent<RectTransform> ().sizeDelta = new Vector2 (this._wrapperWidth, this._wrapperHeight);
	
		// V. Slot
		this._vignetteSlotObject.GetComponent<RectTransform> ().sizeDelta = new Vector2 (this._vignetteSlotWidth, this._vignetteSlotHeight);

		// Init slots
		int totalRowGap = this._wrapperWidth % this._vignetteSlotWidth;
		int rowSlotsCount = (this._wrapperWidth - (this._wrapperWidth % this._vignetteSlotWidth)) / this._vignetteSlotWidth;
		int rowGap = (int)(totalRowGap / rowSlotsCount);

		this._vignetteSlotCount = rowSlotsCount;

		int i = 0;
		int w = 0;
		while (i < rowSlotsCount) {
			/*
			rt.anchorMax = new Vector2(0f,0.5f);
			rt.anchorMin = new Vector2(0f,0.5f);
			*/

			GameObject slot = (GameObject) Instantiate(this._vignetteSlotObject);
			slot.name = "slot_" + i;

			slot.transform.parent = this._vignettesWrapperObject.transform;


			RectTransform rt = slot.GetComponent<RectTransform>();
			rt.anchoredPosition = new Vector2(w, 0);
			/*

			Debug.Log("\ti = " + i + " w = " + w + " RowCount = " + rowSlotsCount + " SLOT Pos = " + slot.GetComponent<RectTransform>().localPosition);
			rt.transform.position = new Vector3(w,0,0);
			*/
			this._vignetteSlotList.Add(slot.GetComponent<VignetteSlotScript>());
			w+= this._vignetteSlotWidth + rowGap;
			i++;
		}
	
	}

	public void setTitle(string value){
		this._title.text = value;
	}

	// Vignettes manipulation methods
	public void pushVignette(string k , Vignette v, Action<object[]> a){
		if (this._vignettesMap.ContainsKey (k))
			return;
		this._vignettesMap.Add (k, v);
		this._VignetteList.Add (v);

		bool f = false;
		foreach (VignetteSlotScript vss in this._vignetteSlotList) {
			if (vss.state == 0){
				vss.vignette = v;
				vss.state = 1;
				vss.button.onClick.RemoveAllListeners();

				object[] param = {vss};
				vss.button.onClick.AddListener(() => { a(param);});
				f = true;
				break;
			}
		}

		if (!f) {
			Debug.Log("Slots list is full ?!!");
		}

	}

	public void setPlayerGUI(IPlayerGUI gui){
		this._GUI = gui;
	}

	public void show(){
		this._vignettesPickerObject.SetActive (true);
		this._animator.SetBool ("active", true);
	}

	public void hide(){

		this._animator.SetBool ("active", false);
		this._vignettesPickerObject.SetActive (false);
	}

	public void slideUp(){
		this._vignettesPickerObject.SetActive (true);
		this._animator.SetBool ("slideDown", false);
		this._animator.SetBool ("slideUp", true);
	}

	public void slideDown(){
		this._vignettesPickerObject.SetActive (false);
		this._animator.SetBool ("slideUp", false);
		this._animator.SetBool ("slideDown", true);
	}

	public void toggleActive(){
		this._active = !this._active;
		if (this._active) {
			this._vignettesPickerObject.SetActive (true);
		} else {
			this._vignettesPickerObject.SetActive (false);
		}
	}

	public void onSlideUp(){
		Debug.Log ("on Slide up!");
	}

	public void onSlideDown(){
		Debug.Log ("on Slide down!");
		this._vignettesPickerObject.SetActive (false);
	}
}
