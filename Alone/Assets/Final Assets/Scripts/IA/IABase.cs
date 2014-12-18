using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IABase : MonoBehaviour {

	// Use this for initialization
	void Start () {
	//gameObject.
        managerCharacter = gameObject.GetComponent<CharacterManager>();
        OnPlanificationStart();
	}
	const float costPerUnit  = 0.2f;

    private CharacterManager managerCharacter;

	static float getPathLength(NavMeshPath path)
	{
		float length = 0f;

		for (int i = 0; i < path.corners.Length-1; i++) {
			length += Vector3.Distance(path.corners[i],path.corners[i+1] );		
		}

		return length;
	}


	static Vector3 getPositionWithDistanceLeft(NavMeshPath path,float distanceLeft,float pointsLeft) // récupére la position qui aura une distance de distanceLeft avec le dernier corner du path
	{
		MoveAction moveAction = new MoveAction (costPerUnit);
		float totalDistance = getPathLength (path);

		if ( totalDistance< distanceLeft)
			return path.corners [0];
		float distanceDone = 0;

		for(int i = 0; i < path.corners.Length-1; i++){

			float costDist = moveAction.calculateCost(path.corners[i],path.corners[i+1]);
			float dist = Vector3.Distance(path.corners[i],path.corners[i+1]);

			if(pointsLeft - costDist <= 0f)
			{
				if(pointsLeft-costDist == 0f)
					return path.corners[i+1];
				Vector3 norm = Vector3.Normalize(path.corners[i+1]- path.corners[i]);  
				return path.corners[i] + norm*(pointsLeft/costPerUnit);
			}

			if(distanceDone+dist < (totalDistance - distanceLeft) ){
				distanceDone+= dist;
				pointsLeft-= costDist;
			}
			else {
				float distanceLeftToDistanceLeft = (totalDistance-distanceLeft)-distanceDone;
				return path.corners[i] + Vector3.Normalize(path.corners[i+1]-path.corners[i])*distanceLeftToDistanceLeft;
			}

		}

		return new Vector3 (-1, -1, -1);
	}


	void OnPlanificationStart()
	{
		if (!Network.isServer)
			return;
		
		Player nearest = getNearestPlayer ();
        CharacterStats stat = managerCharacter.characterStats;
		MoveAction moveAction = new MoveAction (costPerUnit);

		float actionPointLeft = stat.maxActionPoint;
		float portee = 1f;

		NavMeshPath path = new NavMeshPath ();
		NavMesh.CalculatePath (gameObject.transform.position, nearest.playerObject.transform.position,0 , path );
		Vector3 dest = getPositionWithDistanceLeft (path, portee, actionPointLeft);


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




}
