using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Fight : MonoBehaviour {

	private int _index;

	private List<Round> _rounds;

	public Fight(int index){
		this._index = index;
	}

	public void addRound(Round r){
		this._rounds.Add(r);
	}

	// GET / SET


}
