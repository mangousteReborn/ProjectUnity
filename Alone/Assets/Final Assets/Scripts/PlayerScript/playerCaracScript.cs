using UnityEngine;
using System.Collections;

public class playerCaracScript : MonoBehaviour {

    private bool _planifyDeplacement;
    public bool PlanifyDeplacement
    {
        get { return _planifyDeplacement; }
        set { _planifyDeplacement = value; }
    }

    private bool _isInFight;
    public bool IsInFight
    {
        get { return _isInFight; }
    }

	// Use this for initialization
	void Start () {
        _isInFight = false;
	}


    [RPC]
    public void enterFight(NetworkViewID id)
    {
        if(Network.isServer)
        {
            if (!_isInFight)
            {
                _isInFight = true;
                networkView.RPC("enterFight", RPCMode.Others, id);
            }
        }
        else
        {
            _isInFight = true;
        }
    }
}
