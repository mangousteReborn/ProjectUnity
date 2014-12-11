using UnityEngine;
using System.Collections;

public class LineHelper : MonoBehaviour, IActionHelperComponent{

	private GameObject _object;

	private Vector3 _startPoint;
	private Vector3 _endPoint;
	private Vector3 _middlePoint;


	private float _size;

	private bool _initialized = false;

	public void instantiate(Vector3 startPos, Vector3 endPos, float size){
		this._startPoint = startPos;
		this._endPoint = endPos;

		//	 /!!\
		//  /!!!!\
		// /!!!!!!\ Maybe wrong ....
		this._middlePoint = ((startPos - endPos) / 2) + startPos;

		this._object = (GameObject)Instantiate (_object, _middlePoint, Quaternion.identity);
		//this._object.GetComponent<LineRenderer>().
	}
	
	// Update is called once per frame
	void Update () {
		if (!_initialized)
			return;
	}		

}
