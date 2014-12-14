using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InstantiateNPCScript : MonoBehaviour, IPlayerGUI {

    [SerializeField]
    private GameObject baseEnemyPrefab;

    private GameObject currentEntityHandle;
    private bool isEntityHandle;

	// Use this for initialization
	void Start () {
        currentEntityHandle = null;
        isEntityHandle = false;
	}
	
	// Update is called once per frame
	void Update () {
	    if(isEntityHandle && currentEntityHandle)
        {
            RaycastHit hit;
            Vector3 mousePos = Input.mousePosition;
            mousePos = Camera.main.ScreenToWorldPoint(mousePos);
            mousePos.z = 20;
            Ray ray = new Ray(mousePos, Vector3.down);
            Debug.Log("try hit");
            Vector3 entityPos = currentEntityHandle.transform.position;
            entityPos.y = entityPos.y - 1;
            if (Physics.Raycast(entityPos, Vector3.down, out hit))
            {
                Debug.Log("ray hit");
                if (hit.transform.tag == "Ground")
                {
                    Debug.Log("hit");
                    entityPos = hit.point;
                    entityPos.y = 3;
                    currentEntityHandle.transform.position = entityPos;
                    if (Input.GetMouseButton(0))
                    {
                        Debug.Log("pose");
                        entityPos.y = 1;
                        currentEntityHandle.transform.position = entityPos;
                        currentEntityHandle = null;
                        isEntityHandle = false;
                    }
                }
            }
        }
	}

    public void instantiateEnemy(VignetteEntity.EntityType type)
    {
        if (VignetteEntity.EntityType.Base == type)
        {
            currentEntityHandle = (GameObject)Instantiate(baseEnemyPrefab);
            isEntityHandle = true;
        }
    }

    public void setCharacterManager(CharacterManager cm){}

    public void changeGameMode(uint gameMode){}
}
