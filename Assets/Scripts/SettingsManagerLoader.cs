using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManagerLoader : MonoBehaviour {

    GameObject settingsManager;
    SettingsManager settingsAccess;

	// Use this for initialization
	void Start () {
        // find the SettiingsManager gameobject (from previous scene) and display some test data
        settingsManager = GameObject.Find("SettingsManager");
        if (settingsManager)
        {
            settingsAccess = settingsManager.GetComponent<SettingsManager>();
            Debug.Log(gameObject.name + ": numberOfPlayers = " + settingsAccess.numberOfPlayers);
        }
        else
        {
            Debug.Log(gameObject.name + ": could not find settings manager gameobject");
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
