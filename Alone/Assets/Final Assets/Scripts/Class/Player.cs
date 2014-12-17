using UnityEngine;
using System.Collections;

public class Player {

	private string _name;

    private NetworkView _networkView;
	private NetworkViewID _id;
	private Material _material;
	private Color _color;
    private bool _isGM;

	private CharacterManager _characterManager;
	private GameObject _playerObject;

	public Player(string name, NetworkView view, Material mat,bool isGM=false){
		this._name = name;
        this._networkView = view;
		this._id = view.viewID;

		this._material = mat;
		this._color = mat.color;
        this._isGM = isGM;
		this._characterManager = null;
	}

	public string name{
		get {return this._name;}
	}
	public NetworkViewID id{
		get {return this._id;}
	}
	public Color color{
		get{return this._color;}
	}
	public Material material{
		get{return this._material;}
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
