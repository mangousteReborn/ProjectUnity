using UnityEngine;
using System.Collections;
/*
 * @author : Thomas P
 * @created_at : 14 / 11 / 2014
 */
public interface EffectInterface  {

	string name;

	void applyEffect(CharacterStats stats);

	void removeEffect(CharacterStats stats);

	void updateEffect();
}
