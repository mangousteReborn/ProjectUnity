using UnityEngine;
using System.Collections;

public class Player {

	private string _name;
    private NetworkView _networkView;
	private NetworkViewID _id;
	private Color _color;
    private bool _isGM;

	public Player(string name, NetworkView view, Color col,bool isGM=false){
		this._name = name;
        this._networkView = view;
		this._id = view.viewID;
		if (null == col) {
			col = new Color(0,0,0);
		}
		this._color = col;
        this._isGM = isGM;
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
    public bool isGM
    {
        get {return this._isGM;}
    }


}
