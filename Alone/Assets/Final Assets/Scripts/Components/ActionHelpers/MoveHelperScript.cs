using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using System;
using System.Collections;


public class MoveHelperScript : MonoBehaviour, IActionHelper{

	[SerializeField]
	private GameObject _object;

	[SerializeField]
	private GameObject _textObject;

	[SerializeField]
	private LineRenderer _lineRenderer;

	[SerializeField]
	private Text _text;

	private Vector3 _endPoint; // Implements
	private Vector3 _startPoint; // Implements
	private Vector3 _middlePoint; // Implements


	private float _size;

	private bool _validated = false;

	private float _currentCost;
	private MoveAction _moveAction;
	private CharacterManager _owner;

	void Start(){

	}

	void Update(){
		if (this._validated)
			return;

		if (Input.GetMouseButton (0) && !EventSystem.current.IsPointerOverGameObject()) {
			Vector3 target = getTarget();
			calcCost(target);
			object[] p = {this._currentCost, target};
			this._validated = this._moveAction.onActionValidation(this._owner, p);

			return;
		}

		calcCost(getTarget());


	}

	// IMPLEMENTS ME
	public void setAction(Action a){
		this._moveAction = (MoveAction)a;

	}

	// IMPLEMENTS ME
	public void setOwner(CharacterManager cm){
		this._owner = cm;
	}

	public void setStartPosition(Vector3 startPos){
		this._startPoint = startPos;
		this._object.transform.position = startPos;

	}

	private Vector3 getTarget()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		Vector3 target = this._startPoint;
		if (Physics.Raycast(ray, out hit))
		{
			target = hit.point;
			target.y = 0;
		}
		return target;
	}

	private void calcCost(Vector3 destPos){

		this._endPoint = destPos;
		this._lineRenderer.SetVertexCount(2);
		this._lineRenderer.SetPosition(0, this._startPoint);
		this._lineRenderer.SetPosition(1, this._endPoint);
		
		this._currentCost = (float)Math.Round(
			(Vector3.Distance(this._startPoint, this._endPoint) * this._moveAction.costPerUnit),
			2,
			MidpointRounding.ToEven
			);
		
		this._middlePoint = ((this._endPoint - this._startPoint ) / 2) + this._startPoint;
		
		this._textObject.transform.position = this._middlePoint;
		this._text.text = this._moveAction.name + "\n" + this._currentCost + "s";
		if (this._currentCost > this._owner.characterStats.currentActionPoint) {
			this._text.color = Color.red;
		} else 
			this._text.color = Color.white;


		return;
		// FIXME
		NavMeshPath path = new NavMeshPath();
		NavMesh.CalculatePath(this._startPoint, this._endPoint, -1, path);
		Debug.Log ("Path Len == " + path.corners.Length + " path corn " + path.corners);
		if (path.corners.Length < 0)
			return;

		Vector3[] pathPosList = path.corners;
		this._lineRenderer.SetVertexCount(pathPosList.Length);
		this._lineRenderer.SetPosition(0, pathPosList[0]);
		float distance = Vector3.Distance(pathPosList[0], this._startPoint);
		for (int i = 1; i < pathPosList.Length; i++)
		{
			this._lineRenderer.SetPosition(i, pathPosList[i]);
			distance += Vector3.Distance(pathPosList[i], pathPosList[i - 1]);
		}
	}

	public void delete(){
		GameObject.Destroy (this._object);
	}

	public bool validate(object[] param){

		//this._endPoint = (Vector3)param [0];
		return true;

	}

	public Vector3 getEndPoint() {
		return this._endPoint;
	}
	public Vector3 getStartPoint() {
		return this._startPoint;
	}
	public Vector3 getMiddlePoint() {
		return this._middlePoint;
	}



}
