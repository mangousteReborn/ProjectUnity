using UnityEngine;

using System;
using System.Collections;

public class DirectDamageAction : Action {
	private DirectDamageHelperScript _helper;

	private float _costPerUnit;
	private float _degree;
	private float _radius;

	private int _damages;

	private int _maxAttacks = 1; // Maybe player (CharacterStats) have to manage it

	private Action<Collider> _onCollision;
	private float _definedAngle;

	// Used by GameData for definition
	public DirectDamageAction(float costPerUnit, float degree, float radius, int damages)
		: base("directdamage", "Traquer", "Attend que ta proie soit dans ta ligne de mire puis FEU !")
	{
		this._costPerUnit = costPerUnit;
		this._degree = degree;
		this._radius = radius;
		this._damages = damages;
	}
	
	public DirectDamageAction(string k, string name, string d, float costPerUnit,float degree, float radius, int damages)
		: base (k, name, d)
	{
		this._costPerUnit = costPerUnit;
		this._degree = degree;
		this._radius = radius;
		this._damages = damages;
	}
	
	public DirectDamageAction (
		string k, string name, string d,  
		float costPerUnit,float degree, float radius, int damages, 
		float cost,Vector3 startPosition, Vector3 destPosition
	)
		: base(k, name, d)
	{
		this._degree = degree;
		this._radius = radius;
		this._damages = damages;

		this._actionCost = cost;
		this._startPosition = startPosition;
		this._endPosition = destPosition;
		
	}
	
	public override Action getCopy(Action a){
		DirectDamageAction ma = null;
		
		if (System.Object.ReferenceEquals(a.GetType (), this.GetType())) {
			ma = (DirectDamageAction)a;
		} else {
			Debug.LogError("DirectDamageAction have been initialized with bad parameters");
			return null;
		}
		
		return new DirectDamageAction (ma.key, ma.name, ma.desc,ma.costPerUnit, ma.degree, ma.radius, ma.damages);
	}

	public override void onActionSelection(CharacterManager cm, bool drawHelper=true){
		ActionHelperDrawer ahd = GameData.getActionHelperDrawer ();
		
		this._helper = ahd.pushDirectDamageHelper(cm, this);
		
		this._helper.activate (cm, this);
	}
	
	public override void onActionValidation(CharacterManager cm, object[] param){

		
	}
	
	public override void onActionInvalid (CharacterManager cm, object[] param)
	{
		if(this._helper != null)
			this._helper.RPCcallback = true;
	}


	public override void onActionCanceled(CharacterManager cm, object[] param=null){
		this._helper.cancel (cm);
		
	}
	
	public override void onActionStart(CharacterManager cm, object[] param=null){
		//GameObject go = (GameObject)GameObject.Instantiate(getCollider(), this._endPosition, Quaternion.identity);
		int colliderMode = 0;

		if(null != param){
			if (param[0]!=null){
				colliderMode = (int)param[0];
			}
		}

		if(0 == colliderMode){
			useMeshCollider(
				cm,
				buildMesh(10, this._degree, 0.1f, this._radius*10, this._definedAngle)
				);
		} else if (1 == colliderMode){
			useSphereCollider(cm);
		} else {
			Debug.LogError("<DirectDamageAction> onActionStart : Cannot create collider with param : " + colliderMode);
			return;
		}

	}
	
	public override void onActionEnd(CharacterManager cm, object[] param=null){

	}


