using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActionHelperDrawer : MonoBehaviour {

	private List<IActionHelperComponent> _playerPendingHelpers;
	private List<IActionHelperComponent> _playerValidatedHelpers;

	// string : Player or IA helpers
	private Dictionary<string, List<IActionHelperComponent>> _othersHelpersMap;

	private IActionHelperComponent _currentPlayerHelper;

	private Dictionary<string, IActionHelperComponent> _helpersTypeList;

	private LineHelper lineHelperObject;

	// Use this for initialization
	void Start () {
		this._playerPendingHelpers = new List<IActionHelperComponent> ();
		this._playerValidatedHelpers = new List<IActionHelperComponent> ();
		this._othersHelpersMap = new Dictionary<string, List<IActionHelperComponent>>();

		this._helpersTypeList = new Dictionary<string, IActionHelperComponent> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void pushHelperForPlayer(Action a, CharacterManager cm, string type){
		Debug.Log ("HELPER PUSHED LOL");
	}


}
