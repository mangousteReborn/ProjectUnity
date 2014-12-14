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
	private Player _player;
	private bool _isInFight = false;

	void Start () {
		this._healthBar = (GameObject)Instantiate (this._healthBar);

		this._healthBar.GetComponent<HealthbarScript>().setCharacterStats(this._characterStats);
			
		this._characterStats.register (CharacterStatsEvent.currentLifeChange, createPopup);


		 
		this._characterStats.networkView = this.networkView;
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.T)) {
			
			this._characterStats.setCurrentLife(this._characterStats.currentLife + (int)UnityEngine.Random.Range(-5,5),false);
		}
		if (Input.GetKeyDown (KeyCode.Y)) {
			
			this._characterStats.setCurrentLife(this._characterStats.currentLife + 1,false);
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
			a.onActionStart(this);
			yield return new WaitForSeconds(a.actionCost);
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
        set
        {
            this._isInFight = value;
        }
	}
	public Player player{
		set {
			this._player = value;
		}
		get {
			return this._player;
		}
	}

    [RPC]
    private void setLife(int value)
    {
        this._characterStats.setCurrentLife(value,true);
    }
	[RPC]
	private void setCurrentActionPoint(float value){
		this._characterStats.setCurrentActionPoint(value,true);
	}


	[RPC]
	public void pushMoveActionAsPendingRPC(string k, string name, string d, float costpu){
		Debug.Log ("Pending >>> push MoveActionAsPendingRPC");
		this._characterStats.setPendingActionAsMoveAction(k, name, d, costpu);
	}
	[RPC]
	public void pushMoveActionRPC(string k, string name, string d, float costpu, float totalCost, Vector3 startPos, Vector3 destPosition){
		this._characterStats.pushMoveAction(k, name, d, costpu, totalCost, startPos, destPosition);
	}

	[RPC]
	public void removePendingActionRPC(){
		Debug.Log ("Pending >>> remove PendingActionRPC");
		this._characterStats.removePendingAction();
	}

	[RPC] 
	public void validateMoveActionRPC(NetworkViewID id, Vector3 ePos){
		Debug.Log ("Trying to validation ");
		if (!Network.isServer)
			return;

		Player playerWhoValidate = GameData.getPlayerByNetworkViewID (id);

		MoveAction ma = null;

		try{
			ma = (MoveAction)playerWhoValidate.characterManager.characterStats.pendingAction;

		} catch (Exception e){
			Debug.LogError("CharacterManager : <validateMoveAction> Player doest have pending action or action is not a MoveAction : " + e.ToString());
			return;
		}
		Vector3 sPos;

		Action lastHotAction = playerWhoValidate.characterManager.characterStats.getLastHotAction();

		if (null == lastHotAction) {
			sPos = playerWhoValidate.playerObject.transform.position;
		}
		else {
			sPos = lastHotAction.endPosition;
		}
		
		float cost = ma.calculateCost (sPos, ePos);

		if (cost <= playerWhoValidate.characterManager.characterStats.currentActionPoint) {
			playerWhoValidate.characterManager.characterStats.setCurrentActionPoint(
				playerWhoValidate.characterManager.characterStats.currentActionPoint - cost,true);

			ma.onActionValidation(this,null);

            GameData.getActionHelperDrawer().networkView.RPC("pushDefaultStaticHelperRPC", RPCMode.All, playerWhoValidate.id, sPos, ePos, cost + "s");
            this.networkView.RPC("pushMoveActionRPC", RPCMode.All, ma.key, ma.name, ma.desc, ma.costPerUnit, cost, sPos, ePos);
            this.networkView.RPC("removePendingActionRPC", RPCMode.All);

		} else {
			ma.onActionInvalid(this,null);
		}

	}

	[RPC]
	public void validadePendingAction(NetworkViewID id){
		//TODO : genre validafing pending action (only for classic pending action whit predefined action cost
		Player playerWhoValidate = GameData.getPlayerByNetworkViewID (id);

		Action pa = playerWhoValidate.characterManager.characterStats.pendingAction;
		if (null == pa) {
			Debug.LogWarning("CharacterManager : <validatePendingAcion> Player doest have pending action");
		}

		float costRes = playerWhoValidate.characterManager.characterStats.currentActionPoint - pa.actionCost;


		
	}

}
