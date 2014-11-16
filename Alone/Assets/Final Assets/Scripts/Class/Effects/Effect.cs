using UnityEngine;
using System.Collections;
/*
 * @author : Thomas P
 * @created_at : 15/11/2014
 */
public abstract class Effect  {

	private string _key;
	private string _name;
	private string _desc;
	
	/* Duration Type memo :
	 * 0 : permanant (default)
	 * 1 : overtime 
	 * 	- Use for in-combat effects, like poison and general DPS damages. Duration should be treated as ms.
	 * 2 : overfight
	 * 	- Use for whole fight effects, like bonuses. uration should be treated as "turn" (a whole combat, until next room).
	 */
	private uint _durationType;

	private float _duration;

	public Effect(string key, string name="NONE", string desc="This is an effect. And it rulez",uint durationType = 0, float duration = 0f){
		this._durationType = durationType;
		this._duration = duration;
		this._key = key;
		this._name = name;
		this._desc = desc;
	}

	// Get / Set
	public string key
	{
		get {
			return _key;
		}
	}
	public string name
	{
		get {
			return _name;
		}
	}
	public string desc
	{
		get {
			return _desc;
		}
	}

	public uint durationType
	{
		get {
			return _durationType;
		}
	}

	public float duration
	{
		get {
			return _duration;
		}
		set{
			this._duration = duration;
		}
	}

	// Required methods
	// Call once, when pushed in charcacter effect list
	public abstract void applyEffect(CharacterStats stats);
	// Call when effect is over or basicly removed
	public abstract void removeEffect(CharacterStats stats);
	// Call on each character effects update (useleff for 0 types effects (constant))
	public abstract void updateEffect(CharacterStats stats);

}
