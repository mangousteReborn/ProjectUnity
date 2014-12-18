using UnityEngine;
using System.Collections;

public class Player {

	private string _name;

    private NetworkView _networkView;
	private NetworkViewID _id;
    private bool _isGM;

	private CharacterManager _characterManager;
	private GameObject _playerObject;

    public Player(string name, bool isGM )
    {
        this._name = name;

        this._isGM = isGM;
        this._characterManager = null;
    }

	public Player(string name, NetworkView view,bool isGM=false){
		this._name = name;
        this._networkView = view;
		this._id = view.viewID;
		
        this._isGM = isGM;
		this._characterManager = null;
	}

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
