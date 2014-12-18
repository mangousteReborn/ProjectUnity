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

	private bool _initialized = false;
	private CharacterStats _characterStats;// = new CharacterStats (null,200,20f);
	private Player _player;
	private Material _material;
	private bool _isInFight = false;


	void Start () {

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

	public void initialize(CharacterStats stats, Player p, Material mat){
		if(this._initialized)
			return;
		this._initialized = true;
		this._characterStats = stats;
		this._player = p;
		this._material = mat;

		


		// Instanciate GO
		this._healthBar = (GameObject)Instantiate (this._healthBar);
		this._healthBar.GetComponent<HealthbarScript>().setCharacterStats(this._characterStats);

		// Listeners
		this._characterStats.register (CharacterStatsEvent.currentLifeChange, onCurrentLifeChange);

		// Misc
		this._character.GetComponent<MeshRenderer>().material = this._material;
	}

	private void onCurrentLifeChange(CharacterStats stats, object[] param){
		int damages = (int)param [0] - (int)param [1];
		Color color = damages >= 0 ? Color.red : Color.green;

		GameObject popup = (GameObject)Instantiate (this._damagePopup, this._character.transform.position,  Quaternion.identity);


		popup.GetComponentInChildren<Text> ().color = color;
		popup.GetComponentInChildren<Text> ().text = damages >= 0 ? ""+damages : "+"+damages*-1;
	
	}

	public void runHotAcions(){


		Stack<Action> stack = this._characterStats.hotActionsStack;
		List<Action> actions = new List<Action>();


		while(stack.Count > 0)
			actions.Add(stack.Pop());

		actions.Reverse ();
		StartCoroutine("runNextHotAction", actions);
	}

	IEnumerator runNextHotAction(object o){

		List<Action> actions = (List<Action>)o;
		GameData.getGameManager ().networkView.RPC ("hotActionsStarted",RPCMode.All, this.networkView.viewID);
		foreach(Action a in actions){
			Debug.Log ("# Running action '" + a.key + "' Waiting : " + a.actionCost);
			a.onActionStart(this);
			yield return new WaitForSeconds(a.actionCost);
		}
		GameData.getGameManager ().networkView.RPC ("hotActionsEnded",RPCMode.All, this.networkView.viewID);

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
	public Material material{
		get {return this._material;}
		set {
			this._material = value;

			this._character.GetComponent<MeshRenderer>().material = this._material;

		}
	}

	
	/*
	 * 	###########
	 *  ### RPC ###
	 * 	###########
	 */
	 
    [RPC]
    private void setBonusVignetteHandlerRPC(string key, bool isFromRPC)
    {
        this._characterStats.setBonusVignetteRPC(key, isFromRPC);
    }


    [RPC]
    private void setLife(int value)
    {
        this._characterStats.setCurrentLife(value,true);
    }

	[RPC]
	private void setCurrentLife(int value)
	{
		this._characterStats.setCurrentLife(value,true);
	}

	[RPC]
	private void setCurrentActionPoint(float value,bool isFromRPC){
        this._characterStats.setCurrentActionPoint(value, isFromRPC);
	}

	[RPC]// GENERAL (Works !)
	public void setPendingActionByKeyRPC(string k ){
		this._characterStats.setPendingAction(GameData.getCopyOfAction(k));
	}
	/*
	[RPC] // DirectDamage
	public void pushDirectDamageActionAsPendingRPC(string k, string name, string d, float costpu,  float degree, float radius, int damages){
		this._characterStats.setDirectDamageActionAsPending(k, name, d, costpu, degree, radius, damages);
	}
	*/
	[RPC]// Move
	public void pushMoveActionAsPendingRPC(string k, string name, string d, float costpu){
		this._characterStats.setPendingActionAsMoveAction(k, name, d, costpu);
	}
	[RPC]// Wait
	public void pushWaitActionAsPendingRPC(string k, string name, string d, float costpu){
		this._characterStats.setPendingActionAsWaitAction(k, name, d, costpu);
	}


	[RPC]
	public void pushMoveActionRPC(string k, string name, string d, float costpu, float totalCost, Vector3 startPos, Vector3 destPosition){
		this._characterStats.pushMoveAction(k, name, d, costpu, totalCost, startPos, destPosition);
	}
	[RPC]
	public void pushWaitActionRPC(string k, string name, string d, float costpu, float totalCost, Vector3 startPos, Vector3 destPosition){
		this._characterStats.pushWaitAction(k, name, d, costpu, totalCost, startPos, destPosition);//destPosition
	}
	[RPC]
	public void pushDirectDamageActionRPC(string k, float totalCost, Vector3 startPos, Vector3 destPosition, float angle){
		DirectDamageAction a = (DirectDamageAction)GameData.getCopyOfAction(k);
		a.actionCost = totalCost;
		a.endPosition = destPosition;
		a.startPosition = startPos;
		a.definedAngle = angle;

		this._characterStats.pushHotAction (a);
	}

	[RPC]
	public void removePendingActionRPC(){
		this._characterStats.removePendingAction();
	}

	[RPC]
	public void validateDirectDamageActionRPC(NetworkViewID id, Vector3 ePos, float angle){
		if (!Network.isServer)
			return;
		
		Player playerWhoValidate = GameData.getPlayerByNetworkViewID (id);
		
		DirectDamageAction ma = null;
		
		try{
			ma = (DirectDamageAction)playerWhoValidate.characterManager.characterStats.pendingAction;
			
		} catch (Exception e){
			Debug.LogError("CharacterManager : <validateDirectDamageActionRPC> Player doest have pending action or action is not a DirectDamage : " + e.ToString());
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

			object[] p = {1}; // 0 for MeshCollider mode; 1 for SphereCollider mode
			ma.onActionValidation(this,p);
			
			GameData.getActionHelperDrawer().networkView.RPC("pushDirectDamageStaticHelperRPC",  RPCMode.All,playerWhoValidate.id, sPos, sPos, ma.name + "\n" + cost+"s", ma.degree, ma.radius, angle);
			this.networkView.RPC("pushDirectDamageActionRPC", RPCMode.All,  ma.key,cost,sPos, sPos, angle);

			this.networkView.RPC("removePendingActionRPC", RPCMode.All);
			
		} else {
			ma.onActionInvalid(this,null);
		}
	}

	[RPC]
	public void validateWaitActionRPC(NetworkViewID id, Vector3 ePos){
		if (!Network.isServer)
			return;
		
		Player playerWhoValidate = GameData.getPlayerByNetworkViewID (id);
		
		WaitAction ma = null;
		
		try{
			ma = (WaitAction)playerWhoValidate.characterManager.characterStats.pendingAction;
			
		} catch (Exception e){
			Debug.LogError("CharacterManager : <validateMoveAction> Player doest have pending action or action is not a WaitAction : " + e.ToString());
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
			
			GameData.getActionHelperDrawer().networkView.RPC("pushDefaultStaticHelperRPC",  RPCMode.All,playerWhoValidate.id, sPos, sPos, ma.name + "\n" + cost+"s");
			this.networkView.RPC("pushWaitActionRPC", RPCMode.All,  ma.key,  ma.name, ma.desc, ma.costPerUnit, cost, sPos, sPos);
			this.networkView.RPC("removePendingActionRPC", RPCMode.All);
			
		} else {
			ma.onActionInvalid(this,null);
		}
	}

	[RPC] 
	public void validateMoveActionRPC(NetworkViewID id, Vector3 ePos){
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
			
			GameData.getActionHelperDrawer().networkView.RPC("pushDefaultStaticHelperRPC",  RPCMode.All,playerWhoValidate.id, sPos, ePos,  ma.name + "\n" + cost+"s");
			this.networkView.RPC("pushMoveActionRPC", RPCMode.All,  ma.key,  ma.name, ma.desc, ma.costPerUnit, cost, sPos, ePos);
			this.networkView.RPC("removePendingActionRPC", RPCMode.All);
		} else {
			ma.onActionInvalid(this,null);
		}

	}

	/*
	[RPC]
	public void validadePendingAction(NetworkViewID id){
		Player playerWhoValidate = GameData.getPlayerByNetworkViewID (id);

		Action pa = playerWhoValidate.characterManager.characterStats.pendingAction;
		if (null == pa) {
			Debug.LogWarning("CharacterManager : <validatePendingAcion> Player doest have pending action");
		}

		//float costRes = playerWhoValidate.characterManager.characterStats.currentActionPoint - pa.actionCost;


		
	}
	*/

}
