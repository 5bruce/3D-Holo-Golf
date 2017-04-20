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
    public int numberOfPlayers = 1;

    /// <summary>
    /// Materials that get automatically assigned to players in multiplayer games.
    /// Must have all slots prefilled by settingsmanager.
    /// </summary>
    public Material[] playerColors = new Material[maxPlayers];

    public bool isPlayAndPassGame = true;
    public bool isSharedGame = false;

    // Use this for initialization
    void Start () {
        // save settings for access in gameplay scenes
        Object.DontDestroyOnLoad(gameObject);
    }
	
	// Update is called once per frame
	void LateUpdate () {
		if(isPlayAndPassGame && isSharedGame)
        {
            Debug.LogErrorFormat("{0}: {1}: settings set to play-and-pass AND sharing, cannot select both",
                gameObject.name, this.GetType().Name);
            Application.Quit();
        }
	}

    public void toggleSharedGame()
    {
        isSharedGame = (isSharedGame) ? false: true;
        isPlayAndPassGame = (isSharedGame) ? false: true;
    }
}
