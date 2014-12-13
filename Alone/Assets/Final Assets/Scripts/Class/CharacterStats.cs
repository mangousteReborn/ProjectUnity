﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
/*
 * @author : Thomas P
 * @created_at : 15/11/2014
 */
public class CharacterStats  {

	List <Action<CharacterStats,object[]>> _listenersList; // !!! ???
	
	Dictionary <CharacterStatsEvent, Action<CharacterStats,object[]>> _eventListernersMap;

	List <VignetteBonus> _vignettesList;
	List <Effect> _effectsList;

	List<Action> _availableActionList;
	Stack<Action> _hotActionsStack;

	// Statistics
	private int _maxLife;
	private int _currentLife;

	private float _maxActionPoint;
	private float _currentActionPoint;

    private NetworkView _networkView;

	/*
	 *  0 : Default
	 *  1 : restMode
	 *  2 : fightMode
	 */
	private uint _gameMode = 0;

	public CharacterStats (NetworkView networkView=null, int startLife = 100, float actionPoint=5f){
		this._effectsList = new List<Effect> ();
		this._listenersList = new List <Action<CharacterStats,object[]>> ();
		this._eventListernersMap = new Dictionary <CharacterStatsEvent, Action<CharacterStats,object[]>> ();

		this._vignettesList = new List<VignetteBonus> ();

		this._availableActionList = new List<Action> ();

		this._hotActionsStack = new Stack<Action> ();

		this._maxLife = startLife;
		this._currentLife = startLife;
		this._maxActionPoint = actionPoint;
		this._currentActionPoint = actionPoint;

		// Definition of default available Actions
		this._availableActionList.Add (GameData.getAction("move"));

        this._networkView = networkView;

	}

	public void hasChanged(){
		foreach (Action<CharacterStats,object[]> f in this._listenersList) {
			f(this,null);
		}
	}

	public void register(CharacterStatsEvent key, Action<CharacterStats,object[]> a){
		this._eventListernersMap.Add (key, a);
	}

	public void fireEvent(CharacterStatsEvent key, object[] param=null){
		foreach (KeyValuePair<CharacterStatsEvent,Action<CharacterStats,object[]>> kvp in this._eventListernersMap) {
			if(kvp.Key == key){
				kvp.Value(this,param);
			}
		}
	}

	/*
	 * Effects managing
 	 */
	// Vignettes
	public void pushVignette(VignetteBonus v){
		this._vignettesList.Add (v);
		foreach (Effect e in v.effectsList) {
			e.applyEffect(this);
		}

	}

	public void removeVignette(string key, bool all = false, bool silent = false){
		int i = 0;
		bool found = false;
		foreach (VignetteBonus v in this._vignettesList){
			if(v.key.CompareTo(key) == 0){
				found = true;
				if (!silent){
					foreach(Effect e in v.effectsList)
						e.removeEffect(this);
				}
				break;
			}
			i++;
		}
		if (found) {
			this._vignettesList.RemoveAt(i);
		}
		
		if (all && found)
			removeVignette (key, all, silent);
	}

	public bool hasVignette(string key){
		foreach (VignetteBonus vb in this._vignettesList) {
			if(vb.key.CompareTo(key) == 0)
				return true;
		}
		return false;
	}

	// Effects
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

	public bool hasEffect(string key){
		foreach (Effect e in this._effectsList) {
			if(e.key.CompareTo(key) == 0)
				return true;
		}
		return false;
	}



	/*
	 * All effects (Vignettes included) updates
	 */
	public void updateEffects(){
		foreach (Effect e in this._effectsList){
			if(e.durationType != 0){
				e.updateEffect(this);
			}
		}
		fireEvent(CharacterStatsEvent.change,null);
	}

	public void updateEffectsOfType(int durationType){
		foreach (Effect e in this._effectsList){
			if(e.durationType == durationType && durationType != 0){
				e.updateEffect(this);
			}
		}
		fireEvent(CharacterStatsEvent.change,null);
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
		fireEvent(CharacterStatsEvent.change,null);
	}

	/*
	 * Action
	 */
	public void addActionInAvailableList(Action a){
		this._availableActionList.Add (a);
		object[]param={a};
		fireEvent(CharacterStatsEvent.actionAdded,param);
	}
	public void pushHotAction(Action a){
		this._hotActionsStack.Push (a);

		object[]param={a};
		fireEvent(CharacterStatsEvent.hotActionPushed,param);
	}
	// FIXME : CharacterStats maybe doesnt have to cancelAction. GUI will do it by listening lastHotActionRemove event
	public void removeLastHotAction(){
		Action oa = this._hotActionsStack.Pop ();
		object[] subParam = {this};
		oa.cancelAction (subParam);

		object[]param={oa};
		fireEvent (CharacterStatsEvent.lastHotActionRemoved, param);
	}



	// Get/Seters
	/*
	 *  Statictics TODO : Manage Events and Fireing conditions
	 */
	public int maxLife
	{
		get {
			return this._maxLife;
		}
		set {
			this._maxLife = value;

			fireEvent(CharacterStatsEvent.change,null);
		}
	}
	public int currentLife
	{
		get {
			return this._currentLife;
		}
	}

    public void setCurrentLife(int value, bool isFromRPC)
    {
        if (Network.isServer)
        {
            int oldLife = this._currentLife;
            this._currentLife = value;

            object[] param = { oldLife, this._currentLife };

            fireEvent(CharacterStatsEvent.currentLifeChange, param);
            fireEvent(CharacterStatsEvent.change, null);
            _networkView.RPC("setLife", RPCMode.Others, value);
        }
        else
        {
            int oldLife = this._currentLife;
            this._currentLife = value;

            object[] param = { oldLife, this._currentLife };

            fireEvent(CharacterStatsEvent.currentLifeChange, param);
            fireEvent(CharacterStatsEvent.change, null);
            if(!isFromRPC)
                _networkView.RPC("setLife", RPCMode.Server, value);
        }
    }

	public float maxActionPoint
	{
		get {
			return this._maxActionPoint;
		}
		set {
			this._maxActionPoint = value;
			
			fireEvent(CharacterStatsEvent.change,null);
		}
	}
	public float currentActionPoint
	{
		get {
			return this._currentActionPoint;
		}
		set {
			float old = this._currentActionPoint;
			this._currentActionPoint = value;

			object[] param = {old,this._currentActionPoint};
			fireEvent(CharacterStatsEvent.currentActionPointChanged,param);
		}
	}

	// Others
	public uint gameMode
	{
		get {
			return this._gameMode;
		}
		set {
			this._gameMode = value;
			object[] p = {value};
			fireEvent(CharacterStatsEvent.gameModeChanged,p);
		}
	}

	public List<Action> availableActionList
	{
		get {
			return this._availableActionList;
		}
	}

	public Stack<Action> hotActionsStack
	{
		get {
			return this._hotActionsStack;
		}
	}

	public List<Action<CharacterStats,object[]>> listenersList
	{
		get {
			return this._listenersList;
		}
	}
	public NetworkView networkView
	{
		set {this._networkView = value;}
		get {
			return this._networkView;
		}
	}


}
