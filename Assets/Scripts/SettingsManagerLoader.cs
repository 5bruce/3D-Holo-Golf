using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManagerLoader : Singleton<SettingsManagerLoader> {

    GameObject settingsManager;
    public SettingsManager settingsAccess;

    public int numberOfPlayers;

	// Use this for initialization
    // FIXME: This is not necessarily called before other Start() methods of other components.
    //        Thus other scripts tryings to access settings data from here may get null exception.
	void Start () {
        // find the SettiingsManager gameobject (from previous scene) and display some test data
        settingsManager = GameObject.Find("SettingsManager");
        if (settingsManager)
        {
            settingsAccess = settingsManager.GetComponent<SettingsManager>();
            numberOfPlayers = settingsAccess.numberOfPlayers;
        }
        else
        {
            Debug.LogError(this.name + ": could not find a settings manager gameobject");
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
