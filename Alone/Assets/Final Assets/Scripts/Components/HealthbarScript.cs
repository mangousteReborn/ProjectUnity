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

	private CharacterStats _characterStats;

	void Start () {
		this.transform.localScale = _scale;
		this._healthBarScrollbar = _healthbar.GetComponentInChildren<Scrollbar>();
		this._healthBarLabel = _healthbar.GetComponentInChildren<Text>();
		this._healthBarLabel.color = Color.white;
		// Triggering event for first healthbar update

		this._characterStats.fireEvent(CharacterStatsEvent.change);

	}

	private void updateHealthBar(CharacterStats stats,object[] param){
		this._healthBarScrollbar.size = (float)stats.currentLife / (float)stats.maxLife;
		this._healthBarLabel.text = stats.currentLife + " / " + stats.maxLife;
		
	}

	public void setCharacterStats(CharacterStats cs){
		this._characterStats = cs;
		this._characterStats.register (CharacterStatsEvent.change, updateHealthBar);
	}

}
