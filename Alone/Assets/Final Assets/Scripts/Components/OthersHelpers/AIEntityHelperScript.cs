using UnityEngine;
using System.Collections;
using System;
using UnityEngine.EventSystems;

public class AIEntityHelperScript : MonoBehaviour {

	private bool _RPCcallback = true;
	private bool _validated = false;
	private bool _activated = false;

	[SerializeField]
	private GameObject _object;

	[SerializeField]
	private Transform _transform;

	[SerializeField]
	private MeshRenderer _meshRenderer;

	[SerializeField]
	private MeshFilter _mesh;

	private Action<Vector3> _validationAction;

	public void activate(Mesh mesh, Transform t, Action<Vector3> a){
		_mesh.mesh = mesh;
		_activated = true;
		_transform.position = t.position;
		_validationAction = a;
	}
	
	
	void Update(){
		if (!this._activated)
			return;
		
		if (this._validated)
			return;

		Vector3 nextPos = getTarget ();

		if (Vector3.zero == nextPos) {
			_meshRenderer.material.color = new Color(1f,0f,0f,0.3f);
			return;
		}

		_meshRenderer.material.color = new Color(1f,1f,1f,0.3f);
		_transform.position = nextPos;

		if (Input.GetMouseButton (0) && !EventSystem.current.IsPointerOverGameObject()) {
			this._validated = true;
			if(!GameData.myself.isGM){
				Debug.LogError("A non GM player has this helper .. FIX THAT !");
			}
			_validationAction(nextPos);

			/*
			Vector3 target = getTarget();
			
			if(this._RPCcallback){
				this._owner.networkView.RPC("validateMoveActionRPC", RPCMode.All, this._owner.player.id, this._endPoint);
				this._RPCcallback = false;
			}
			return;
			*/
		}
	}

	public void delete(){
		Destroy (_object);
	}

	private Vector3 getTarget()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		Vector3 target = Vector3.zero;
		if (Physics.Raycast(ray, out hit) && hit.transform.tag == "Ground")
		{
			target = hit.point;
			target.y = 0;
		}
		return target;
	}
}
