using UnityEngine;
using System.Collections;

public class Player {

	private string _name;
	// Action point
	private float _maxAP;
	private float _currAP;

    private NetworkView _networkView;
	private NetworkViewID _id;
    private bool _isGM;

	private uint _gameMode;

	private IPlayerGUI _gui;
	private CharacterManager _characterManager;
	private GameObject _playerObject;

	public Player(string name, NetworkView view,bool isGM){
		this._name = name;
		this._networkView = view;
		this._id = view.viewID;
		this._maxAP = 0f;
		this._currAP = 0f;
		this._gui = null;
		this._isGM = isGM;
		this._characterManager = null;
	}

	public Player(string name, NetworkView view,bool isGM, IPlayerGUI gui){
		this._name = name;
        this._networkView = view;
		this._id = view.viewID;
		this._maxAP = 0f;
		this._currAP = 0f;
		this._gui = gui;
        this._isGM = isGM;
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

	public float maxAP {
		get {
			return _maxAP;
		}
		set {
			_maxAP = value;
		}
	}
	public float currAP {
		get {
			return _currAP;
		}
		set {
			_currAP = value;
		}
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
