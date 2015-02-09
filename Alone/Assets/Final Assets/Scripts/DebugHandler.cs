using UnityEngine;
using System.Collections;

public class DebugHandler : MonoBehaviour {

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.G)) {
			GameData.printGameDatas();
		}
	}
}
