using UnityEngine;
using System.Collections;
/*
 * @author: Thomas p
 * @created_at : 16/11/2014
 */
public class EffectDamageOverBattle : Effect {
	
	private int _damage;
	
	public EffectDamageOverBattle(int damage, int duration, string key="dob") 
		: base(key, "Dégats par combat", "Inflige " + damage + " à chaque début de combat, sur " + duration + "combat" + (duration>1?"s":""), 1 , duration)
	{
		this._damage = damage;
	}
	
	public override void applyEffect(CharacterStats stats){
		return;
	}
	
	public override void removeEffect(CharacterStats stats){
		return;
	}
	
	public override void updateEffect(CharacterStats stats){
		stats.currentLife -= this._damage;
	}
	
}
