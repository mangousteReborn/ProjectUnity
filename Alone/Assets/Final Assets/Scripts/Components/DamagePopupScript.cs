using UnityEngine;
using System.Collections;

public class DamagePopupScript : MonoBehaviour {

	[SerializeField]
	Animator _animator;

	[SerializeField]
	GameObject _popupObject;
	// Use this for initialization
	void Start () {
	
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
