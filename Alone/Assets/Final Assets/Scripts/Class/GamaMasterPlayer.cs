using UnityEngine;
using System.Collections;

public class GamaMasterPlayer : Player {



	public GamaMasterPlayer(NetworkView id, IPlayerGUI gui)
		: base("GameMaster", id, gui)
	{
		this._isGM = true;
	}

	public GamaMasterPlayer(NetworkView id)
		: base("GameMaster", id)
	{
		this._isGM = true;
	}
}
