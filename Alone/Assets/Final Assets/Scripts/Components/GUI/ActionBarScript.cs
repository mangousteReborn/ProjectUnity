using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActionBarScript : MonoBehaviour {
	[SerializeField]
	GameObject _actionbarObject;

	[SerializeField]
	GameObject _vignetteSlotObject;

	[SerializeField]
	int _vignetteSlotWidth;

	[SerializeField]
	int _vignetteSlotHeight;

	private IPlayerGUI _GUI;
	private int _maxWidth;
	private int _maxHeight;
	private List<VignetteSlotScript> _vignetteSlotList = new List<VignetteSlotScript>();

	public void setPlayerGUI(IPlayerGUI gui){
		this._GUI = gui;
	}

	public void setDimension(int width, int height){
		this._maxWidth = width;
		this._maxHeight = height;
		
		// Container
		this._actionbarObject.GetComponent<RectTransform>().sizeDelta = new Vector2 (width, height);

		// V. Slot
		this._vignetteSlotObject.GetComponent<RectTransform> ().sizeDelta = new Vector2 (this._vignetteSlotWidth, this._vignetteSlotHeight);
		
		// Init slots
		int vignetteSlotWidth = this._vignetteSlotWidth;
		int totalRowGap = this._maxWidth % vignetteSlotWidth;
		int rowSlotsCount = (this._maxWidth - totalRowGap) / vignetteSlotWidth;
		int rowGap = (int)(totalRowGap / rowSlotsCount);

		int i = 0;
		int w = 0;
		while (i < rowSlotsCount) {
			GameObject slot = (GameObject) Instantiate(this._vignetteSlotObject);
			slot.name = "slot_" + i;
			
			slot.transform.parent = this._actionbarObject.transform;

			VignetteSlotScript vss = slot.GetComponent<VignetteSlotScript>();
			vss.state = 0;
			
			RectTransform rt = slot.GetComponent<RectTransform>();
			rt.anchoredPosition = new Vector2(w, 0);
			this._vignetteSlotList.Add(vss);
			w+= vignetteSlotWidth + rowGap;
			i++;
		}
		
	}
}
