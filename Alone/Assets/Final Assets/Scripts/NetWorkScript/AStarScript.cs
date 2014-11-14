using UnityEngine;
using System.Collections;

public class AStarScript {

    private NetworkView networkView;
    private NavMeshPath pathToTargetCorner;


    public AStarScript(NetworkView view)
    {
        this.networkView = view;
    }

    public NavMeshPath setTarget(Vector3 startPos,Vector3 newTarget,bool isFormRPC)
    {
        pathToTargetCorner = getCalcPath(startPos, newTarget);

        //Networking
        if (pathToTargetCorner != null && (!isFormRPC || Network.isServer))
        {
            networkView.RPC("setTargetRPC", RPCMode.Others, networkView.viewID, startPos, newTarget);
        }
        return pathToTargetCorner;
    }

    private NavMeshPath getCalcPath(Vector3 origin, Vector3 wantToGo)
    {
        NavMeshPath path = new NavMeshPath();
        NavMesh.CalculatePath(origin, wantToGo, -1, path);

        if (path.corners.Length > 0)
            return path;
        else
            return null;
    }


    /*
     HOW USE IT : ADD a handler rpc in your IA Class
     * EXAMPLE
     * 
     *  deplacementScript = new AStarScript(networkView);
     *  
     *  [RPC]
        private void changeDirection(string pathCorner)
        {
            deplacementScript.changeDirection(pathCorner);
        }  
     */
}
