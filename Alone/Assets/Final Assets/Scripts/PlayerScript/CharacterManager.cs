using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
/*
 * @author: Thomas P
 * @created_at:15/11/2014
 * 
 * Character manager for Player char. and IA char.
 * /!\ MUST BE GENERIC, CAUSE WILL BE USED BY BOTH AI/PLAYER CHARACTERS
 */
public class CharacterManager : MonoBehaviour {
	
	CharacterStats _characterStats;
		
	[SerializeField]
	private GameObject _damagePopup;

	[SerializeField]
	private GameObject _healthBar;

	private Scrollbar _healthBarScrollbar;
	private Text _healthBarLabel;
		

	[SerializeField]
	public GameObject _character;

	void Start () {
		this._characterStats = new CharacterStats ();

		// HealthBar init (if defined)
		if (this._healthBar != null) {

			this._healthBar = (GameObject)Instantiate (this._healthBar);
			//this._healthBar.transform.parent = this._character.transform;

			this._healthBarScrollbar = this._healthBar.GetComponentInChildren<Scrollbar>();
			this._healthBarLabel = this._healthBar.GetComponentInChildren<Text>();

			this._characterStats.listenersList.Add(updateHealthBar);


		}

		this._characterStats.register (CharacterStatsEvent.currentLifeChange, createPopup);


		this._characterStats.pushEffect(new EffectDamageOverBattle(10, 3));

		this._characterStats.hasChanged();
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.T)) {
			
			this._characterStats.currentLife += (int)UnityEngine.Random.Range(-5,5);
		}
		if (Input.GetKeyDown (KeyCode.Y)) {
			
			this._characterStats.currentLife += 1;
		}
		if (Input.GetKeyDown (KeyCode.M)) {
			Vignette v = GameData.getBonusVignette("lifebonus");
			Debug.Log("Vignette === " + v.name);
		}
		  //!\\
		 //!!!\\
		//!!!!!\\
		this._healthBar.transform.position = new Vector3(this._character.transform.position.x,3,this._character.transform.position.z + 1);
	}

	private void updateHealthBar(CharacterStats stats,object[] param){
		this._healthBarScrollbar.size = (float)stats.currentLife / (float)stats.maxLife;
		this._healthBarLabel.text = stats.currentLife + " / " + stats.maxLife;

	}

	private void createPopup(CharacterStats stats, object[] param){
		int damages = (int)param [0] - (int)param [1];
		Color color = damages >= 0 ? Color.red : Color.green;

		GameObject popup = (GameObject)Instantiate (this._damagePopup, this._character.transform.position,  Quaternion.identity);


		popup.GetComponentInChildren<Text> ().color = color;
		popup.GetComponentInChildren<Text> ().text = damages >= 0 ? ""+damages : "+"+damages*-1;
	}

	public void dealDamages(int damages){
		this._characterStats.currentLife -= damages;

	}

	// Get / Seters
	public CharacterStats characterStats{
		get {
			return this._characterStats;
		}
	}


}
