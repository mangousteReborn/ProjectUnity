using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using System.Collections;

/**
 * @author: Thomas P
 * @created_ad : 16 / 12 / 14
 * 
 * Listner textcanvas of Helper to move them is cursor is on it.
 **/
public class TextListenerScrpit : MonoBehaviour, IPointerEnterHandler {
	[SerializeField]
	GameObject _textObject;

	private Text _text;

	// Use this for initialization
	void Start () {
		this._text = _textObject.GetComponent<Text>();
	}

	public void OnPointerEnter(PointerEventData p){
		this._textObject.transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z + 100);
	}
}
