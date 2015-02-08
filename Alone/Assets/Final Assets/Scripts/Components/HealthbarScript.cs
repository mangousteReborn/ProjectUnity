using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthbarScript : MonoBehaviour {

	[SerializeField]
	GameObject _healthbar;

	[SerializeField]
	Vector3 _scale;


	private Scrollbar _healthBarScrollbar;

	private Text _healthBarLabel;


	void Awake () {
		this.transform.localScale = _scale;
		this._healthBarScrollbar = _healthbar.GetComponentInChildren<Scrollbar>();
		this._healthBarLabel = _healthbar.GetComponentInChildren<Text>();
		this._healthBarLabel.color = Color.white;
		// Triggering event for first healthbar update

		//this._characterStats.fireEvent(CharacterStatsEvent.change);

	}

	public void setLife(int curr, int max){
		this._healthBarScrollbar.size = (float)curr / (float)max;
		this._healthBarLabel.text = curr + " / " + max;
	}
	

}
