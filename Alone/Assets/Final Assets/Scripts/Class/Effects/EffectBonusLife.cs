using UnityEngine;
using System.Collections;
/*
 * @author: Thomas p
 * @created_at : 14/11/2014
 */
public class EffectBonusLife : Effect {

	private int _lifeBonus;
	//private static int lifeBonus = 100;
	public EffectBonusLife(int lifeBonus, string key="lifebonus") 
		: base(key, lifeBonus + " life", "Bonus permanent de "+lifeBonus+" à la vie.")
	{
		this._lifeBonus = lifeBonus;
	}

	public override void applyEffect(CharacterStats stats){
		stats.maxLife += this._lifeBonus;
	}

	public override void removeEffect(CharacterStats stats){
		stats.maxLife = stats.maxLife - this._lifeBonus < 1 ? 1 : stats.maxLife - this._lifeBonus;
	}

	public override void updateEffect(CharacterStats stats){
		return;
	}

}
