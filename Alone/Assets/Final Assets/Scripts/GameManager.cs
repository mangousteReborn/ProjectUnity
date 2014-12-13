using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
	// STUFF STUFF !!
	
	[SerializeField]
	GameObject _playerDesktopGUIObject;

	// TODO : GameMasterGUIScript !!

	// Players GUI Pattern
	private PlayerDesktopGUIScript _playerDesktopGUIScript;


	// Current player GUI (depending of player type (classic or game master)
	private IPlayerGUI _currentPlayerGUI;

	void Start () {

		GameData.init();
		
		// TODO : Check if current player is "classic" or "game master"
		_playerDesktopGUIObject = (GameObject)Instantiate(_playerDesktopGUIObject);
		_playerDesktopGUIScript = _playerDesktopGUIObject.GetComponent<PlayerDesktopGUIScript>();

		_currentPlayerGUI = _playerDesktopGUIScript;
	}

	public IPlayerGUI playerGUI {
		get {
			return this._currentPlayerGUI;
		}
	}
}
