using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ServeurListScript : MonoBehaviour {

    HostData[] data;
    public Text textPrefab;
    public Button buttonPrefab;

    public Canvas canvasRef;

	// Use this for initialization
	void OnEnable () {
        Debug.Log("toto au ski");
        MasterServer.RequestHostList("MyUnityProject");
        data = MasterServer.PollHostList();
        drawServeur();
	}
	
	// Update is called once per frame
	void Update () {
        MasterServer.RequestHostList("MyUnityProject");
        HostData[] newData = MasterServer.PollHostList();
        if (newData.Length != data.Length)
        {
            data = newData;
            drawServeur();
        }
    }

    void drawServeur()
    {
        Vector3 position = Vector3.zero;
        canvasRef.transform.DetachChildren();
        foreach (HostData element in data)
        {
            string name = element.gameName + " " + element.connectedPlayers + " / " + element.playerLimit;
            Text text = (Text)Instantiate(textPrefab);
            string hostInfo;
            hostInfo = "[";
            foreach (string host in element.ip)
                hostInfo = hostInfo + host + ":" + element.port + " ";
            hostInfo = hostInfo + "]";
            text.text = name + " " + hostInfo;
            text.transform.parent = canvasRef.transform;
            text.transform.position = position;
            /*if (GUILayout.Button("Connect"))
            {
                // Connect to HostData struct, internally the correct method is used (GUID when using NAT).
                Network.Connect(element);
            }*/
        }
    }
}
