using UnityEngine;
using System.Collections;
/*
 * @author : Thomas P
 * @created_at : 15/11/2014
 * 
 * How to use/create effects :
 * - Extends Effect class, override specified methods.
 * - When calling mother-class construtor (base keyword) specify the duration type of the effect (see "Duration Type memo")
 * - /!\ Note for "key" attribute :
 * 		-> "key" attribute will be used to reconize an Effect. By default, the key will be the same for each instance
 * 		of a given implementation class. When "removeEffect" method will be called by "characterStats", all effects with
 * 		passed key will be removed.
 * 		-> BUT ! If two different effects use the same implementation class, when removeEffect will be called, the both
 * 		will be removed. So, you have to set a different key for each.
 * 		Exemple :
 * 		I want to create two different effect based on "EffectDamageOverBattle" : 
 * 		a "Poison" effect, who deals 10 damages each turn for 5 turn.
 * 		and a "Fire" effect, who deals 50 damges each turn for 2 trun.
 * 		By default,the both will have the predefined key "dob".
 * 		Basicly, if i just want to remove "poison" effect, i have to override the default "dob" key, and when necessary,
 * 		tell to "characterStats" to remove my overrided key. That's all forks !
 * 		
 * 
 */
public abstract class Effect  {

	private string _key;
	private string _name;
	private string _desc;
	
	/* Duration Type memo :
	 * 0 : permanant (default)
	 * 1 : At beginin of each combat 
	 * 	- Use for in-combat effects, like poison and general DPS damages. Duration should be treated as number of combat.
	 *  - Update Effect will be call on each combat step begining.
	 * 	- Will be removed on combat beginin if duration is 0.
	 * 2 : Over rooms
	 * 	- Use for whole fight effects, like bonuses. duration should be treated as "turn" (a whole combat, until next room).
	 *  - Update Effect will be call on each combat begining.
	 * 	- Will be removed on entering new room if duration is 0.
	 * 3 : Over time TODO
	 * 	- 
	 * 	
	 */
	private uint _durationType;

	private float _duration;
	private float _currentDuration;

	public Effect(string key, string name="NONE", string desc="This is an effect. And it rulez",uint durationType = 0, float duration = 0f){
		this._durationType = durationType;
		this._duration = duration;
		this._currentDuration = duration;
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
			this._duration = value;
		}
	}

	public float currentDuration
	{
		get {
			return _currentDuration;
		}
		set{
			this._currentDuration = value;
		}
	}

	// Required methods
	// Call once, when pushed in charcacter effect list
	public abstract void applyEffect(CharacterStats stats);
	// Call when effect is over or basicly removed
	public abstract void removeEffect(CharacterStats stats);
	// Call for non-zero duration typed effect, at specific moments depending of duration type (see doc above).
	public abstract void updateEffect(CharacterStats stats);

}
