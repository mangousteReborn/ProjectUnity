using UnityEngine;
using System.Collections;

public interface IActionHelper {
	Vector3 getEndPoint();
	Vector3 getStartPoint();
	Vector3 getMiddlePoint();

	bool validate(object[] param);
	void delete();
}
