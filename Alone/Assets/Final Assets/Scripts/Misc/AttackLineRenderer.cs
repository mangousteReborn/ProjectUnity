using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AttackLineRenderer : MonoBehaviour {

	[SerializeField]
	LineRenderer _lineRenderer;

	[SerializeField]
	Material _lazerMaterial;

	private static int _MAX_ATTACK_COUNT = 5;

	private Stack<AttackInfos> _stack;

	private bool _inProgress;

	void Awake(){
		_lineRenderer.material = _lazerMaterial;
		_lineRenderer.SetWidth (0f, 0f);
		_stack = new Stack<AttackInfos> ();
		_inProgress = false;
	}

	public void addLine(Vector3 start, Vector3 end, float duration){

		start.y = 1;
		end.y = 1;

		_stack.Push(new AttackInfos(start, end, duration));
		if(!_inProgress)
			drawLines ();
	}


	private void drawLines(){
		_inProgress = true;

		if (_stack.Count <= 0) {
			_lineRenderer.SetWidth (0f, 0f);
			_lineRenderer.SetVertexCount (0);
			_inProgress = false;
			return;
		}

		_lineRenderer.SetWidth (1f, 1f);
		_lineRenderer.SetVertexCount (2);
		AttackInfos infos = _stack.Pop ();

		_lineRenderer.SetPosition(0,infos.start);
		_lineRenderer.SetPosition(1, infos.end);

		StartCoroutine( removeLine (infos.duration));
	}
	IEnumerator removeLine(float dur){
		yield return new WaitForSeconds(dur);
		drawLines ();
	}
}

public class AttackInfos {

	private Vector3 _start;

	private Vector3 _end;

	private float _duration;

	public AttackInfos(Vector3 s, Vector3 e, float d){
		this._start = s;
		this._end = e;
		this._duration = d;
	}


	public Vector3 start {
		get {
			return _start;
		}
	}

	public Vector3 end {
		get {
			return _end;
		}
	}

	public float duration {
		get {
			return _duration;
		}
	}
}
