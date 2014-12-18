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


	private void getPositionWithDistanceLeft(NavMeshPath path,float distanceLeft,float pointsLeft) // récupére la position qui aura une distance de distanceLeft avec le dernier corner du path
	{
		MoveAction moveAction = new MoveAction (costPerUnit);
		float totalDistance = getPathLength (path);

		if ( totalDistance< distanceLeft)
        {
            Debug.Log("return1");
            return;
        }

		float distanceDone = 0;

		for(int i = 0; i < path.corners.Length-1; i++){

			float costDist = moveAction.calculateCost(path.corners[i],path.corners[i+1]);
			float dist = Vector3.Distance(path.corners[i],path.corners[i+1]);

			if(pointsLeft - costDist <= 0f)
            {
                Debug.Log("return2");
                return;
            }

            Debug.Log("distance done : " + distanceDone);
            Debug.Log("distance : " + dist);
            Debug.Log("totalDistance : " + totalDistance);

            if (distanceDone + dist <= (totalDistance - distanceLeft))
            {
                distanceDone += dist;
                pointsLeft -= costDist;
                Debug.Log("add to stack");
                managerCharacter.characterStats.pushHotAction(moveAction);
                networkView.RPC("pushMoveActionRPC", RPCMode.Others, moveAction.key, moveAction.name, moveAction.desc, moveAction.costPerUnit, costDist, path.corners[i], path.corners[i+1]);
            }
		}
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
		NavMesh.CalculatePath (gameObject.transform.position, nearest.playerObject.transform.position,-1 , path );
		getPositionWithDistanceLeft (path, portee, actionPointLeft);
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
