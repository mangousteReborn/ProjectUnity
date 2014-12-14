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
	private bool _activated = false;
	private bool _RPCcallback = true;

	private float _currentCost;

	private MoveAction _moveAction;
	private CharacterManager _owner;

    public CharacterManager manager{
        get { return _owner; }
    }

	void Start(){

	}

    [RPC]
    private void setValidate(bool isValidate)
    {
        this._validated = isValidate;
    }

	void Update(){
		if (!this._activated)
			return;

		if (this._validated)
			return;

		if (Input.GetMouseButton (0) && !EventSystem.current.IsPointerOverGameObject()) {
			Vector3 target = getTarget();
			calcCost(target);

			if(this._RPCcallback){
				Debug.Log("cliclick and run CallBack");
                this._owner.networkView.RPC("validateMoveActionRPC", RPCMode.All, this._owner.player.id, this._endPoint);
				this._RPCcallback = false;
			}
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

	public void activate (CharacterManager cm, Action a){
		MoveAction ma = (MoveAction)a;

		this._moveAction = ma;
		this._owner = cm;
        Debug.Log("activate RPCALL");
        cm.networkView.RPC("pushMoveActionAsPendingRPC", RPCMode.All, ma.key, ma.name, ma.desc, ma.costPerUnit);

		this._activated = true;
	}

	public void cancel(CharacterManager cm){
        Debug.Log("cancel RPCALL");
        cm.networkView.RPC("removePendingActionRPC", RPCMode.All);
		GameObject.Destroy (this._object);
	}

	public void validate(CharacterManager cm){
        Debug.Log("validate RPCALL");
        GameData.getActionHelperDrawer().networkView.RPC("pushDefaultStaticHelperRPC", RPCMode.All, this._owner.player.id, this._startPoint, this._middlePoint, this._endPoint, this._text.text);
		this._validated = true;
	}

	public void setStartPosition(Vector3 startPos){
		this._startPoint = startPos;
		this._object.transform.position = startPos;

	}





	public void delete(){
		GameObject.Destroy (this._object);
	}

	public bool validate(object[] param){

		//this._endPoint = (Vector3)param [0];
		return true;

	}


	private void calcCost(Vector3 destPos){
		
		this._endPoint = destPos;
		this._lineRenderer.SetVertexCount(2);
		this._lineRenderer.SetPosition(0, this._startPoint);
		this._lineRenderer.SetPosition(1, this._endPoint);

		this._currentCost = this._moveAction.calculateCost (this._startPoint, this._endPoint);
		
		this._middlePoint = ((this._endPoint - this._startPoint ) / 2) + this._startPoint;
		
		this._textObject.transform.position = this._middlePoint;
		this._text.text = this._moveAction.name + "\n" + this._currentCost + "s";
		if (this._currentCost > this._owner.characterStats.currentActionPoint) {
			this._text.color = Color.red;
		} else 
			this._text.color = Color.white;
		
		
		return;
		// FIXME
		/*NavMeshPath path = new NavMeshPath();
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
		}*/
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

	public Vector3 getEndPoint() {
		return this._endPoint;
	}
	public Vector3 getStartPoint() {
		return this._startPoint;
	}
	public Vector3 getMiddlePoint() {
		return this._middlePoint;
	}

	public bool RPCcallback{
		set{this._RPCcallback = value;}
	}


}
