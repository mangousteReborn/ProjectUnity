using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VignetteBonus : Vignette {

	List<Effect> _effectsList;

	public VignetteBonus(List<Effect> l,string key, string name, string desc=null, string imagePath="Vignettes/default_bonus")
	: base(key, name, desc == null ? name : desc, VignetteType.bonus, imagePath){
		copyList(l);
	}

	private void copyList(List<Effect> l){
		
		if (this._effectsList == null){
			this._effectsList = new List<Effect>();
		} else {
			this._effectsList.Clear();
		}
		
		foreach(Effect e in l){
			this._effectsList.Add(e);
		}
	}




	public List<Effect> effectsList{
		get{
			return this._effectsList;
		}
	}
}
