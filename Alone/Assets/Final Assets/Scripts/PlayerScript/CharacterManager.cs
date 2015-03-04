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
	public GameObject _character;
	
	[SerializeField]
	private Transform _characterTransform;
		
	[SerializeField]
	private GameObject _damagePopup;

	[SerializeField]
	private GameObject _healthBar;


	[SerializeField]
	private LineRenderer _lineRenderer;

	private bool _initialized = false;

	private CharacterStats _characterStats;
	private Player _player; // Owner
	private Material _material;
	private bool _isInFight = false;

	private HealthbarScript _healthBarScript;

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
		if (Input.GetKeyDown (KeyCode.L)) {
			Vector3 mousepos = Input.mousePosition;
			mousepos.z = Camera.main.transform.position.y;
			
			Ray ray = Camera.main.ScreenPointToRay(mousepos);
			RaycastHit hit;
			Vector3 target = Vector3.zero;
			if (Physics.Raycast(ray, out hit))
			{
				target = hit.point;
				target.y = 0;
			}

			if(null != target){
				Debug.Log("A MA FIRIN MA LAZER !!!");
				GameData.getActionHelperDrawer().createStaticLazerRPC(this._characterTransform.position, target, 0.3f);
			}
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
		this._healthBarScript = this._healthBar.GetComponent<HealthbarScript> ();
		this._healthBarScript.setLife (stats.currentLife, stats.maxLife);
		// Listeners
		//this._characterStats.register (CharacterStatsEvent.currentLifeChange, onCurrentLifeChange);

		// Misc
		this._character.GetComponent<MeshRenderer>().material = this._material;
	}

	private void createPopup(String msg,Color col){
		GameObject popup = (GameObject)Instantiate (this._damagePopup, this._character.transform.position,  Quaternion.identity);

		popup.GetComponentInChildren<Text> ().color = col;
		popup.GetComponentInChildren<Text> ().text = msg;
	
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
		bool isAI = this._player.isGM;

		List<Action> actions = (List<Action>)o;
		foreach(Action a in actions){
			a.onActionStart(this);
			yield return new WaitForSeconds(a.actionCost);
			a.onActionEnd(this);

            if (Network.isServer)
				GameData.getGameManager().hotActionProcessed(this._player.id,isAI);
            else
				GameData.getGameManager().networkView.RPC("hotActionProcessed", RPCMode.Server, this.networkView.viewID, isAI);//this._player.id
		}
	}


	/*
	 * 	###########
	 *  ### RPC ###
	 * 	###########
	 */
	 /* Stats */
	[RPC] // ALL
	public void resetActionPointRPC(){
		this._characterStats.currentActionPoint = this._characterStats.maxActionPoint;

		if(this.player.gui != null)
			this._player.gui.updateGUI();

	}
	[RPC] // All
	public void inflictDamages(int value){
		Debug.Log("damages !");
		this._characterStats.currentLife -= value;
		if(this._characterStats.currentLife <= 0){
			this._characterStats.isDead = true;
		}

		this._healthBarScript.setLife(this._characterStats.currentLife, this._characterStats.maxLife);
		/*
		String msg = value >= 0 ? ""+value : "+"+value*-1;
		Color col = value >= 0 ? Color.red : Color.green;
		createPopup(msg,col);
		*/
		if(this.player.gui != null)
			this._player.gui.updateGUI();
	}
	[RPC]
	public void destroy(){
		Destroy (this._healthBar);
		Destroy (this.gameObject);
	}
	[RPC] // All
	public void setCurrentActionPointRPC(float value){
		this._characterStats.currentActionPoint = value;
		if(this.player.gui != null)
			this._player.gui.updateGUI();
	}

	[RPC]
	private void setLife(int value)
	{
		this._characterStats.setCurrentLife(value,true);
	}
	
	[RPC]
	private void setCurrentLifeHandlerRPC(int value,bool isFromRPC)
	{
		this._characterStats.setCurrentLife(value, isFromRPC);
	}


    [RPC]
    private void setBonusVignetteHandlerRPC(string key, bool isFromRPC)
    {
        this._characterStats.setBonusVignetteRPC(key, isFromRPC);
    }

	[RPC]
	private void setCurrentActionPoint(float value,bool isFromRPC){
        this._characterStats.setCurrentActionPoint(value, isFromRPC);
	}

	[RPC]// GENERAL (Works !)
	public void setPendingActionByKeyRPC(string k ){
		this._characterStats.setPendingAction(GameData.getCopyOfAction(k));
	}
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

	[RPC] // All
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
			float nextAPValue = playerWhoValidate.characterManager.characterStats.currentActionPoint - cost;
			playerWhoValidate.characterManager.characterStats.setCurrentActionPoint(nextAPValue ,true);

			object[] p = {1}; // 0 for MeshCollider mode; 1 for SphereCollider mode
			ma.onActionValidation(this,p);
			
			GameData.getActionHelperDrawer().networkView.RPC("pushDirectDamageStaticHelperRPC",  RPCMode.All,playerWhoValidate.id, sPos, sPos, ma.name + "\n" + cost+"s", ma.degree, ma.radius, angle);
			this.networkView.RPC("pushDirectDamageActionRPC", RPCMode.All,  ma.key,cost,sPos, sPos, angle);
			this.networkView.RPC("setCurrentActionPointRPC", RPCMode.All, nextAPValue);
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
			float nextAPValue = playerWhoValidate.characterManager.characterStats.currentActionPoint - cost;
			playerWhoValidate.characterManager.characterStats.setCurrentActionPoint(nextAPValue,true);
			
			ma.onActionValidation(this,null);
			
			GameData.getActionHelperDrawer().networkView.RPC("pushDefaultStaticHelperRPC",  RPCMode.All,playerWhoValidate.id, sPos, sPos, ma.name + "\n" + cost+"s");
			this.networkView.RPC("pushWaitActionRPC", RPCMode.All,  ma.key,  ma.name, ma.desc, ma.costPerUnit, cost, sPos, sPos);
			this.networkView.RPC("setCurrentActionPointRPC", RPCMode.All, nextAPValue);
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
			float nextAPValue = playerWhoValidate.characterManager.characterStats.currentActionPoint - cost;
			playerWhoValidate.characterManager.characterStats.setCurrentActionPoint(nextAPValue,true);
				
			ma.onActionValidation(this,null);
			
			GameData.getActionHelperDrawer().networkView.RPC("pushDefaultStaticHelperRPC",  RPCMode.All,playerWhoValidate.id, sPos, ePos,  ma.name + "\n" + cost+"s");
			this.networkView.RPC("pushMoveActionRPC", RPCMode.All,  ma.key,  ma.name, ma.desc, ma.costPerUnit, cost, sPos, ePos);
			this.networkView.RPC("setCurrentActionPointRPC", RPCMode.All, nextAPValue);
			this.networkView.RPC("removePendingActionRPC", RPCMode.All);
		} else {
			ma.onActionInvalid(this,null);
		}

	}

	// 
	// Get / Seters
	//
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
	public Transform characterTransform {
		get {return this._characterTransform;}
	}
	public LineRenderer lineRenderer {
		get {return this._lineRenderer;}
	}

	public GameObject healthBar {
		get {
			return _healthBar;
		}
	}
}