	public Mesh buildMesh(int quality, float angle_fov,float dist_min,float dist_max, float startAngle=0f){
		// Global def
		Mesh mesh = new Mesh();
		
		mesh.name = "cone_mesh";
		mesh.vertices = new Vector3[4 * quality];   // Could be of size [2 * quality + 2] if circle segment is continuous
		mesh.triangles = new int[3 * 2 * quality];
		
		Vector3[] normals = new Vector3[4 * quality];
		Vector2[] uv = new Vector2[4 * quality];
		
		for (int i = 0; i < uv.Length; i++)
			uv[i] = new Vector2(0, 0);
		for (int i = 0; i < normals.Length; i++)
			normals[i] = new Vector3(0, 1, 0);
		
		mesh.uv = uv;
		mesh.normals = normals;
		
		// Vertice fill
		float angle_lookat = startAngle;
		
		float angle_start = angle_lookat - angle_fov;
		float angle_end = angle_lookat + angle_fov;
		float angle_delta = (angle_end - angle_start) / quality;
		
		float angle_curr = angle_start;
		float angle_next = angle_start + angle_delta;
		
		Vector3 pos_curr_min = Vector3.up;//zero
		Vector3 pos_curr_max = Vector3.up;
		
		Vector3 pos_next_min = Vector3.up;
		Vector3 pos_next_max = Vector3.up;
		
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
		
		return mesh;
	}

	public float calculateCost(Vector3 start, Vector3 dest){
		return (float)Math.Round(
			(Vector3.Distance(start, dest) * this._costPerUnit),
			2,
			MidpointRounding.ToEven
			);
	}
	
	public GameObject useSphereCollider(CharacterManager cm){
		
		GameObject go = new GameObject("AngleCollider_"+(int)this._degree);
		go.layer = LayerMask.NameToLayer("AngleCollider");
		//Transform t = go.AddComponent<Transform> ();
		Transform t = go.GetComponent<Transform>();
		t.position = this._endPosition;
		
		SphereCollider sc = go.AddComponent<SphereCollider> ();
		sc.radius = this._radius*10;
		sc.isTrigger = true;
		this._onCollision = delegate (Collider c){

			GameObject player = c.gameObject;

			Debug.Log("hey" + player.name);

		};

		AngleCollider ac = go.AddComponent<AngleCollider> ();
		ac.activate(this._radius*10, this._onCollision);

		return go;
	}

	public GameObject useMeshCollider(CharacterManager cm, Mesh mesh){
		Debug.Log("this._mesh == " + mesh);

		GameObject go = new GameObject("AngleCollider_"+(int)this._degree);
		go.layer = LayerMask.NameToLayer("AngleCollider");
		//Transform t = go.AddComponent<Transform> ();
		Transform t = go.GetComponent<Transform>();
		t.position = this._endPosition;
		
		MeshCollider mc = go.AddComponent<MeshCollider> ();
		mc.sharedMesh = mesh;
		mc.isTrigger = true;
		this._onCollision = delegate (Collider c){

			CharacterManager target = c.gameObject.GetComponent<CharacterManager>();

			if(!target.networkView.isMine && target.characterStats.hasTargetType(CharacterStats.TargetType.ally)){
				RaycastHit hit;
				Debug.Log("Ok, is ally");
				if(
					Physics.Raycast(new Vector3(this._startPosition.x, 0.5f, this._startPosition.z), new Vector3(target.character.transform.position.x,0.5f, target.character.transform.position.z), out hit,this._radius*10)
					){
					Debug.Log("Ok raycast hit");
					int newLife = target.characterStats.currentLife - (cm.characterStats.currentStrength + this.damages);

					target.networkView.RPC("setCurrentLife", RPCMode.All, newLife);
					/*
					if(Network.isServer){
						target.networkView.RPC("setCurrentLife", RPCMode.Server, newLife);
					} else {
						target.characterStats.setCurrentLife(newLife, false);
					}
					*/

				}

			}
			Debug.Log("hey ::: " + target.player.name + " is ::: " + target.characterStats.targetTypesToString());
			
		};
		
		AngleCollider ac = go.AddComponent<AngleCollider> ();
		ac.activate(this._radius*10, this._onCollision);

		return go;
	}

	// Get / Set
	public float definedAngle{
		get{return this._definedAngle;}
		set{this._definedAngle = value;}
	}
	/*
	public Mesh mesh{
		get{return this._mesh;}
		set{this._mesh = value;}
	}
	*/
	public float costPerUnit{
		get {return this._costPerUnit;}
	}
	public float degree{
		get {return this._degree;}
	}
	public float radius{
		get {return this._radius;}
	}
	public int damages{
		get {return this._damages;}
	}
}
