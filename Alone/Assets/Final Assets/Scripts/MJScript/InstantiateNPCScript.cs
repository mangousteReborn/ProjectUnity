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
            Vector3 entityPosition = Input.mousePosition;
            entityPosition.y = 3;
            currentEntityHandle.transform.position = entityPosition;
            if(Input.GetMouseButton(0))
            {
                entityPosition.y = 1;
                currentEntityHandle.transform.position = entityPosition;
                currentEntityHandle = null;
                isEntityHandle = false;
            }
        }
	}

    public void instantiateEnemy()
    {
        //GetVignetteEntityType

        /*if (VignetteEntity.EntityType.Base == getVignetteGetEntityType)
        {
            currentEntityHandle = (GameObject)Instantiate(baseEnemyPrefab);
            isEntityHandle = true;
        }*/


    }

    public void setCharacterManager(CharacterManager cm){}

    public void changeGameMode(uint gameMode){}
}
