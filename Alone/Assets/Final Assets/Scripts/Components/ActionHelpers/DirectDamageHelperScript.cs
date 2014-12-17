using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using System;
using System.Collections;
using System.Collections.Generic;


public class DirectDamageHelperScript : MonoBehaviour, IActionHelper{
	
	[SerializeField]
	private GameObject _object;
	
	[SerializeField]
	private GameObject _textObject;
	
	[SerializeField]
	private LineRenderer _lineRenderer;
	
	[SerializeField]
	private GameObject _meshWrapperObject;
	
	private MeshFilter _meshFilter;

	[SerializeField]
	private Text _text;

	public Material material;

	private Vector3 _endPoint; // Implements
	private Vector3 _startPoint; // Implements
	private Vector3 _middlePoint; // Implements
	

	
	
	private bool _validated = false;
	private bool _activated = false;
	private bool _RPCcallback = true;

	// Params
	private bool _drawLine = false;
	private Func<DirectDamageHelperScript, float> _calculateDistanceFuncion = null;
	// Subparams
	private float _angle_fov = 30;
	
	private float _dist_min = 0.1f;
	private float _dist_max = 1.0f;


	private float _currentCost;
	private float _size;
	
	private DirectDamageAction _action;
	private CharacterManager _owner;
	private Mesh _mesh;
	private float _setAngle;
	//Constants
	private float _costPerUnit = 0.15f;
	private int _quality = 15;


	void Start(){

	}
	
	void Update(){
		if (!this._activated)
			return;
		
		if (this._validated)
			return;
		
		if (Input.GetMouseButton (0) && !EventSystem.current.IsPointerOverGameObject()) {
			Vector3 target = getTarget();
			calcDistance(target);
			
			if(this._RPCcallback){
				this._owner.networkView.RPC("validateDirectDamageActionRPC", RPCMode.All,this._owner.player.id, target, this._setAngle);
				this._endPoint = target;
				this._RPCcallback = false;
			}
			return;
		}
		
		calcDistance(getTarget());
	}

	public void activateAsStatic(CharacterManager cm, Vector3 startPoint, Vector3 endPoint, string label , float degree, float radius, float angle){
		if (this._activated)
			return;

		this._dist_max = radius;
		this._angle_fov = degree;
		this._startPoint = startPoint;

		this._meshFilter = this._meshWrapperObject.GetComponent<MeshFilter>();
		this._meshFilter.transform.position =  new Vector3(this._startPoint.x,0.5f,this._startPoint.z);
		this._meshFilter.mesh = buildMesh(angle);
		this._meshWrapperObject.GetComponent<MeshRenderer> ().material.color = new Color(cm.material.color.r,
		                                                                                 cm.material.color.g,
		                                                                                 cm.material.color.b,
		                                                                                 0.2f);

		this._text.text = label;
		this._textObject.transform.position = ((endPoint - startPoint) / 2) + startPoint;
	}

	public void activate (CharacterManager cm, Action a){
		
		this._action = (DirectDamageAction)a;
		this._owner = cm;

		this._dist_max = this._action.radius;
		this._angle_fov = this._action.degree;

		this._meshFilter = this._meshWrapperObject.GetComponent<MeshFilter>();
		this._meshFilter.transform.position =  new Vector3(this._startPoint.x,0.5f,this._startPoint.z);
		this._meshFilter.mesh = this._action.buildMesh(this._quality, this._action.degree, 0.1f, this._action.radius, 0f);
		this._meshWrapperObject.GetComponent<MeshRenderer> ().material.color = new Color(cm.material.color.r,
		                                                                                 cm.material.color.g,
		                                                                                 cm.material.color.b,
		                                                                                 0.2f);

		cm.networkView.RPC("setPendingActionByKeyRPC", RPCMode.All, a.key);

		this._activated = true;
	}
	
