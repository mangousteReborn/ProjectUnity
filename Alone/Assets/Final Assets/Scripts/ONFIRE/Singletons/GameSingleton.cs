using UnityEngine;
using System.Collections;

public class GameSingleton
{
	private static GameSingleton me;

	private GameSingleton() {
		Debug.Log ("Game started");
		
	}

	public static GameSingleton instance()
	{
		if (me == null)
		{
			me = new GameSingleton();
		}

		return me;
	}


}