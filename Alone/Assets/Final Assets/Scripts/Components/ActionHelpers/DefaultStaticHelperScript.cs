using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class DefaultStaticHelperScript : MonoBehaviour {
	[SerializeField]
	private GameObject _object;

	[SerializeField]
	private GameObject _textObject;

	[SerializeField]
	private Text _text;

	// Use this for initialization
	void Start () {
	
	}


	public GameObject textObject {
		get {
			return _object;
		}
	}

	public Text text {
		get {
			return _text;
		}
	}
}
