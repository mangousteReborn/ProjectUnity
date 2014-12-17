using UnityEngine;
using System.Collections;

public interface IActionHelper {
	Vector3 getEndPoint();
	Vector3 getStartPoint();
	Vector3 getMiddlePoint();


	void activate (CharacterManager cm, Action a);
	bool validate(object[] param);
	void delete();
}