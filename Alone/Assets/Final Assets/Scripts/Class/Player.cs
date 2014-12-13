using UnityEngine;
using System.Collections;

public class Player {

	private string _name;
	private NetworkViewID _id;
	private Color _color;

	public Player(string name, NetworkViewID id, Color col){
		this._name = name;
		this._id = id;
		if (null == col) {
			col = new Color(0,0,0);
		}
		this._color = col;
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
}
