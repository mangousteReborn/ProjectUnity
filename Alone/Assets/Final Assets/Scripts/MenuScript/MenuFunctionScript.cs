﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuFunctionScript : MonoBehaviour {

    public Slider numberPlayerSlider;
    public InputField gameNameInput;
    public InputField playerNameInput;

    

    public void createServeur()
    {
        Debug.Log(gameNameInput.text.ToString() + " : " + (int)numberPlayerSlider.value);
        Network.InitializeServer((int)numberPlayerSlider.value, 8080,!Network.HavePublicAddress());
        MasterServer.RegisterHost("MyUnityProject", gameNameInput.text.ToString(), "");
    }

    public void quitGame()
    {
        Application.Quit();
    }
}
