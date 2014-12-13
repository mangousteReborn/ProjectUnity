using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VignetteAction : Vignette {

	private Action _action;

	public VignetteAction(Action a, string imagePath="Vignettes/default_action")
	: base(a.key, a.name, a.desc == null ? a.name : a.desc, VignetteType.action, imagePath){
		this._action = a;
	}

	public Action action{
		get{
			return this._action;
		}
	}
}
