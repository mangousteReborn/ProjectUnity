using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InstantiateNPCScript : MonoBehaviour, IPlayerGUI {

    

    [SerializeField]
    private GameObject baseEnemyPrefab;

    [SerializeField]
    private int nbPoints;


    private List<spawnEntityInfos> validatedEntity;
    private List<GameObject> tmpEntity;
    private GameObject currentEntityHandle;
    private GameObject currentPrefab;
    private int currentCost;
    private bool isEntityHandle;
    private int currentRoomNumber;

	// Use this for initialization
	void Start () {
        currentRoomNumber = 1;
        tmpEntity = new List<GameObject>();
        validatedEntity = new List<spawnEntityInfos>();
        currentEntityHandle = null;
        isEntityHandle = false;
	}
	
	// Update is called once per frame
	void Update () {
	    if(isEntityHandle && currentEntityHandle)
        {
            RaycastHit hit;
            Vector3 mousePos = Input.mousePosition;
            //Debug.Log(mousePos);
            mousePos.z = Camera.main.transform.position.y;
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            worldPos.y = currentEntityHandle.transform.position.y - 1;
            Ray ray = new Ray(worldPos, Vector3.down);
            if (Physics.Raycast(ray,out hit))
            {
                if (hit.transform.tag == "Ground")
                {
                    Vector3 entityPos = hit.point;
                    entityPos.y = 3;
                    currentEntityHandle.transform.position = entityPos;
                    if (Input.GetMouseButton(0))
                    {
                        entityPos.y = 1;
                        currentEntityHandle.transform.position = entityPos;
                        tmpEntity.Add(currentEntityHandle);
                        validatedEntity.Add(new spawnEntityInfos(currentPrefab, entityPos));
                        nbPoints -= currentCost;
                        currentCost = 0;
                        currentEntityHandle = null;
                        isEntityHandle = false;
                    }
                }
            }
        }
	}

    public void instantiateEnemy(VignetteEntity.EntityType type, int cost)
    {
        if (nbPoints < 1)
        {
            Debug.Log("not enough point");
            return;
        }
        if (currentEntityHandle != null)
            Destroy(currentEntityHandle);
        if (VignetteEntity.EntityType.Base == type && nbPoints > cost-1)
        {
            Debug.Log(type + "::" + cost);
            currentPrefab = baseEnemyPrefab;
            currentEntityHandle = (GameObject)Instantiate(baseEnemyPrefab);
            currentCost = cost;
            isEntityHandle = true;
        }
    }

    public void onValidate()
    {
        List<GameObject> enemyList = new List<GameObject>();
        foreach(GameObject obj in tmpEntity)
        {
            Destroy(obj);
        }
        foreach(spawnEntityInfos entity in validatedEntity)
        {
            GameObject enemyInstance = (GameObject)Network.Instantiate(entity.entity, entity.position, Quaternion.identity, 10);
            NetworkViewID enemyID = enemyInstance.networkView.viewID;
            enemyList.Add(enemyInstance);
            addToRoomList(currentRoomNumber,enemyID);
            networkView.RPC("addToRoomList", RPCMode.Others, currentRoomNumber, enemyID);
        }
        tmpEntity.Clear();
        validatedEntity.Clear();
    }

    [RPC]
    public void addToRoomList(int roomNumber,NetworkViewID idEnemy)
    {
        roomBattleModeScript room = gameObject.GetComponent<GameManager>().getRoomNumber(roomNumber);
        room.EnemyList.Add(NetworkView.Find(idEnemy).gameObject);
    }

    public void setCharacterManager(CharacterManager cm){}

    public void changeGameMode(uint gameMode){}
}
