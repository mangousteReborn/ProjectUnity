using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameManager : MonoBehaviour {

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

	/* Game Steps */
	private List<Fight> _fights;
	private List<Round> _currentRounds;
	private bool _roundInProgress;

	private List<Player> _playersConnected;
	private Dictionary<NetworkViewID, int> _playersReadyMap;

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
		_playersConnected = new List<Player> ();
		_withoutGameMasterMode = true;
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
	 * Step 1 : Planif mode
	 */
	[RPC]
	public void enterFightMode(NetworkViewID id)
	{
		Debug.Log("Enter Planif Mode");

		// PLANIF COLOR
		networkView.RPC("changeLightForAllPlayer", RPCMode.All, 2);
		networkView.RPC("deleteAllHelpers", RPCMode.All);
		networkView.RPC("switchToFightMode", RPCMode.All);

		Player player = GameData.getPlayerByNetworkViewID (id);
		CharacterManager cm = player.characterManager;

		if (Network.isServer)
		{
			getRoomNumber(currentRoomBattle).beginBattleMode();
			cm.isInFight = true;
			cm.characterStats.gameMode = 2;
			cm.lineRenderer.SetVertexCount(0);
			networkView.RPC("enterFightMode", RPCMode.Others, id);
		}
		else
		{
			cm.isInFight = true;
			cm.characterStats.gameMode = 2;
			cm.lineRenderer.SetVertexCount(0);
			
		}
	}

	/*
	 * Step 2 : Round
	 */
	/* 2.1 : Wait for players + GM being ready */
	[RPC]
	public void addReadyPlayer(NetworkViewID id){
		bool f = _playersReadyMap.ContainsKey (id);
		if (!f) {
			Player next = GameData.getPlayerByNetworkViewID(id);

			_playersReadyMap.Add(id, next.characterManager.characterStats.hotActionsStack.Count);
		} else {
			Debug.LogError("Player " + GameData.getPlayerByNetworkViewID(id).name + " aleady ready");
		}

		checkPlayersAndGameMasterReady();

	}
	[RPC]
	public void gameMasterReady(){
		if (this._gameMasterReady) {
			Debug.LogWarning("Game master already ready");
			return;
		}

		//this._gameMaster = GameData.getGameMasterPlayer ();
		this._gameMasterReady = true;

		checkPlayersAndGameMasterReady ();
	}
	private void checkPlayersAndGameMasterReady(){
		if (_playersReadyMap.Count == GameData.getNonGMPlayerCount () &&
		    (_gameMasterReady)) {
			startNextRound();
		}
	}

	/* 2.2 : Run Next Round */
	[RPC]
	private void startNextRound(){
		_roundInProgress = true;
		Debug.Log ("Start next round");

		networkView.RPC("deleteAllHelpers", RPCMode.All);

		// Check if GameMaster Mobs are dead
		/*
		bool _gameMasterLostRound = true;

		foreach (GameObject enemy in getRoomNumber(currentRoomBattle).EnemyList)
		{
			NetworkViewID id = enemy.networkView.viewID;
			CharacterManager managerCharac = NetworkView.Find(id).gameObject.GetComponent<CharacterManager>();
			managerCharac.runHotAcions();
			managerCharac.characterStats.gameMode = 3;
			managerCharac.characterStats.removePendingAction();
		}
		*/

		// SPECTATOR COLOR
		networkView.RPC("changeLightForAllPlayer", RPCMode.All, 3);
		
		foreach (KeyValuePair<NetworkViewID, int> kvp in _playersReadyMap)
		{
			if(kvp.Value <= 0)
				continue;
			Player p = GameData.getPlayerByNetworkViewID(kvp.Key);
			if(null == p){
				Debug.LogError("[startNextRound] Trying to get unexisting player ID : " + kvp.Key);
				continue;
			}
			p.characterManager.characterStats.removePendingAction();
			p.characterManager.characterStats.gameMode = 3;
			p.characterManager.runHotAcions();
		}

		checkIfRoundIsOver ();

	}
	[RPC]
	public void hotActionProcessed (NetworkViewID ownerId){
		Player player = GameData.getPlayerByNetworkViewID(ownerId);
		bool roundIsOver = true;

		if (player.isGM) {
			// TODO
		} else {
			if (_playersReadyMap.ContainsKey(ownerId)){
				_playersReadyMap[ownerId] -= 1;

				Debug.Log("Player " + player.name + " remaining actions : " + _playersReadyMap[ownerId]);

			} else {
				Debug.LogError("[hotActionProcessed] _playersReadyMap doesnt contains Player " + player.name);
			}
		}

		checkIfRoundIsOver ();
	}
	private void checkIfRoundIsOver(){
		bool f = true;
		foreach (KeyValuePair<NetworkViewID, int> kvp in _playersReadyMap) {
			Debug.Log("kvp.value == " + kvp.Value);
			if(kvp.Value != 0){
				f = false;
				break;
			}
		}
		// TODO check GM !

		if(!f)
			return;

		// At this point, round is OVER
		StartCoroutine ("resetRound");
	}

	IEnumerator resetRound(){

		yield return new WaitForSeconds (1);
		Debug.Log ("Reset Round");
		_playersReadyMap = new Dictionary<NetworkViewID, int> ();
		_gameMasterReady = false;

		foreach (Player player in GameData.getPlayerList())
		{
			if (Network.isServer)
				enterFightMode(player.id);
			else
				this.networkView.RPC("enterFightMode", RPCMode.Server, player.id);
		}
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

	[RPC]
	public void deleteAllHelpers(){
		GameData.getActionHelperDrawer ().deleteAllHelpers ();
	}

	[RPC]
	public void resetRoundForPlayer(NetworkViewID id){
		Player p = GameData.getPlayerByNetworkViewID (id);
		p.resetRound ();
	}

	[RPC]
	public void switchToFightMode(){
		//GameData.getGameMasterPlayer ().enterFightMode ();
		foreach (Player p in GameData.getPlayerList()) {
			if(p.isGM){
				//TODO
				continue;
			}
			p.enterFightMode();
		}
	}
	/*
	 * Rooms
	 */
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













	/*
	[RPC]
	public void increaseReadyPlayer(NetworkViewID id)
	{
		_playerValidateCount += 1;
		int playerCount = GameData.getNonGMPlayerCount();;
		if (_playerValidateCount == playerCount) {
			runCurrentFightStep();
			_playerValidateCount = 0;
		}
	}

	public void hotActionsStarted(){

		_hotActionsStartedCount += 1;
		Debug.Log ("Action start " + _hotActionsStartedCount);
	}

	public void hotActionsEnded(){
		_hotActionsEndedCount += 1;
		Debug.Log ("Action end " + _hotActionsEndedCount + " / " + _hotActionsStartedCount);
		if(_hotActionsEndedCount >= _hotActionsStartedCount){
			runNextFightStep();
			_hotActionsStartedCount = 0;
			_playerValidateCount = 0;
		}
		
	}

    [RPC]
    public void runCurrentFightStep()
    {
        Debug.Log("Next Fight Step");

        GameData.getActionHelperDrawer().deleteAllHelpers();

        foreach (Player p in GameData.getPlayerList())
        {
            CharacterManager managerCharac = NetworkView.Find(p.id).gameObject.GetComponent<CharacterManager>();
            managerCharac.runHotAcions();
            managerCharac.characterStats.gameMode = 3;
            managerCharac.characterStats.removePendingAction();
        }

        foreach (GameObject enemy in getRoomNumber(currentRoomBattle).EnemyList)
        {
            NetworkViewID id = enemy.networkView.viewID;
            CharacterManager managerCharac = NetworkView.Find(id).gameObject.GetComponent<CharacterManager>();
            managerCharac.runHotAcions();
            managerCharac.characterStats.gameMode = 3;
            managerCharac.characterStats.removePendingAction();
        }

        if (Network.isServer) networkView.RPC("runCurrentFightStep", RPCMode.Others);
    }

    [RPC]
    public void runNextFightStep()
    {
        Debug.Log("Next Fight Step");

        GameData.getActionHelperDrawer().deleteAllHelpers();
        foreach (Player p in GameData.getPlayerList())
        {
            CharacterManager managerCharac = NetworkView.Find(p.id).gameObject.GetComponent<CharacterManager>();
            managerCharac.characterStats.nextFightStep();
            managerCharac.characterStats.gameMode = 2;
        }

        foreach (GameObject enemy in getRoomNumber(currentRoomBattle).EnemyList)
        {
            NetworkViewID id = enemy.networkView.viewID;
            CharacterManager managerCharac = NetworkView.Find(id).gameObject.GetComponent<CharacterManager>();
            managerCharac.runHotAcions();
            managerCharac.characterStats.nextFightStep();
            //managerCharac.characterStats.gameMode = 2;
        }
        if (Network.isServer) networkView.RPC("runNextFightStep", RPCMode.Others);
    }
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
}
