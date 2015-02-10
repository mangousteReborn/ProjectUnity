using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IABase : MonoBehaviour {
	
	const float _costPerUnit  = 0.2f;
	
	private bool DEBUG = false;

	// Use this for initialization
	void Start () {
		//gameObject.

		//OnPlanificationStart();
	}
	
	
	float getPathLength(NavMeshPath path)
	{
		float length = 0f;
		for (int i = 0; i < path.corners.Length; i++) {
			length += path.corners[i].magnitude;		
		}
		
		return length;
	}
	
	
	private int getPositionWithDistanceLeft(CharacterManager cm, NavMeshPath path,float distanceLeft,float pointsLeft) // récupére la position qui aura une distance de distanceLeft avec le dernier corner du path
	{
		int actions = 0;

		MoveAction moveAction = new MoveAction (_costPerUnit);
		float totalDistance = getPathLength (path);
		
		if ( totalDistance< distanceLeft)
		{
			return actions;
		}
		
		float distanceDone = 0;
		
		for(int i = 0; i < path.corners.Length-1; i++){
			
			float costDist = moveAction.calculateCost(path.corners[i],path.corners[i+1]);
			float dist = Vector3.Distance(path.corners[i],path.corners[i+1]);
			
			Vector3 dest = path.corners[i+1];
			
			if(pointsLeft - costDist <= 0f)
			{
				dest = moveAction.getMaxPositionByCost(path.corners[i], path.corners[i + 1], pointsLeft);
				if(dest == Vector3.zero)
				{
					if(DEBUG)Debug.Log("maxPositionByCost return vector3.zero");
					dest = path.corners[i + 1];
				}
				costDist = pointsLeft;
				dist = Vector3.Distance(path.corners[i], dest);
			}
			
			if(DEBUG) Debug.Log("distance done : " + distanceDone);
			if(DEBUG)Debug.Log("distance : " + dist);
			if(DEBUG)Debug.Log("totalDistance : " + totalDistance);
			
			if (distanceDone + dist <= (totalDistance - distanceLeft))
			{
				distanceDone += dist;
				pointsLeft -= costDist;
				//cm.characterStats.pushHotAction(moveAction);
				cm.networkView.RPC("pushMoveActionRPC", RPCMode.All, moveAction.key, moveAction.name, moveAction.desc, moveAction.costPerUnit, costDist, path.corners[i], dest);
				actions ++;
			}
		}
		return actions;
	}
	
	
	public void runPlanification(AIEntityData source)
	{
		//if (!Network.isServer)
		//	return;
		CharacterManager cm = source.tryGetCharacterManager();

		if(null == cm){
			Debug.Log("runPlanification() : CharacterManager is NULL");
			return;
		}

		Player nearest = getNearestPlayer ();
		CharacterStats stat = cm.characterStats;
		//MoveAction moveAction = new MoveAction (_costPerUnit);
		
		float actionPointLeft = stat.maxActionPoint;
		float portee = 0.001f;
		
		NavMeshPath path = new NavMeshPath ();
		NavMeshAgent agent = gameObject.GetComponent<NavMeshAgent>();
		Debug.Log(gameObject.transform.position);
		Debug.Log(nearest.playerObject.transform.position);
		agent.CalculatePath( nearest.playerObject.transform.position, path);
		Debug.Log(path.corners.Length);

		int pushedAction = getPositionWithDistanceLeft (cm, path, portee, actionPointLeft);

		//GameData.getGameManager().networkView.RPC("addReadyAI", RPCMode.All, cm.networkView.viewID, pushedAction);
	}
	
	Player getNearestPlayer()
	{
		Player nearestPlayer = null;
		float nearestPlayerDist = -1;
		List<Player> list = GameData.getPlayerList ();
		foreach (Player player in list)
		{
			if (!player.isGM)
			{
				
				if (nearestPlayer == null)
				{
					nearestPlayer = player;
					nearestPlayerDist = Vector3.Distance(player.playerObject.transform.position, gameObject.transform.position);
				}
				else
				{
					float newDist = Vector3.Distance(player.playerObject.transform.position, gameObject.transform.position);
					if (newDist < nearestPlayerDist)
					{
						nearestPlayerDist = newDist;
						nearestPlayer = player;
					}
				}
			}
		}
		return nearestPlayer;
	}
	
	void OnSimulationStart()
	{
		
	}
}