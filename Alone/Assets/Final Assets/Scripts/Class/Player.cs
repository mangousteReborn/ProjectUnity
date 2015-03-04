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


	public virtual void onInitNextRound(){
		if(null == this._characterManager){
			Debug.LogError("(onInitNextRound) _characterManager is not set for : " + this._name);
			return;
		}
		this._gui.changeGameMode(2);
		//this._characterManager.networkView.RPC("resetActionPointRPC", RPCMode.All);
		if(null !=this._gui){
			PlayerDesktopGUIScript pdgui = (PlayerDesktopGUIScript)this._gui;
			pdgui.setActionPointText(this._characterManager.characterStats.maxActionPoint, this._characterManager.characterStats.maxActionPoint);
		}
	}

	public virtual void resetFight(){
		if(null == this._characterManager){
			Debug.LogError("(resetFight) _characterManager is not set for : " + this._name);
			return;
		}
		
		this._characterManager.networkView.RPC("resetActionPointRPC", RPCMode.All);
		this._characterManager.isInFight = false;

		if(null !=this._gui){
			this._gui.changeGameMode(1);
			PlayerDesktopGUIScript pdgui = (PlayerDesktopGUIScript)this._gui;
			pdgui.setActionPointText(this._characterManager.characterStats.maxActionPoint, this._characterManager.characterStats.maxActionPoint);
		}

	}

	public virtual void hasLost(){
		if(this._gui != null)
			this._gui.changeGameMode(4);
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
