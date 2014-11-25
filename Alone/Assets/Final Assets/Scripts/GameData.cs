using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class GameData {

	//private static Dictionary<string, Spell> _spellMap;

	private static Dictionary<string, Vignette> _bonusVignetteMap = new Dictionary<string, Vignette>();




	private static bool initialized = false;

	public static string init(){
		string err = null;
		if(initialized)
			return "GameDatas have been already initialized";
		initialized = true;


		/*
			VIGNETTES
		 */
		// Life Bonus
		List<Effect> lifeupEffects = new List<Effect>();
		lifeupEffects.Add(new EffectBonusLife(100));
		_bonusVignetteMap.Add("lifebonus", new VignetteBonus(lifeupEffects, "lifebonus", "Super Vie"));

		/*
		 * SPELLS 
		 */



		return err;
	}


	public static Vignette getBonusVignette(string k){
		Vignette val;
		_bonusVignetteMap.TryGetValue(k, out val);
		return val;
	}

	public static Dictionary<string, Vignette> getBonusVignettes(){
		return _bonusVignetteMap;
	}


}
