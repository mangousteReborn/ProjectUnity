using UnityEngine;
using System.Collections;

public class DamagePopupScript : MonoBehaviour {

	[SerializeField]
	Animator _animator;

	[SerializeField]
	GameObject _popupObject;

	[SerializeField]
	Vector3 _scale;

	// Use this for initialization
	void Start () {
		_popupObject.transform.localScale = _scale;
	}
	
	public void destroy(){
		Destroy (_popupObject);
	}

	public GameObject popupObject{
		get {
			return _popupObject;
		}
	}
}
