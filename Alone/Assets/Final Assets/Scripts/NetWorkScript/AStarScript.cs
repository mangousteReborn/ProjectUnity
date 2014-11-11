using UnityEngine;
using System.Collections;

public class AStarScript {

    private NetworkView networkView;
    private NavMeshPath pathToTargetCorner;


    public AStarScript(NetworkView view)
    {
        this.networkView = view;
    }

    public NavMeshPath setTarget(Vector3 startPos,Vector3 newTarget)
    {
        if (Network.isServer)
        {
            pathToTargetCorner = getCalcPath(startPos, newTarget);

            //Networking
            string pathCorner = "";
            if (pathToTargetCorner != null)
            {
                foreach (Vector3 corner in pathToTargetCorner.corners)
                    pathCorner += corner.x.ToString() + "," + corner.y.ToString() + "," + corner.z.ToString() + ";";
                networkView.RPC("changeDirection", RPCMode.Others, networkView.viewID, pathCorner);
            }
            return pathToTargetCorner;
        }
        return null;
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


    public NavMeshPath changeDirection(string pathCorner)
    {
        string[] arrayCorner = pathCorner.Split(';');
        pathToTargetCorner = new NavMeshPath();
        for (int i = 0; i < arrayCorner.Length-1;i++ )
        {
            string[] cornerFloat = arrayCorner[i].Split(',');
            Vector3 cornerVector = new Vector3(float.Parse(cornerFloat[0]), float.Parse(cornerFloat[1]), float.Parse(cornerFloat[2]));
            pathToTargetCorner.corners.SetValue(cornerVector,i);
        }
        return pathToTargetCorner;
    }
}
