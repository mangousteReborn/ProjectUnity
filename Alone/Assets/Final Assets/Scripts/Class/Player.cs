using UnityEngine;
using System.Collections;

public class Player {

	protected string _name;

	// Action Point
	protected float _maxAP;
	protected float _currAP;

	protected NetworkView _networkView;
	protected NetworkViewID _id;
	protected bool _isGM = false;

	protected uint _gameMode;

	protected IPlayerGUI _gui;
	protected CharacterManager _characterManager;
	protected GameObject _playerObject;

	public Player(string name, NetworkView view, IPlayerGUI gui=null){
		this._name = name;
        this._networkView = view;
		this._id = view.viewID;
		this._gui = gui;
		this._characterManager = null;
	}


	public virtual void enterFightMode(){

	}

	public virtual void resetRound(){
		Debug.Log ("Player reset Room");
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

	public IPlayerGUI gui {
		get {
			return _gui;
		}
		set {
			_gui = value;
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

	public float maxAP {
		get {
			return _maxAP;
		}
		set {
			_maxAP = value;
		}
	}
}
