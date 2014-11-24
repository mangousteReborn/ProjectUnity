using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
/*
 * @author : Thomas P
 * @created_at : 15/11/2014
 */
public class CharacterStats  {

	List <Func<CharacterStats>> _listenersList; // !!! ???

	List <Effect> _effectsList;

	private int _maxLife;
	private int _currentLife;

	public CharacterStats (int startLife = 100){
		this._effectsList = new List<Effect> ();

		this._maxLife = startLife;
		this._currentLife = startLife;

	}

	/*
	 * Effects managing
	 */
	public void pushEffect(Effect e){
		this._effectsList.Add (e);
		e.applyEffect (this);
	}

	// Set "all" to true to remove all effect with given key.
	public void removeEffect(string key, bool all = false){
		int i = -1;
		foreach (Effect e in this._effectsList){
			if(e.key.CompareTo(key) == 0){
				break;
			}
			i++;
		}
		if (-1 != i)
			this._effectsList.RemoveAt(i);

		if (all && -1 != i)
			removeEffect (key, all);
	}

	public void updateEffect(){
		foreach (Effect e in this._effectsList){
			if(e.durationType != 0){
				e.updateEffect(this);
			}
		}
	}
	

	// Get/Seters
	public int maxLife
	{
		get {
			return _maxLife;
		}
		set {
			_maxLife = value;

		}
	}


}
