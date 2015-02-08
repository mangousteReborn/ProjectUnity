using UnityEngine;
using System.Collections;

public class Player {

	protected string _name;

	protected NetworkView _networkView;
	protected NetworkViewID _id;
	protected bool _isGM = false;

	protected uint _gameMode;

	protected IPlayerGUI _gui;
	protected CharacterManager _characterManager;
	protected GameObject _playerObject;

	public Player(string name, NetworkView view){
		this._name = name;
		this._networkView = view;
		this._id = view.viewID;
		this._gui = null;
		

		this._characterManager = null;
	}

	public Player(string name, NetworkView view, IPlayerGUI gui){
		this._name = name;
        this._networkView = view;
		this._id = view.viewID;
		this._gui = gui;

		this._characterManager = null;
	}

	public void changeGameMode(uint mode){
		this._gameMode = mode;
		this._gui.changeGameMode(mode);

	}

	/* GET / SET */

	public string name{
		get {return this._name;}
	}

	public NetworkViewID id{
		get {return this._id;}
	}
    public bool isGM
    {
        get {return this._isGM;}
    }
	public CharacterManager characterManager {
		get {return this._characterManager;}
		set {this._characterManager = value;}
	}
	public GameObject playerObject{
		get {return this._playerObject;}
		set {this._playerObject = value;}
	}
}
