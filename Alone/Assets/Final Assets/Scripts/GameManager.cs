using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
	// STUFF STUFF !!
	
	[SerializeField]
	GameObject _playerGUIObject;
	// TODO : GameMasterGUIScript !!

	private PlayerGUIScript _playerGUIScript;



	void Start () {
		GameData.init();
		// TODO : Check if current player is "classic" or "game master"
		//_playerGUIScript = (PlayerGUIScript) Instantiate(_playerGUIObject);
		_playerGUIObject = (GameObject)Instantiate(_playerGUIObject);
		_playerGUIScript = _playerGUIObject.GetComponent<PlayerGUIScript>();
	}
}
