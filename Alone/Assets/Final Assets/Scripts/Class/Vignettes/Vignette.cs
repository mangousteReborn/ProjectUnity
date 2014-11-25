using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Vignette {

	VignetteType _type;
	string _key;
	string _name;
	string _desc;
	string _imagePath;


	public Vignette(string key, string name, string desc, VignetteType type, string imagePath="imports/images/notfound.png"){
		this._type = type;
		this._key = key;
		this._name = name;
		this._desc = desc != null ? desc : name;
		this._imagePath = imagePath;

	}






	public string key{
		get{
			return this._key;
		}
		set {
			this._key = value;
		}
	}
	public string name{
		get{
			return this._name;
		}
		set {
			this._name = value;
		}
	}
	public string desc{
		get{
			return this._desc;
		}
		set {
			this._desc = value;
		}
	}
	public VignetteType type{
		get{
			return this._type;
		}
		set {
			this._type = value;
		}
	}
	public string imagePath{
		get{
			return this._imagePath;
		}
		set {
			this._imagePath = value;
		}
	}


}
