using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
/*
 * @author : Thomas P
 * @created_at : 15/11/2014
 */
public class CharacterStats  {

	List <Action<CharacterStats,object[]>> _listenersList; // !!! ???

	/*
	 * 
	 */
	Dictionary <CharacterStatsEvent, Action<CharacterStats,object[]>> _eventListernersMap;

	List <Vignette> _vignettesList;
	List <Effect> _effectsList;

	private int _maxLife;
	private int _currentLife;

	public CharacterStats (int startLife = 100){
		this._effectsList = new List<Effect> ();
		this._listenersList = new List <Action<CharacterStats,object[]>> ();
		this._eventListernersMap = new Dictionary <CharacterStatsEvent, Action<CharacterStats,object[]>> ();

		this._maxLife = startLife;
		this._currentLife = startLife;

	}


	public void hasChanged(){
		foreach (Action<CharacterStats,object[]> f in this._listenersList) {
			f(this,null);
		}
	}

	public void register(CharacterStatsEvent key, Action<CharacterStats,object[]> a){
		this._eventListernersMap.Add (key, a);
	}

	private void fireEvent(CharacterStatsEvent key, object[] param=null){
		foreach (KeyValuePair<CharacterStatsEvent,Action<CharacterStats,object[]>> kvp in this._eventListernersMap) {
			if(kvp.Key == key){
				kvp.Value(this,param);
			}
		}
	}

	/*
	 * Effects managing
	 */
	public void pushVignette(Vignette v){

	}

	public void removeVignette(string k){

	}

	public void pushEffect(Effect e){
		this._effectsList.Add (e);
		e.applyEffect (this);
	}

	// Set "all" to true to remove all effect with given key.
	// Set "silent" to true to ignore "removeEffect" calling.
	public void removeEffect(string key, bool all = false, bool silent = false){
		int i = 0;
		bool found = false;
		foreach (Effect e in this._effectsList){
			if(e.key.CompareTo(key) == 0){
				found = true;
				if (!silent){
					e.removeEffect(this);
				}
				break;
			}
			i++;
		}
		if (found) {
			this._effectsList.RemoveAt(i);
		}

		if (all && found)
			removeEffect (key, all, silent);
	}

	public void updateEffects(){
		foreach (Effect e in this._effectsList){
			if(e.durationType != 0){
				e.updateEffect(this);
			}
		}
		hasChanged();
	}

	public void updateEffectsOfType(int durationType){
		foreach (Effect e in this._effectsList){
			if(e.durationType == durationType && durationType != 0){
				e.updateEffect(this);
			}
		}
		hasChanged();
	}

	public void updateCombatEffects(){
		List <Effect> toRemove = new List<Effect>();

		foreach (Effect e in this._effectsList){
			if(e.durationType == 1){
				if (e.currentDuration <= 0){
					e.removeEffect(this);
					toRemove.Add(e);
					continue;
				}
				e.currentDuration -= 1;
				e.updateEffect(this);
			}

		}
		foreach (Effect e in toRemove) {
			this._effectsList.Remove(e);
		}
		hasChanged();
	}

	// Get/Seters
	public int maxLife
	{
		get {
			return this._maxLife;
		}
		set {
			this._maxLife = value;

			hasChanged();
		}
	}
	public int currentLife
	{
		get {
			return this._currentLife;
		}
		set {
			int oldLife = this._currentLife;
			this._currentLife = value;

			object[] param = {oldLife,this._currentLife};

			fireEvent(CharacterStatsEvent.currentLifeChange, param);
			hasChanged();
		}
	}

	public List<Action<CharacterStats,object[]>> listenersList
	{
		get {
			return this._listenersList;
		}
	}


}
