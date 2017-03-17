﻿using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Loads and stores relevant settings information for the scene.
/// </summary>
public class SettingsManagerLoader : Singleton<SettingsManagerLoader> {

    GameObject settingsManager;
    
    /// <summary>
    /// Gives access to the settings information of a SettingsManager component
    /// </summary>
    SettingsManager settingsAccess;

    public int numberOfPlayers;

    [Tooltip("Material inserted in the inspector will be overwritten at startup and so should only be used as defaults/backups")]
    public Material[] playerColors;

	// Use this for initialization
    // FIXME: This is not necessarily called before other Start() methods of other components.
    // Thus other scripts tryings to access settings data from here may get null exception.
    // For this reason, may need to use Awake() method; tho I think HoloToolkit/Utility/Singleton.cs takes care of this.
	void Start () {
        // find the SettiingsManager gameobject (from previous scene) and display some test data
        settingsManager = GameObject.Find("SettingsManager");
        if (settingsManager)
        {
            settingsAccess = settingsManager.GetComponent<SettingsManager>();

            numberOfPlayers = settingsAccess.numberOfPlayers;
            playerColors = settingsAccess.playerColors;
        }
        else
        {
            Debug.LogError(gameObject.name + ": " + this.GetType().Name + 
                ": could not find a settings manager gameobject. Setting default values.");
           
            numberOfPlayers = 2;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
