using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Determines which player is currently active
/// </summary>
public class PlayAndPassManager : MonoBehaviour {

    /// <summary>
    /// Prefab or gameobject to use as tempate for generating players
    /// </summary>
    public GameObject playerPrefab;

    SettingsManager settingsAccess;
    int numberOfPlayers;
    List<GameObject> players;

	// Use this for initialization
	void Start () {
        if (!playerPrefab)
        {
            Debug.LogError(this.name + ": no player prefab assigned");
        }
        else
        {
            // get access to settings information
            settingsAccess = GameObject.Find("SettingsManager").GetComponent<SettingsManager>();

            numberOfPlayers = settingsAccess.numberOfPlayers;
            Debug.Log(this.name + ": numberOfPlayers = " + numberOfPlayers);

            // create list of Player gameobject children and instantiate them
            Debug.Log(this.name + ": creating players");
            for (int i = 0; i < numberOfPlayers; i++)
            {
                GameObject player = Instantiate(playerPrefab, playerPrefab.transform);
                Debug.Log(this.name + "player clone: " + player.name);
                players.Add(player);  // FIXME: raising null exception
            }

            // set the first child Player in the list to be the current active
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
