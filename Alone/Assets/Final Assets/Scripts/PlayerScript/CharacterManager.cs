using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
/*
 * @author: Thomas P
 * @created_at:15/11/2014
 * 
 * Character manager for Player char. and IA char.
 * /!\ MUST BE GENERIC, CAUSE WILL BE USED BY BOTH AI/PLAYER CHARACTERS
 */
public class CharacterManager : MonoBehaviour {
	
	CharacterStats _characterStats;
		

	[SerializeField]
	public GameObject _healthBar;

	[SerializeField]
	public GameObject _character;

	void Start () {
		this._characterStats = new CharacterStats ();

		this._healthBar = (GameObject)Instantiate (this._healthBar);
		RectTransform r = this._healthBar.GetComponent<RectTransform>();
		r.position = this._character.transform.position;


	}


	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.T)) {
			this._characterStats.pushEffect(new EffectBonusLife(100));
		}
	}
}
