using UnityEngine;
using System.Collections;

public class StaticVariableScript : MonoBehaviour {

    public static bool isServer=false;
    public static bool isGameMaster = false;
    public static string ip = "";

    public static int numberClient= -1;

    private int port = 25000;

    public void setIsServer(bool value)
    {
        isServer  = value;
        isGameMaster = value;
    }

    public void setIp(string ipValue)
    {
        ip = ipValue;
    }

    public bool createServer()
    {
        Network.InitializeSecurity();
        NetworkConnectionError error = Network.InitializeServer(3, port, false);
        if (error == NetworkConnectionError.NoError)
            return true;
        return false;
    }

    public bool ConnectToServer()
    {
        NetworkConnectionError error = Network.Connect(ip, port);
        if (error == NetworkConnectionError.NoError)
            return true;
        return false;
    }
}
