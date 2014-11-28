using UnityEngine;
using System.Collections;

public class PlayerGUIScript : MonoBehaviour {

	[SerializeField]
	GameObject _playerGUIObject;

	private CharacterManager _characterManager;

	void Start () {
		
	}

	private void buildUI(){
		if (_characterManager == null){
			Debug.LogWarning("Cannot build player's menu : CharacterManager is not set");
			return;
		}



	}

	public void setCharacterManager(CharacterManager cm){

		_characterManager = cm;

	}
}
