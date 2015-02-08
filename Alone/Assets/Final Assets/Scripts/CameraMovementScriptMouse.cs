using UnityEngine;
using System.Collections;

public class CameraMovementScriptMouse : MonoBehaviour {
	[SerializeField]
	private float mDelta = 10; // Pixels. The width border at the edge in which the movement work
	
	[SerializeField]
	private float mSpeed = 3.0f; // Scale. Speed of the movement

    //[SerializeField]
    private GameObject restrictAreaObject;

	private bool _lockCamera = true;
    private Transform joueur;
    private bool cameraCanBeLock = true;
    public int marges = 5;

	// Use this for initialization
	void Start () {
        //joueur = transform.parent;
        GameData.init();
        var playerList = GameData.getPlayerList();
        foreach (Player player in playerList)
        {
            if (player.id.isMine)
                joueur = NetworkView.Find(player.id).transform;
        }
	}

    public Transform setJoueur
    {
        set { this.joueur = value; }
    }
	
	// Update is called once per frame
	void Update () {

		// Check if on the right edge
        if (joueur == null)
            return;
		if (!_lockCamera)
		{
            if (Input.mousePosition.x >= Screen.width - mDelta)
            {
                if (checkCameraDeplacement(Vector3.right * Time.deltaTime * mSpeed))
                    transform.position += Vector3.right * Time.deltaTime * mSpeed;
            }
            if (Input.mousePosition.x <= 0 + mDelta)
            {
                if (checkCameraDeplacement(Vector3.left * Time.deltaTime * mSpeed))
                    transform.position += Vector3.left * Time.deltaTime * mSpeed;
            }
            if (Input.mousePosition.y >= Screen.height - mDelta)
            {
                if (checkCameraDeplacement(Vector3.forward * Time.deltaTime * mSpeed))
                    transform.position += Vector3.forward * Time.deltaTime * mSpeed;
            }
            if (Input.mousePosition.y <= 0 + mDelta)
            {
                if (checkCameraDeplacement(-(Vector3.forward * Time.deltaTime * mSpeed)))
                    transform.position -= Vector3.forward * Time.deltaTime * mSpeed;
            }
        }

        if (cameraCanBeLock)
            if (Input.GetKeyDown(KeyCode.E))
			    _lockCamera = !_lockCamera;

		if (_lockCamera)
		{
			float yPos = transform.position.y;
			transform.position = new Vector3(joueur.position.x, yPos, joueur.position.z);
		}
	}

    private bool checkCameraDeplacement(Vector3 deplacementValue)
    {
        
        Vector3 center = restrictAreaObject.transform.position;
        Vector3 newPos = transform.position + 2*deplacementValue;
        if (newPos.x > center.x + (restrictAreaObject.renderer.bounds.size.x / 2) + marges)
        {
            return false;
        }
        if (newPos.x < center.x - (restrictAreaObject.renderer.bounds.size.x / 2) - marges)
        {
            return false;
        }
        if (newPos.z > center.z + (restrictAreaObject.renderer.bounds.size.z / 2) + marges)
        {
            return false;
        }
        if (newPos.z < center.z - (restrictAreaObject.renderer.bounds.size.z / 2) - marges)
        {
            return false;
        }
        return true;
    }

    public void replaceRestictArea(GameObject newRestrictAreaObject)
    {
        this.restrictAreaObject = newRestrictAreaObject;
    }

	public bool lockCamera{
		set{
			this._lockCamera = value;
		}
	}

    public bool cameraCanBeLocked
    {
        set { this.cameraCanBeLock = value; }
    }
}

