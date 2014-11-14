using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterStats  {

	List <EffectInterface> effectsList;

	int maxLife;
	int currentLife;

	CharacterStats (){
		this.effectsList = new List<EffectInterface> ();

		this.maxLife = 100;
		this.currentLife = 100;

	}


}
