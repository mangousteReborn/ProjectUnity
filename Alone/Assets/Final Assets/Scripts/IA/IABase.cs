using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IABase : MonoBehaviour {

	// Use this for initialization
	void Start () {
	//gameObject.

	}
	const float costPerUnit  = 0.2f;

	static float getPathLength(NavMeshPath path)
	{
		float length = 0f;
		MoveAction moveAction = new MoveAction (costPerUnit);

		for (int i = 0; i < path.corners.Length-1; i++) {
			length += moveAction.calculateCost(path.corners[i],path.corners[i+1] );		
		}

		return length;
	}


	static Vector3 getPositionWithDistanceLeft(NavMeshPath path,float distanceLeft,float pointLeft) // récupére la position qui aura une distance de distanceLeft avec le dernier corner du path
	{
		float totalDistance = getPathLength (path);

		if ( totalDistance< distanceLeft)
			return path.corners [0];
		float distanceDone = 0;

		for(int i = 0; i < path.corners.Length-1; i++){
			float dist = moveAction.calculateCost(path.corners[i],path.corners[i+1]);
			if(distanceDone+path.corners[i] < (totalDistance - distanceLeft) ){
				distanceDone+= dist;
			}
			else {
				float distanceLeftToDistanceLeft = (totalDistance-distanceLeft)-distanceDone;
				return path.corners[i] + Vector3.Normalize(path.corners[i],path.corners[i+1])*distanceLeftToDistanceLeft;
			}

		}

		return new Vector3 (-1, -1, -1);
	}


	void OnPlanificationStart()
	{
		if (!Network.isServer)
			return;
		
		Player nearest = getNearestPlayer ();
		CharacterStats stat = gameObject.GetComponent<CharacterManager> ().characterStats;
		MoveAction moveAction = new MoveAction (costPerUnit);

		float actionPointLeft = stat.maxActionPoint;

		NavMeshPath path = new NavMeshPath ();
		NavMesh.CalculatePath (gameObject.transform.position, nearest.playerObject.transform.position,0 , path );
	/*
		if(path.status == NavMeshPathStatus.PathComplete)
			for (int i = 0; i < path.corners.Length-1; i++) {
				float cost = moveAction.calculateCost(path.corners[i],path.corners[i+1];
			    if( actionPointLeft > cost){                     
			        actionPointLeft-= cost;
				}
			    else {
					//TODO : calculer le point le plus eloigné atteignable avec les points restants

				}

					
			}
	*/

	}
	
	Player getNearestPlayer()
	{
		Player nearestPlayer = null;
		float nearestPlayerDist = -1;
		List<Player> list = GameData.getPlayerList ();
		foreach (Player player in list)
		if (!player.isGM ) {
			
			if(nearestPlayer == null)
			{
				nearestPlayer = player;
				nearestPlayerDist = Vector3.Distance(player.playerObject.transform.position,gameObject.transform.position);
			}
			else 
			{
				float newDist =  Vector3.Distance(player.playerObject.transform.position,gameObject.transform.position);
				if(newDist < nearestPlayerDist) 
				{
					nearestPlayerDist = newDist;
					nearestPlayer = player;
				}
			}
		}
		
		return nearestPlayer;
	}

	void OnSimulationStart()
	{



	}

	// Update is called once per frame
	void Update () {




}
