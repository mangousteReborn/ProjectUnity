using UnityEngine;
using System.Collections;

public class EffectBonusLife : Effect {

	private int _lifeBonus;
	//private static int lifeBonus = 100;
	public EffectBonusLife(int lifeBonus) : base("lowlifebonus", lifeBonus + " life", "Bonus permanent de "+lifeBonus+" à la vie.")
	{
		this._lifeBonus = lifeBonus;
	}

	public override void applyEffect(CharacterStats stats){
		Debug.Log ("Super ! dla vie en plus !");
		stats.maxLife += this._lifeBonus;
	}

	public override void removeEffect(CharacterStats stats){
		Debug.Log ("Allez, casse toi l'effet !");
		stats.maxLife = stats.maxLife - this._lifeBonus < 1 ? 1 : stats.maxLife - this._lifeBonus;
	}

	public override void updateEffect(CharacterStats stats){
		return;
	}

}
