using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/*
 * @author : Thomas P
 * @created_at : 28/11/14
 * 
 * Store static game data, as Action, Available Bonus, ect ..
 * 
 */
public static class GameData {

    private static List<Player> _playerList;

	private static Dictionary<string, Vignette> _bonusVignetteMap = new Dictionary<string, Vignette>();

    //private static Dictionary<string, Vignette> _entityVignetteMap = new Dictionary<string, Vignette>();

	private static Dictionary<string, Vignette> _actionVignetteMap = new Dictionary<string, Vignette>();

	private static Dictionary<string, Action> _actionsMap = new Dictionary<string, Action>();

	private static ActionHelperDrawer _actionHelperDrawer;

	private static GameObject _playerCameraObject;

	private static bool initialized = false;

	public static string init(){
		string err = null;
		if(initialized)
			return "GameDatas have been already initialized";
		initialized = true;

		_actionHelperDrawer = GameObject.Find ("Handler").GetComponent<ActionHelperDrawer> ();
		_playerCameraObject = GameObject.Find ("Main Camera");
        _playerList = new List<Player>();
		/*
			VIGNETTES
		 */
		/* Bonuses */
		List<Effect> lifeupEffects = new List<Effect>();
		lifeupEffects.Add(new EffectBonusLife(100));
		_bonusVignetteMap.Add("lifebonus", new VignetteBonus(lifeupEffects, "lifebonus", "Super Vie", null, "Vignettes/lifelow"));
		List<Effect> fakeEffects = new List<Effect>();
		lifeupEffects.Add(new EffectBonusLife(150));
		_bonusVignetteMap.Add("lifebonus2", new VignetteBonus(lifeupEffects, "lifebonus2", "Souper Viche",null, "Vignettes/lifehigh"));
		/* Actions */
        /*Entity Enemy */
        //_entityVignetteMap.Add("Base Enemy", new VignetteEntity(VignetteEntity.EntityType.Base, "Base", "DefaultEnemy"));
		// Move
		Action moveAction = new MoveAction (0.2f);
		WaitAction waitAction = new WaitAction(0.9f);
		DirectDamageAction ddAction= new DirectDamageAction(0.9f, 45f, 2f, 10);

		_actionsMap.Add (moveAction.key, moveAction);
		_actionsMap.Add (waitAction.key, waitAction);
		_actionsMap.Add (ddAction.key, ddAction);
		_actionVignetteMap.Add(moveAction.key, new VignetteAction(moveAction));
		_actionVignetteMap.Add(waitAction.key, new VignetteAction(waitAction));
		_actionVignetteMap.Add(ddAction.key, new VignetteAction(ddAction));

		return err;
	}

	// Bonus Vignette Get
	public static Vignette getBonusVignette(string k){
		Vignette val;
		_bonusVignetteMap.TryGetValue(k, out val);
		return val;
	}

	public static Dictionary<string, Vignette> getBonusVignettes(){
		return _bonusVignetteMap;
	}

	// Action Vignette Get
	public static Vignette getActionVignette(string k){
		Vignette val;
		_actionVignetteMap.TryGetValue(k, out val);
		return val;
	}
	
    public static List<Player> getPlayerList()
    {
        return _playerList;
    }

	public static Dictionary<string, Vignette> getActionVignettes(){
		return _actionVignetteMap;
	}

	// Action Get
	public static Action getAction(string k){
		Action val;
		_actionsMap.TryGetValue(k, out val);
		return val;
	}
	public static Action getCopyOfAction(string k){
		/*
		object[] param = {va.action};
		Action newAction = (Action)va.action.GetType ().GetMethod ("getCopy").Invoke (va.action, param);
		*/
		Action a = null;
		if(!_actionsMap.TryGetValue(k, out a))
			return null;

		return a.getCopy(a);

	}

	public static ActionHelperDrawer getActionHelperDrawer(){
		return _actionHelperDrawer;
	}

	public static GameObject getCameraObject(){
		return _playerCameraObject;
	}

    public static void addPlayer(Player player)
    {
        _playerList.Add(player);
    }

	public static Player getPlayerByNetworkViewID(NetworkViewID id){
		bool f = false;

		foreach (Player p in _playerList) {
			if (p.id == id){
				return p;
			}
		}

		Debug.LogWarning ("GameData : <getPlayerByNetworkViewID> player not found : " + id.ToString ());
		return null;
	}

    public static int getNonGMPlayerCount()
    {
        int playerCount = 0;
        foreach (Player player in _playerList)
        {
            if (!player.isGM)
                playerCount++;
        }
        return playerCount;
    }
}
