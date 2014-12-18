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

    public void increaseReadyPlayer(object sender, EventArgs e)
    {
        _playerValidateCount += 1;
        int playerCount = GameData.getNonGMPlayerCount();;
        if (_playerValidateCount == playerCount)
            _playerDesktopGUIScript.broadcastStartSimulation();
    }

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

        _playerDesktopGUIScript.readyPlayer += increaseReadyPlayer;
        _playerValidateCount = 0;
    }

    [RPC]
    public void enterFightMode(NetworkViewID id)
    {
        Debug.Log("enterFightMode");
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
