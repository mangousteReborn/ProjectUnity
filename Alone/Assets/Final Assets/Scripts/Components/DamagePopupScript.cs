using UnityEngine;
using System.Collections;

public class DamagePopupScript : MonoBehaviour {

	[SerializeField]
	Animator _animator;

	[SerializeField]
	GameObject _popup;
	// Use this for initialization
	void Start () {

	}
	
	public void destroy(){
		Destroy (_popup);
	}
}
