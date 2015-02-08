using UnityEngine;
using System.Collections;

public class PlayerInfos {

	private Player _owner;

	private int _expectedAction;

	public PlayerInfos (Player owner, int expectedActions){
		this._owner = owner;
		this._expectedAction = expectedActions;
	}
}
