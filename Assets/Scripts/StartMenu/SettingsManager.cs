using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to maintain settings information across scenes
/// </summary>
public class SettingsManager : Singleton<SettingsManager>
{
    public const int maxPlayers = 4;

    /// <summary>
    /// Number of players expected in game 
    /// TODO: make this field changable.
    /// (DEBUG: currently only used for PlayAndPass scene)
    /// </summary>
    public int numberOfPlayers = 2;

    /// <summary>
    /// Materials that get automatically assigned to players in multiplayer games.
    /// Must have all slots prefilled by settingsmanager.
    /// </summary>
    public Material[] playerColors = new Material[maxPlayers];

	// Use this for initialization
	void Start () {
        // save settings for access in gameplay scenes
        Object.DontDestroyOnLoad(gameObject);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