	public void cancel(CharacterManager cm){
		cm.networkView.RPC ("removePendingActionRPC", RPCMode.All);
		GameObject.Destroy (this._object);
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
	
	
	private void calcDistance(Vector3 destPos){
		
		this._endPoint = destPos;
		this._setAngle = getMagicalAngle(destPos);
		this._meshFilter.transform.eulerAngles = new Vector3(0, this._setAngle, 0);

		this._currentCost = this._action.calculateCost (this._startPoint, this._endPoint);

		/*
		if(null != this._calculateDistanceFuncion){
			this._currentCost = this._calculateDistanceFuncion(this);
		} else {
			this._currentCost = defaultCalculateDistance(this._startPoint, this._endPoint);
		}
		*/

		this._middlePoint = ((this._endPoint - this._startPoint ) / 2) + this._startPoint;
		
		this._textObject.transform.position = this._middlePoint;
		this._text.text = this._action.name + "\n" + this._currentCost + "s";
		if (this._currentCost > this._owner.characterStats.currentActionPoint) {
			this._text.color = Color.red;
		} else 
			this._text.color = Color.white;
		
	}
	
	private Mesh buildMesh(float angle=0f){
		// Global def
		Mesh mesh = new Mesh();

		mesh.name = "cone_mesh";
		mesh.vertices = new Vector3[4 * _quality];   // Could be of size [2 * quality + 2] if circle segment is continuous
		mesh.triangles = new int[3 * 2 * _quality];
		
		Vector3[] normals = new Vector3[4 * _quality];
		Vector2[] uv = new Vector2[4 * _quality];
		
		for (int i = 0; i < uv.Length; i++)
			uv[i] = new Vector2(0, 0);
		for (int i = 0; i < normals.Length; i++)
			normals[i] = new Vector3(0, 1, 0);
		
		mesh.uv = uv;
		mesh.normals = normals;

		// Vertice fill
		float angle_lookat = angle;
		
		float angle_start = angle_lookat - _angle_fov;
		float angle_end = angle_lookat + _angle_fov;
		float angle_delta = (angle_end - angle_start) / _quality;
		
		float angle_curr = angle_start;
		float angle_next = angle_start + angle_delta;
		
		Vector3 pos_curr_min = Vector3.up;//zero
		Vector3 pos_curr_max = Vector3.up;
		
		Vector3 pos_next_min = Vector3.up;
		Vector3 pos_next_max = Vector3.up;
		
		Vector3[] vertices = new Vector3[4 * _quality];   // Could be of size [2 * quality + 2] if circle segment is continuous
		int[] triangles = new int[3 * 2 * _quality];
		
		for (int i = 0; i < _quality; i++)
		{
			Vector3 sphere_curr = new Vector3(
				Mathf.Sin(Mathf.Deg2Rad * (angle_curr)), 0,   // Left handed CW
				Mathf.Cos(Mathf.Deg2Rad * (angle_curr)));
			
			Vector3 sphere_next = new Vector3(
				Mathf.Sin(Mathf.Deg2Rad * (angle_next)), 0,
				Mathf.Cos(Mathf.Deg2Rad * (angle_next)));
			
			pos_curr_min = sphere_curr * _dist_min;
			pos_curr_max = sphere_curr * _dist_max;
			
			pos_next_min = sphere_next * _dist_min;
			pos_next_max = sphere_next * _dist_max;
			
			int a = 4 * i;
			int b = 4 * i + 1;
			int c = 4 * i + 2;
			int d = 4 * i + 3;
			
			vertices[a] = pos_curr_min; 
			vertices[b] = pos_curr_max;
			vertices[c] = pos_next_max;
			vertices[d] = pos_next_min;
			
			triangles[6 * i] = a;       // Triangle1: abc
			triangles[6 * i + 1] = b;  
			triangles[6 * i + 2] = c;
			triangles[6 * i + 3] = c;   // Triangle2: cda
			triangles[6 * i + 4] = d;
			triangles[6 * i + 5] = a;
			
			angle_curr += angle_delta;
			angle_next += angle_delta;
			
		}
		
		mesh.vertices = vertices;
		mesh.triangles = triangles;

		return mesh;
	}

	private float getMagicalAngle(Vector3 mousePos)
	{	
		Vector3 refRight = Vector3.Cross(Vector3.up, this._startPoint  + Vector3.forward);
		
		float angle = Vector3.Angle(mousePos -this._startPoint, this._startPoint + Vector3.forward);
		
		return (Mathf.Sign(Vector3.Dot(mousePos -this._startPoint, refRight)) * angle) - 180; // 180 to revert result, beacause formula use inverse ref. to rotate
		
	}
	
	private Vector3 getTarget()
	{
		Vector3 mousepos = Input.mousePosition;
		mousepos.z = Camera.main.transform.position.y;

		Ray ray = Camera.main.ScreenPointToRay(mousepos);
		RaycastHit hit;
		Vector3 target = this._startPoint;
		if (Physics.Raycast(ray, out hit))
		{
			target = hit.point;
			target.y = 0;
		}
		return target;
	}

	/*
	 *  Get / Set
	 */
	private float defaultCalculateDistance(Vector3 start, Vector3 dest){
		return (float)Math.Round(
			(Vector3.Distance(start, dest) * this._costPerUnit),
			2,
			MidpointRounding.ToEven
			);
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

	public Text text {
		get {
			return _text;
		}
	}
	public GameObject textObject {
		get {
			return _textObject;
		}
	}
	public float distanceMax {
		get {return this._dist_max;}
		set {this._dist_max = value;}
	}
	public float angleFOV {
		get {return this._angle_fov;}
		set {this._angle_fov = value;}
	}

	public bool drawLine {
		get {return this._drawLine;}
		set {this._drawLine = value;}
	}
	public CharacterManager manager{
		get { return _owner; }
	}
	public Func<DirectDamageHelperScript, float> calculateDistanceFuncion{
		set{this._calculateDistanceFuncion = value;}
	}
	public bool RPCcallback{
		set{
			Debug.Log("rollracking");
			this._RPCcallback = value;
		}
	}
}


/* OLD CODE

private void calcCone(Vector3 mousePos){
	float angle_lookat = getMagicalAngle(mousePos);
	
	float angle_start = angle_lookat - angle_fov;
	float angle_end = angle_lookat + angle_fov;
	float angle_delta = (angle_end - angle_start) / quality;
	
	float angle_curr = angle_start;
	float angle_next = angle_start + angle_delta;
	
	Vector3 pos_curr_min = Vector3.zero;////zero
	Vector3 pos_curr_max = Vector3.zero;
	
	Vector3 pos_next_min = Vector3.zero;//Vector3.zero;
	Vector3 pos_next_max = Vector3.zero;
	
	Vector3[] vertices = new Vector3[4 * quality];   // Could be of size [2 * quality + 2] if circle segment is continuous
	int[] triangles = new int[3 * 2 * quality];
	
	for (int i = 0; i < quality; i++)
	{
		Vector3 sphere_curr = new Vector3(
			Mathf.Sin(Mathf.Deg2Rad * (angle_curr)), 0,   // Left handed CW
			Mathf.Cos(Mathf.Deg2Rad * (angle_curr)));
		
		Vector3 sphere_next = new Vector3(
			Mathf.Sin(Mathf.Deg2Rad * (angle_next)), 0,
			Mathf.Cos(Mathf.Deg2Rad * (angle_next)));
		
		pos_curr_min = sphere_curr * dist_min;
		pos_curr_max = sphere_curr * dist_max;
		
		pos_next_min = sphere_next * dist_min;
		pos_next_max = sphere_next * dist_max;
		
		int a = 4 * i;
		int b = 4 * i + 1;
		int c = 4 * i + 2;
		int d = 4 * i + 3;
		
		vertices[a] = pos_curr_min; 
		vertices[b] = pos_curr_max;
		vertices[c] = pos_next_max;
		vertices[d] = pos_next_min;
		
		triangles[6 * i] = a;       // Triangle1: abc
		triangles[6 * i + 1] = b;  
		triangles[6 * i + 2] = c;
		triangles[6 * i + 3] = c;   // Triangle2: cda
		triangles[6 * i + 4] = d;
		triangles[6 * i + 5] = a;
		
		angle_curr += angle_delta;
		angle_next += angle_delta;
		
	}
	
	mesh.vertices = vertices;
	mesh.triangles = triangles;
	
	Graphics.DrawMesh(mesh, new Vector3(this._startPoint.x,0.5f,this._startPoint.z), Quaternion.identity, material, 0);
}


//*/