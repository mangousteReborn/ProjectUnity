using UnityEngine;
using UnityEngine.UI;

using System.Collections.Generic;
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
	

		
	[SerializeField]
	private GameObject _damagePopup;

	[SerializeField]
	private GameObject _healthBar;

	[SerializeField]
	public GameObject _character;

	private CharacterStats _characterStats = new CharacterStats ();
	private bool _isInFight = false;

	void Start () {

		// HealthBar init (if defined)
		if (this._healthBar != null) {

			this._healthBar = (GameObject)Instantiate (this._healthBar);
			//this._healthBar.transform.parent = this._character.transform;

			this._healthBar.GetComponent<HealthbarScript>().setCharacterStats(this._characterStats);
			
			this._characterStats.register (CharacterStatsEvent.currentLifeChange, createPopup);
		}

		//this._characterStats.pushEffect(new EffectDamageOverBattle(10, 3));


	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.T)) {
			
			this._characterStats.currentLife += (int)UnityEngine.Random.Range(-5,5);
		}
		if (Input.GetKeyDown (KeyCode.Y)) {
			
			this._characterStats.currentLife += 1;
		}
		if (Input.GetKeyDown (KeyCode.M)) {
			Vignette v = GameData.getBonusVignette("lifebonus");
			Debug.Log("Vignette === " + v.name);
		}
		  //!\\
		 //!!!\\
		//!!!!!\\
		this._healthBar.transform.position = new Vector3(this._character.transform.position.x,3,this._character.transform.position.z + 1);
	}



	private void createPopup(CharacterStats stats, object[] param){
		int damages = (int)param [0] - (int)param [1];
		Color color = damages >= 0 ? Color.red : Color.green;

		GameObject popup = (GameObject)Instantiate (this._damagePopup, this._character.transform.position,  Quaternion.identity);


		popup.GetComponentInChildren<Text> ().color = color;
		popup.GetComponentInChildren<Text> ().text = damages >= 0 ? ""+damages : "+"+damages*-1;
	
	}

	public void runHotAcions(){


		Stack<Action> stack = this._characterStats.hotActionsStack;
		List<Action> actions = new List<Action>();

		Debug.Log ("stack len = " + stack.Count);

		while(stack.Count > 0)
			actions.Add(stack.Pop());

		actions.Reverse ();
		StartCoroutine("runNextHotAction", actions);
	}

	IEnumerator runNextHotAction(object o){

		List<Action> actions = (List<Action>)o;

		foreach(Action a in actions){
			Debug.Log ("# Running action " + a.key + " Waiting : " + a.actionCost);
			a.onActionRunning(this);
			yield return new WaitForSeconds(a.actionCost);
		}


	}

	[RPC]
	public void enterFightMode(NetworkViewID id)
	{
		if(Network.isServer)
		{
			if (!this._isInFight)
			{
				this._isInFight = true;
				this._characterStats.gameMode = 2;
				networkView.RPC("enterFightMode", RPCMode.Others, id);
			}
		}
		else
		{
			this._characterStats.gameMode = 2;
			this._isInFight = true;
		}
	}

	// Get / Seters
	public GameObject character{
		get {
			return this._character;
		}
	}
	public CharacterStats characterStats{
		get {
			return this._characterStats;
		}
	}
	public bool isInFight{
		get {
			return this._isInFight;
		}
	}


}
