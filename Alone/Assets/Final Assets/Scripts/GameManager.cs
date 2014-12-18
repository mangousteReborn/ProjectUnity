using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameManager : MonoBehaviour {

	[SerializeField]
	GameObject _playerDesktopGUIObject;

	[SerializeField]
	GameObject _gameMasterGUIObject;

    [SerializeField]
    private List<GameObject> _roomList;

    private int _playerValidateCount;
	private int _hotActionsStartedCount = 0;
	private int _hotActionsEndedCount = 0;

	// TODO : GameMasterGUIScript !!

	// Players GUI Pattern
	private PlayerDesktopGUIScript _playerDesktopGUIScript;
	private GameMasterGUIScript _gameMasterGUIScript;

    private int currentRoomBattle = 1;

	// Current player GUI (depending of player type (classic or game master)
	private IPlayerGUI _currentPlayerGUI;

	void Start () {
	}

	public IPlayerGUI playerGUI {
		get {
			return this._currentPlayerGUI;
		}
	}

   	/*
		public void increaseReadyPlayer(object sender, EventArgs e)
	{
		Debug.Log ("ea == " + e);
		_playerValidateCount += 1;
		int playerCount = GameData.getNonGMPlayerCount();;
		if (_playerValidateCount == playerCount) {
			//this.networkView.RPC("runCurrentFightStep", RPCMode.All, );
			
			//_playerDesktopGUIScript.broadcastStartSimulation();
			
			_playerValidateCount = 0;
		}
	}
	*/

    public void setGMGui()
    {
        GameData.init();
        _playerDesktopGUIObject = (GameObject)Instantiate(_gameMasterGUIObject);
        _playerDesktopGUIObject.GetComponent<GameMasterGUIScript>().gm = this;
    }

    public void setPlayerGUI()
    {
        GameData.init();
        _playerDesktopGUIObject = (GameObject)Instantiate(_playerDesktopGUIObject);
        _playerDesktopGUIScript = _playerDesktopGUIObject.GetComponent<PlayerDesktopGUIScript>();

        _currentPlayerGUI = _playerDesktopGUIScript;

       // _playerDesktopGUIScript.readyPlayer += increaseReadyPlayer;
        _playerValidateCount = 0;
    }

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

	/*
	 *  Game Steps
	 */
	// Step 0
	[RPC]
	public void enterFightMode(NetworkViewID id)
	{
		Debug.Log("Enter Fight Mode");
		
		GameData.getActionHelperDrawer().deleteAllHelpers ();
		CharacterManager managerCharac = NetworkView.Find(id).gameObject.GetComponent<CharacterManager>();
		if (Network.isServer)
		{
			if (!managerCharac.isInFight)
			{
				getRoomNumber(currentRoomBattle).beginBattleMode();
				managerCharac.isInFight = true;
				managerCharac.characterStats.gameMode = 2;
				networkView.RPC("enterFightMode", RPCMode.Others, id);
			}
		}
		else
		{
			managerCharac.characterStats.gameMode = 2;
			managerCharac.isInFight = true;

		}
	}

	// Step 1
	[RPC]
	public void runCurrentFightStep(){
		Debug.Log("Next Fight Step");

		GameData.getActionHelperDrawer().deleteAllHelpers ();
		
		foreach(Player p in GameData.getPlayerList()){
            CharacterManager managerCharac = NetworkView.Find(p.id).gameObject.GetComponent<CharacterManager>();
            managerCharac.runHotAcions();
			managerCharac.characterStats.gameMode = 3;
			managerCharac.characterStats.removePendingAction();
			
		}
        if (Network.isServer) networkView.RPC("runCurrentFightStep", RPCMode.Others);
	}

    [RPC]
	public void runNextFightStep(){
		Debug.Log("Next Fight Step");

		GameData.getActionHelperDrawer().deleteAllHelpers ();
        foreach(Player p in GameData.getPlayerList())
        {
            CharacterManager managerCharac = NetworkView.Find(p.id).gameObject.GetComponent<CharacterManager>();
            managerCharac.characterStats.nextFightStep();
			managerCharac.characterStats.gameMode = 2;
        }
        if (Network.isServer) networkView.RPC("runNextFightStep", RPCMode.Others);
	}

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
