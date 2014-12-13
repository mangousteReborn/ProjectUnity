using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameManager : MonoBehaviour {

	[SerializeField]
	bool _forceGameMasterUI;

	[SerializeField]
	GameObject _playerDesktopGUIObject;

	[SerializeField]
	GameObject _gameMasterGUIObject;

    private int _playerValidateCount;

	// TODO : GameMasterGUIScript !!

	// Players GUI Pattern
	private PlayerDesktopGUIScript _playerDesktopGUIScript;

	private GameMasterGUIScript _gameMasterGUIScript;

	// Current player GUI (depending of player type (classic or game master)
	private IPlayerGUI _currentPlayerGUI;

	void Start () {
		GameData.init();

		// TODO : Check if current player is "classic" or "game master"
		_playerDesktopGUIObject = (GameObject)Instantiate(_playerDesktopGUIObject);
		_playerDesktopGUIScript = _playerDesktopGUIObject.GetComponent<PlayerDesktopGUIScript>();

		_currentPlayerGUI = _playerDesktopGUIScript;

        _playerDesktopGUIScript.readyPlayer += increaseReadyPlayer;
        _playerValidateCount = 0;
	}

	public IPlayerGUI playerGUI {
		get {
			return this._currentPlayerGUI;
		}
	}

    public void increaseReadyPlayer(object sender, EventArgs e)
    {
        _playerValidateCount += 1;
        int playerCount = GameData.getNonGMPlayerCount();
        Debug.Log(_playerValidateCount);
        Debug.Log(playerCount);
        if (_playerValidateCount == playerCount)
            _playerDesktopGUIScript.broadcastStartSimulation();
    }
}
