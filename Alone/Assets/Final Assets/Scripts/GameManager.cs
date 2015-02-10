using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameManager : MonoBehaviour {

	/* AI Entity Prefab */
	[SerializeField]
	private GameObject _basicEntityPrefab;

	/* Misc */
	[SerializeField]
	Light _light;

	/* GUI */
	[SerializeField]
	GameObject _playerDesktopGUIObject;

	[SerializeField]
	GameObject _gameMasterGUIObject;
	
	/* Rooms */
    [SerializeField]
    private List<GameObject> _roomList;

	/* STATIC */
	private static Color _FREE_MODE_COLOR = new Color(1f,1f,1f); // index : 1
	private static Color _PLANIF_MODE_COLOR = new Color(0,0,1f); // index : 2
	private static Color _SPECTATOR_MODE_COLOR = new Color(1f,0,0); // index : 3
	private static bool _LOG_STEPS = false;
	/* Game Steps */
	// 1 : Free
	// 2 : Planif
	// 3 : Fight
	private int _playerGameStep;
	/* GM Game Stes */
	// 1 : Pose
	// 2 : ?
	private int _gameMasterGameStep;
	private List<Fight> _fights;
	private List<Round> _currentRounds;
	private bool _roundInProgress;

	private Dictionary<NetworkViewID, int> _playersReadyMap;
	private Dictionary<NetworkViewID, int> _AIReadyMap;

	private List<AIEntityData> _hiddenEntities;
	private List<AIEntityData> _gameMastersMinions;

	private Player _gameMaster;
	private bool _gameMasterReady;

	private bool _withoutGameMasterMode;


    private int _playerValidateCount = 0;
	private int _hotActionsStartedCount = 0;
	private int _hotActionsEndedCount = 0;

    private int currentRoomBattle = 1;



	void Start () {
		_light.color = _FREE_MODE_COLOR;
		_playersReadyMap = new Dictionary<NetworkViewID, int> ();
		_AIReadyMap = new Dictionary<NetworkViewID, int> ();
		_hiddenEntities = new List<AIEntityData> ();
		_gameMastersMinions = new List<AIEntityData>();

		_withoutGameMasterMode = true;
		_playerGameStep = 1;
		_gameMasterGameStep = 1;
        closeAllRoom();
	}

	public IPlayerGUI instanciateAndGetPlayerGUI(){
		GameData.init();
		_playerDesktopGUIObject = (GameObject)Instantiate(_playerDesktopGUIObject);
		IPlayerGUI gui = _playerDesktopGUIObject.GetComponent<PlayerDesktopGUIScript>();
		return gui;
	}
	public IPlayerGUI instanciateAndGetGameMasterGUI(){
		GameData.init();
		_playerDesktopGUIObject = (GameObject)Instantiate(_gameMasterGUIObject);
		IPlayerGUI gui = _playerDesktopGUIObject.GetComponent<GameMasterGUIScript>();
		return gui;
	}

   
   /*
    * Step 0 : Fight is over (FREE mode)
    */
	[RPC]
	public void fightIsOver(){
		if(GameData.myself.isGM){
			// TODO : check for LAST room
		} else {
			GameData.myself.resetFight();
		}

		GameData.getActionHelperDrawer().deleteAllHelpers();
		GameData.getActionHelperDrawer().deleteAllAIEntityHelpers();

		_playersReadyMap.Clear();
		_gameMastersMinions.Clear();
		_hiddenEntities.Clear();
				
		_playerGameStep = 1;
		_gameMasterGameStep = 1;
		_light.color = _FREE_MODE_COLOR;

	}
	/*
	 * Step 1 : Entering in room (Fight Mode)
	 */
	public void playerEnteredRoom(){
		if(_playerGameStep != 1){
			Debug.LogError("playerEnteredRoom _playerGameStep is not 'FREE' : " + _playerGameStep);
		}

		networkView.RPC("initNextRound", RPCMode.All);
	}


	/*
	 * Step 2 : Check for next Round
	 */
	[RPC] // All
	public void initNextRound(){

		if(_LOG_STEPS) Debug.Log ("# 2 : initNextRound");
		if(_playerGameStep == 2){
			Debug.LogError("initNextRound : Player GameStep : " +_playerGameStep);

		}
		if(_gameMastersMinions.Count <= 0){
			Debug.LogError("GameMaster hasnt posed units");
		}

		_playersReadyMap.Clear();

		/* 
		 * Victory / Loose conditions check
		 */

		// Check if players lost
		int deadPlayer = 0;
		foreach(Player p in GameData.getPlayerList()){
			if(p.characterManager.characterStats.isDead)
				deadPlayer ++;
		}
		if(deadPlayer == GameData.getNonGMPlayerCount()){
			Debug.Log("Players lost !");
			foreach(Player p in GameData.getPlayerList()){
				p.hasLost();
			}
			return;
		}

		// Check if gamemaster lost Round
		int deadMinion = 0;
		foreach(AIEntityData e in _gameMastersMinions){
			CharacterManager cm = e.tryGetCharacterManager();
			if (null == cm){
				Debug.LogError("GM minion missing CharacterManager !!!");
				continue;
			}
			if(cm.characterStats.isDead){
				deadMinion ++;
			}
		}
		if(deadMinion == _gameMastersMinions.Count){
			networkView.RPC ("fightIsOver", RPCMode.All);
			return;
		}

		/* 
		 * Reseting stats
		 */
		if (GameData.myself.isGM) {
			GameData.myself.gui.changeGameMode(1);
			
		} else {
			GameData.myself.gui.changeGameMode(2);
			GameData.myself.characterManager.lineRenderer.SetVertexCount(0);
			GameData.myself.characterManager.isInFight = true;
		}
		

		
		// Reset AP for Minions
		foreach(AIEntityData e in _gameMastersMinions){
			CharacterManager cm = e.tryGetCharacterManager();
			if(null == cm)
				Debug.LogError("CharacterManager for AIEntityData is null ..");
			else
				cm.networkView.RPC("resetActionPointRPC", RPCMode.All);
		}

		// Init AI
		foreach(AIEntityData e in _gameMastersMinions){ 
			IABase ai = e.instanciateObject.GetComponent<IABase>();
			ai.enabled = true;
			if(ai == null){
				Debug.LogError("AIBase compo. is NULL");
				continue;
			}
			
			ai.runPlanification(e);
		}

		// Reset Data for player and gm
		GameData.myself.onInitNextRound();

		GameData.getActionHelperDrawer().deleteAllAIEntityHelpers();
		GameData.getActionHelperDrawer().deleteAllHelpers();
		_playerGameStep = 2;
		_gameMasterGameStep = 1;
		_light.color = _PLANIF_MODE_COLOR;

		// Game Master move to next Room
		networkView.RPC ("setGameMasterPosePoint", RPCMode.All, GameData.getCastedGameMasterPlayer().maxPosePoint);
	
	}

	/*
	 * Step 3 : Round
	 */
	/* 3.1 : Wait for players clicking Ready Button */
	[RPC] // All
	public void addReadyPlayer(NetworkViewID id, int count){
		if(_LOG_STEPS)Debug.Log ("# 3.1 : addReadyPlayer");
		if(_playerGameStep != 2) {
			Debug.LogError("addReadyPlayer called for non PLANIF mode : " + _playerGameStep);
		}



		bool f = _playersReadyMap.ContainsKey (id);
		if (!f) {
			Player next = GameData.getPlayerByNetworkViewID(id);

			_playersReadyMap.Add(next.id, count);//next.characterManager.characterStats.hotActionsStack.Count);
		} else {
			Debug.LogError("(Player) Key " + id + " aleady in map");
		}

		showMap ();

		if(Network.isServer)
			checkPlayersAndGameMasterReady();

	}
	[RPC]// All 
	public void addReadyAI(NetworkViewID id, int count){
		if(_LOG_STEPS)Debug.Log ("# 3.1(bis) : addReadyAI");
		if(_playerGameStep != 2) {
			Debug.LogError("addReadyAI called for non PLANIF mode : " + _playerGameStep);
		}
		
		
		
		bool f = _AIReadyMap.ContainsKey (id);
		if (!f) {
			//Player next = GameData.getPlayerByNetworkViewID(id);
			
			_AIReadyMap.Add(id, count);//next.characterManager.characterStats.hotActionsStack.Count);
		} else {
			Debug.LogError("(AI) Key " + id + " aleady in map");
		}
		
		showMap ();
		
		if(Network.isServer)
			checkPlayersAndGameMasterReady();
	}
	private void checkPlayersAndGameMasterReady(){
		bool rdy = true;
		// Players ready ?
		if (_playersReadyMap.Count != GameData.getNonGMPlayerCount ()) {
			rdy = false;
		}
		/* 
		// AIs ready ?
		if (_AIReadyMap.Count != _gameMastersMinions.Count) {
			rdy = false;
		}
		*/
		if(rdy){
			networkView.RPC ("startNextRound", RPCMode.All);
		}
	}
	[RPC] // Server
	private void playerAreReady(){
		networkView.RPC ("startNextRound", RPCMode.All);
	}
	/* 3.2 : Run Next Round */
	[RPC]// All
	private void startNextRound(){
		if(_LOG_STEPS)Debug.Log ("# 3.2 : startNextRound");
		_roundInProgress = true;

		_playerGameStep = 3;
		_gameMasterGameStep = 1;

		if(GameData.myself.isGM){

		}else {
			GameData.myself.characterManager.networkView.RPC("removePendingActionRPC", RPCMode.All);
			GameData.myself.gui.changeGameMode(3);
			GameData.myself.characterManager.runHotAcions();
		}

		foreach(AIEntityData e in _gameMastersMinions){
			CharacterManager cm = e.tryGetCharacterManager();
			if(null == cm)
				Debug.LogError("CharacterManager for AIEntityData is null ..");
			cm.runHotAcions();
		}

		GameData.getActionHelperDrawer().deleteAllAIEntityHelpers();
		GameData.getActionHelperDrawer().deleteAllHelpers();

		_light.color = _SPECTATOR_MODE_COLOR;
		
		checkIfRoundIsOver ();

	}

	[RPC]
	public void hotActionProcessed (NetworkViewID id, bool fromAI){
		if(_playerGameStep!= 3){
			Debug.LogError("hotActionProcessed called in non SPECTATOR step : " + _playerGameStep);
		}
		Player owner = null;
		if(fromAI){
			owner = GameData.getPlayerByNetworkViewID(id);
		} else {
			owner = GameData.getGameMasterPlayer();
		}

		bool roundIsOver = true;

		if (fromAI) {
			if (_AIReadyMap.ContainsKey(id)){
				_AIReadyMap[id] -= 1;
				
				Debug.Log("AI (owner : " + owner.name + ") remaining actions : " + _AIReadyMap[id]);
				
			} else {
				Debug.LogError("[hotActionProcessed] _AIReadyMap doesnt contains AI " + id + "(owner : " + owner.name);
			}
		} else {
			if (_playersReadyMap.ContainsKey(id)){
				_playersReadyMap[id] -= 1;

				Debug.Log("Player " + owner.name + " remaining actions : " + _playersReadyMap[id]);

			} else {
				Debug.LogError("[hotActionProcessed] _playersReadyMap doesnt contains Player " + owner.name);
			}
		}

		checkIfRoundIsOver ();
	}
	private void checkIfRoundIsOver(){
		bool f = true;
		// Players
		foreach (KeyValuePair<NetworkViewID, int> kvp in _playersReadyMap) {
			Debug.Log("(Player) kvp.value == " + kvp.Value);
			if(kvp.Value != 0){
				f = false;
				break;
			}
		}
		/*
		// AI
		foreach (KeyValuePair<NetworkViewID, int> kvp in _AIReadyMap) {
			Debug.Log("(AI) kvp.value == " + kvp.Value);
			if(kvp.Value != 0){
				f = false;
				break;
			}
		}
		*/

		if(!f)
			return;
		if(_LOG_STEPS)Debug.Log ("Round is OVER");
		// At this point, round is OVER
		StartCoroutine ("resetRound");
	}

	IEnumerator resetRound(){

		yield return new WaitForSeconds (1);
		Debug.Log ("Reset Round");
		_playersReadyMap.Clear();
		_playersReadyMap = new Dictionary<NetworkViewID, int> ();

		_gameMasterReady = false;

		networkView.RPC("initNextRound", RPCMode.All);
	}


	/*
	 * Misc
	 */
	[RPC]
	public void changeLightForAllPlayer(int index){
		if(1 == index)
			_light.color = _FREE_MODE_COLOR;
		else if(2 == index)
			_light.color = _PLANIF_MODE_COLOR;
		else if (3 == index)
			_light.color = _SPECTATOR_MODE_COLOR;
	}


	[RPC]
	public void changeGUIModeForPlayer(NetworkViewID id, int mode){
		Player p = GameData.getPlayerByNetworkViewID (id);
		p.gui.changeGameMode (mode);
	}


	/*
	 * Rooms
	 */
	public roomBattleModeScript getRoomNumber(int number)
	{
		foreach (GameObject roomHandler in this._roomList)
		{
			roomBattleModeScript room = roomHandler.GetComponent<roomBattleModeScript>();
			if (room.roomNumber == number)
				return room;
		}
		return null;
	}

	private void closeAllRoom()
	{
		foreach(GameObject room in _roomList)
		{
			ConnectRoomScript script = room.GetComponent<ConnectRoomScript>();
			if(script)
				script.setIsOpen(false);
		}
	}

	[RPC]
	public void openRoomNumber(int number)
	{
		if (number > 0 && number <= this._roomList.Count)
		{
			GameObject room = this._roomList[number - 1];
			ConnectRoomScript script = room.GetComponent<ConnectRoomScript>();
			if (script)
				script.setIsOpen(true);
			if (Network.isServer)
				networkView.RPC("openRoomNumber", RPCMode.Others, number);

		}

	}
	[RPC] // All
	public void moveGMToPosition(Vector3 pos){
		if(GameData.myself.isGM){
			GameData.myself.playerObject.transform.position = pos;
		} else{
			GameData.getGameMasterPlayer().playerObject.transform.position = pos;
		}
	}
	/*
	 * AI Entity RPC
	 */
	[RPC] // All
	public void instanciateHiddenEntities(){

		GameData.getActionHelperDrawer().deleteAllAIEntityHelpers();

		foreach(AIEntityData e in _hiddenEntities){
			Vector3 safePos = e.initPos;
			safePos.y = 1.2f;
			GameObject go = (GameObject) Instantiate(e.prefab, safePos, Quaternion.identity);

			CharacterManager cm = go.GetComponent<CharacterManager>();

			// Logic Stats
			CharacterStats stats = new CharacterStats(cm.networkView, e.life, e.actionPoint, e.str);
			stats.addTargetType(CharacterStats.TargetType.ai);

			// CharacManager Step 2 : Instantiate (logic data)
			cm.initialize(stats,e.owner, null);
			e.instanciateObject = go;
			e.characterManager = cm;

			_gameMastersMinions.Add(e);
		}
		_hiddenEntities.Clear();
		_hiddenEntities = new List<AIEntityData>();
	}

	[RPC] // All
	public void addHiddenBasicEntity(Vector3 pos, NetworkViewID id){
		_hiddenEntities.Add (new AIEntityData(_basicEntityPrefab, pos, GameData.getPlayerByNetworkViewID(id), 40,2,3f));
	}

	/*
	 *  Game Master Stuff
	 */
	[RPC] // All
	public void setGameMasterPosePoint(int value){
		if (GameData.myself.isGM) {
			GameMasterPlayer gm = (GameMasterPlayer)GameData.myself;
			gm.currPosePoint = value;
		} else {
			GameData.getCastedGameMasterPlayer().currPosePoint = value;
		}
	}





	/*

	[RPC]
	public void enterFightMode(NetworkViewID id)
	{
		Debug.Log("Enter Planif Mode");

		// PLANIF COLOR
		networkView.RPC("changeLightForAllPlayer", RPCMode.All, 2);
		networkView.RPC("deleteHelpers", RPCMode.All);
		networkView.RPC("switchToFightMode", RPCMode.All);

		Player player = GameData.getPlayerByNetworkViewID (id);
		CharacterManager cm = player.characterManager;

		if (Network.isServer)
		{
			getRoomNumber(currentRoomBattle).beginBattleMode();
			cm.isInFight = true;
			cm.lineRenderer.SetVertexCount(0);
			networkView.RPC("enterFightMode", RPCMode.Others, id);
		}
		else
		{
			cm.isInFight = true;
			cm.lineRenderer.SetVertexCount(0);
			
		}
	}
	*/



	// Units
	public void showMap(){
		int cpt = 1;
		Debug.Log ("Ready Player Map :: ");
		foreach (KeyValuePair<NetworkViewID, int> kvp in _playersReadyMap) {
			String msg = cpt + " : " + GameData.getPlayerByNetworkViewID(kvp.Key) + " Actions : " + kvp.Value;

			Debug.Log(msg);
			cpt ++;
		}
	}


	
	/*
	 *  GET / SET
	 */
	public List<AIEntityData> gameMastersMinions{
		get {
			return _gameMastersMinions;
		}
	}

	public List<AIEntityData> hiddenEntities{
		get {
			return _hiddenEntities;
		}
	}

	public int gameMasterGameStep {
		get {

			return _gameMasterGameStep;
		}
	}
   	
	public int playerGameStep {
		get {
			return _playerGameStep;
		}
	}
}
