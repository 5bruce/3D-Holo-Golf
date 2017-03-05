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
            Debug.LogError(this.name + ": no player creation prefab assigned");
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
                //player.BroadcastMessage("Deactivate");
                player.SetActive(false);
                Debug.Log(this.name + ": player1 clone created: " + player.name);
                players.Add(player); 
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void ChangeActivePlayer ()
    {
        // safely deactivate current active player
        //activePlayer.BroadcastMessage("Deactivate");
        activePlayer.SetActive(false);

        // cycle thru players in round-robin schedule
        activePlayer = (players.IndexOf(activePlayer) + 1 < numberOfPlayers)
                        ? players[players.IndexOf(activePlayer) + 1]
                        : players[0];
        Debug.Log(this.name + ": activePlayer: " + activePlayer.name);

        //activePlayer.BroadcastMessage("Activate");
        activePlayer.SetActive(true);
    }
}
