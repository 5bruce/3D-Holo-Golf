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
    public GameObject firstPlayer;

    SettingsManager settingsAccess;
    int numberOfPlayers;
    List<GameObject> players = new List<GameObject>();
    GameObject activePlayer;

	// Use this for initialization
	void Start () {
        if (!firstPlayer)
        {
            Debug.LogError(this.name + ": no player prefab assigned");
        }
        else
        {
            // ensure that the firstPlayer is active and first to play
            if (firstPlayer.activeSelf == false)
            {
                firstPlayer.SetActive(true);
            }
            activePlayer = firstPlayer;

            // get access to settings information
            settingsAccess = GameObject.Find("SettingsManager").GetComponent<SettingsManager>();

            // get number of player for this game
            numberOfPlayers = settingsAccess.numberOfPlayers;
            Debug.Log(this.name + ": numberOfPlayers = " + numberOfPlayers);

            // add the first player to players list (there should always be a first player)
            players.Add(firstPlayer);

            // create list of other Player gameobject children and instantiate them
            Debug.Log(this.name + ": creating players");
            for (int i = 1; i < numberOfPlayers; i++)
            {
                GameObject player = Instantiate(firstPlayer, firstPlayer.transform.position, firstPlayer.transform.rotation, gameObject.transform);
                player.name = "Player" + (1+i);
                player.SetActive(false);
                Debug.Log(this.name + ": player clone: " + player.name);
                players.Add(player); 
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
